<#
.SYNOPSIS
    Runs locally what CI runs, in the same order.

.DESCRIPTION
    One command answers whether a change is ready. Running the same steps CI
    runs, in the same order, means a green local run and a red pipeline stop
    disagreeing about what "ready" means.

    Suites run through dotnet run rather than dotnet test. xUnit v3 builds each
    test project as an executable on Microsoft.Testing.Platform, and on the
    .NET 10 SDK dotnet test reports zero tests for these projects while the
    executables themselves discover and run everything.

.PARAMETER SkipRegression
    Skips the Playwright suite, which needs a browser and starts the
    application.

.PARAMETER SkipMutation
    Skips the mutation run, the slowest step by a wide margin.

.EXAMPLE
    .\scripts\verify.ps1
    .\scripts\verify.ps1 -SkipMutation
#>
[CmdletBinding()]
param(
    [switch]$SkipRegression,
    [switch]$SkipMutation
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$repoRoot = Split-Path -Parent $PSScriptRoot
Push-Location $repoRoot

$configuration = 'Release'
$framework = 'net10.0'

$fastSuites = @(
    'tests/App.Tests/App.Tests.csproj',
    'tests/Analyzer.Tests/Analyzer.Tests.csproj'
)

function Invoke-Step {
    param(
        [Parameter(Mandatory)][string]$Name,
        [Parameter(Mandatory)][scriptblock]$Action
    )

    Write-Host ''
    Write-Host "==> $Name" -ForegroundColor Cyan
    & $Action
    if ($LASTEXITCODE -ne 0) {
        Write-Host "FAILED: $Name" -ForegroundColor Red
        Pop-Location
        exit $LASTEXITCODE
    }
}

try {
    # The build carries the lint: warnings are errors, code style is enforced
    # during compilation, and the literal analyzer runs as part of it.
    Invoke-Step 'restore' { dotnet restore }
    Invoke-Step 'build' { dotnet build --configuration $configuration --no-restore }

    foreach ($suite in $fastSuites) {
        Invoke-Step "test $suite" {
            dotnet run --project $suite --configuration $configuration --no-build
        }
    }

    if (-not $SkipRegression) {
        # Playwright's own install script, emitted into the build output. The
        # browser version comes from the package reference, so the binaries
        # match the client rather than drifting to whatever is newest.
        $install = "tests/Regression.Tests/bin/$configuration/$framework/playwright.ps1"
        Invoke-Step 'install browsers' { pwsh $install install chromium }

        # ApplicationServer starts and stops the app around the run, so no
        # separate server step appears here.
        Invoke-Step 'regression' {
            dotnet run --project tests/Regression.Tests/Regression.Tests.csproj `
                --configuration $configuration --no-build
        }
    }
    else {
        Write-Host ''
        Write-Host '==> regression skipped' -ForegroundColor Yellow
    }

    if (-not $SkipMutation) {
        # The mutation engine comes from the tool manifest rather than the
        # machine, so a clean checkout has to restore it before the command
        # exists at all.
        Invoke-Step 'restore tools' { dotnet tool restore }

        # Stryker runs from the project under test rather than from here.
        # Standing at a solution makes it test every project in that solution,
        # which drags the 25 browser specs into the mutant set and leaves the
        # run needing a browser and a started application. Standing in src\App
        # restricts it to App.Tests, which kills every mutant on its own and
        # finishes in seconds. The config stays at the root and gets passed in.
        Push-Location (Join-Path $repoRoot 'src/App')
        try {
            Invoke-Step 'mutation' { dotnet stryker -f ..\..\stryker-config.json }
        }
        finally {
            Pop-Location
        }
    }
    else {
        Write-Host ''
        Write-Host '==> mutation skipped' -ForegroundColor Yellow
    }

    Write-Host ''
    Write-Host 'All checks passed.' -ForegroundColor Green
}
finally {
    Pop-Location
}
