# Blazor Standards

A working .NET 10 Blazor application that demonstrates how I expect a Blazor
front end to be built, and how it must supply what a regression suite needs.
Every standard below is enforced by the build, a test, or an analyzer. Nothing
here is advice; a violation fails.

The sibling projects `cypress-standards`, `playwright-standards`, and
`playwright-standards-csharp` all bind to `data-testid` attributes that
application code has to supply. None of them shows where those attributes come
from. This one does.

## Running it

Windows, PowerShell:

```powershell
dotnet run --project src\App
```

The application answers on `http://localhost:5280`. Three pages: tasks at `/`,
signup at `/signup`, inventory at `/inventory`.

Everything CI runs:

```powershell
.\scripts\verify.ps1
.\scripts\verify.ps1 -SkipMutation   # faster
```

Individual suites run as executables rather than through `dotnet test`:

```powershell
dotnet run --project tests\App.Tests
dotnet run --project tests\Analyzer.Tests
dotnet run --project tests\Regression.Tests
```

xUnit v3 brings Microsoft.Testing.Platform with it, and the .NET 10 SDK refuses
to drive a platform project through the VSTest target that `dotnet test` uses.
The command fails loudly rather than reporting a false pass. The VSTest adapter
stays referenced anyway, because Stryker drives tests through VSTest and cannot
see a platform application without it.

## Secrets

Nothing in this repository holds a secret, and nothing needs one to run. The
keys exist to show where a real application's credentials belong.

Local development uses .NET user secrets. From `src/App`:

```powershell
dotnet user-secrets set "App:AdminPassword" "value"
dotnet user-secrets list
```

Values land in the user profile, outside the repository entirely. Ignoring a
file stops it reaching a commit, not a reader: editor extensions, language
servers, AI assistants, and any dependency with a build-time script can read
anything with filesystem access to the project.

Deployed and cadenced runs inject the same keys as environment variables, and
environment variables win over user secrets so a pipeline never reads a
developer's machine. `secrets.example.json` documents the key names and commits
safely because it holds none.

### One key, every target

.NET's configuration binder maps the same key across every store:

| Target | Form of `App:AdminPassword` |
| --- | --- |
| .NET user secrets | `App:AdminPassword` |
| Environment variable, any OS or CI | `App__AdminPassword` |
| GitHub Actions | repository secret, exported as `App__AdminPassword` |
| Azure App Service application setting | `App__AdminPassword` |
| Azure Key Vault via the Key Vault provider | `App--AdminPassword` |
| AWS Secrets Manager or SSM | store as JSON, export as `App__AdminPassword` |
| GCP Secret Manager | store the value, export as `App__AdminPassword` |

Colon becomes double underscore in an environment variable and double hyphen in
a Key Vault secret name. Those two rules cover every target above.

| Key | Default | Purpose |
| --- | --- | --- |
| `App:BaseUrl` | `http://localhost:5280` | Point the regression suite at a deployed environment |
| `App:AdminUser` | none | Shows where a real application's credentials belong |
| `App:AdminPassword` | none | Unused by this project |
| `App:ApiToken` | none | Unused by this project |

## The standards

**1. Test ids are a contract, and one helper renders them.** Every element a
suite reaches carries `data-testid` from `TestIds`, and no component writes the
attribute name. `TestId.For(TestIds.Task.AddButton)` splats it through
`@attributes`, so the name lives in one constant and a rename touches one file.
*Enforced by* a test that scans every authored `.razor` file for the attribute
name and for any `TestId.For` argument that is not a `TestIds` constant.

**2. The ids match the siblings exactly.** `cypress-standards` and
`playwright-standards-csharp` drive the same three pages, so a renamed id here
breaks a suite in another repository. That is the point: the id is a promise the
application makes, not a detail a suite invents.

**3. No literal escapes a named constant, markup included.** Two mechanisms,
because one cannot see both. A Roslyn analyzer covers C# at compile time, so a
stray literal fails the build. Razor compiles to generated code that analyzers
skip deliberately, so a scan test covers markup: no inline prose, no raw ids.
The analyzer caught its own test file twice while this was written, and the seed
data three times.

**4. Copy lives in constants.** `Copy.Tasks.Heading`, not text typed into a
component. Copy written inline cannot be found by someone looking for it,
changes in one place while its twin stays behind, and gives a test nothing to
assert but a literal it repeats. Swapping this for `.resx` later changes one
file and no component.

**5. Components receive data and raise events.** A page composes presentational
components; a component takes parameters and raises `EventCallback`, and reaches
no service. `TaskRow` knows how a task looks and nothing about where tasks live.

**6. Services sit behind interfaces defined at the consumer, and take a
cancellation token.** This reference holds state in memory and observes no
token, and it takes them anyway: a signature that omits one forces every caller
above it to drop cancellation too.

**7. Interactive Server, chosen deliberately.** The component tree lives on the
server and the browser holds a SignalR circuit. State and secrets stay on this
side of it, which is why the render mode and the secrets standard are one
decision. The cost is a connection to lose, which the framework's reconnect
modal handles. WebAssembly would push anything the client holds into public
view and force a separate API to keep secrets server-side.

**8. Service lifetime matches the circuit.** Scoped, so a visitor's task list
belongs to that visitor. A singleton would share one list across every browser
reaching the server.

**9. Accessibility is asserted, not assumed.** Every input has a label, the
counters announce through `aria-live`, invalid fields carry `aria-invalid` only
while invalid, header cells report `aria-sort` including `none`, and completed
tasks read as done through more than color. *Enforced by* automated axe checks
in the regression suite, which caught `aria-sort` written onto a button. Two
further defects fell to ordinary assertions in the same suite: a stylesheet rule
that painted an element marked `hidden`, and fields that reported themselves
valid to a screen reader while the page showed an error beside them.

**10. Rendered behavior is verified once, by the regression suite.** No
component-test framework. Playwright drives the real application through the
same `data-testid` contract a sibling suite uses, and xUnit covers the services,
validators, and view models that carry the rules.

**11. Coverage is not the bar; mutation is.** Stryker mutates the services and
models and the build fails below 70%. Coverage says a line ran. Mutation says a
test would have noticed if that line were wrong.

The gate runs on the Microsoft Testing Platform runner (`test-runner: mtp`),
which matters more than it looks. These suites build as executables on that
platform, and the .NET 10 SDK refuses to run them through VSTest at all. Stryker
defaults to VSTest, and that default fails quietly rather than loudly: it
registers every test name, kills almost nothing, and reports a score that reads
exactly like missing tests. Switching the runner moved the score from 50% to
about 90% without anyone's writing a test, which is the tell. A mutation score
falling for no clear reason deserves a look at whether the tests ran at all
before anyone writes tests to raise it.

Two notes on the configuration. `stryker-config.json` rejects any key its schema
does not define and refuses to start, comment keys included, so that reasoning
lives here instead. And `test-projects` narrows what Stryker reports on without
keeping the regression suite out of the run, so the mutation job still pays for
the browser specs.

**12. Warnings fail the build.** `TreatWarningsAsErrors`, `AnalysisMode=All`,
and `EnforceCodeStyleInBuild`. Every rule this repository turns off is listed in
`.editorconfig` with the reason it was turned off.

**13. The suite waits for a ready circuit, never for a clock.** Interactive
Server answers the first request with prerendered markup and replaces it once the
circuit connects. Text typed into that first copy never reaches the server and
disappears in the swap, so a spec that fills a field too early submits an empty
one and then fails on an assertion that names the wrong thing. `MainLayout`
renders a marker when `RendererInfo.IsInteractive` turns true, and every page
object navigates through the one helper that waits for it. A sleep would bury the
same race under a number nobody can defend.

**14. Validation reports through the framework's own channel.** The signup form
runs a hand-written validator, and it files every result into a
`ValidationMessageStore` on the form's `EditContext` rather than writing
`aria-invalid` onto the inputs. Blazor's input components own that attribute and
derive it from the context, so one set by hand disappears with no warning and no
error: the field then tells a screen reader it holds a valid value while the page
shows an error beside it, and the CSS that styles an invalid border never
matches. Feeding the context keeps the markup, the styling, and the screen reader
saying the same thing, and it costs less code than writing the attribute.

## Divergences, and why

**Routes drop the extension.** The sibling static sites serve `/signup.html`
because they ship files. Blazor routes rather than serving files, so this
application uses `/signup`. A suite written against one uses its own paths.

**The regression suite runs on xUnit, not NUnit.** `playwright-standards-csharp`
uses Playwright's NUnit flavor. One framework across this repository was worth
more than matching the sibling's skeleton exactly.

**No bUnit.** Component rendering is verified by the regression suite against
the real application rather than by a second framework against a simulated one.

**The layout keeps framework copy.** `Layout/MainLayout.razor` and the
reconnect modal carry text the template generates. The scan covers `Pages/` and
`Shared/`, the components this repository authors. Holding generated files to
the copy rule would mean editing what the tooling rewrites.

**`aria-sort` sits on the header cell here, and on the button in the siblings.**
ARIA permits the attribute on a `columnheader` and forbids it on a `button`, so
the sibling markup carries a defect their suites assert into place. Neither of
them runs an accessibility check, which is why it survived. The sort buttons keep
their ids and the header cells gained three of their own, so the shared id
contract gains entries and renames none.

## Layout

```
analyzers/LiteralAnalyzer   the literal rule, applied to every project
src/App/Constants           test ids, copy, routes, ARIA values
src/App/Models              the types components exchange
src/App/Services            the rules, and the code mutation testing covers
src/App/Components          pages, shared components, the TestId helper
tests/App.Tests             logic tests and the Razor markup scan
tests/Analyzer.Tests        the analyzer, driven through Roslyn
tests/Regression.Tests      Playwright against the running application
```

## Adopting this

Take the `TestId` helper, the constants discipline, and the two-mechanism
enforcement. Replace the three pages with your own, and `Constants/TestIds.cs`
with your own ids.

The piece worth keeping intact is the helper plus the scan test. A team that
writes `data-testid` by hand loses the single definition within a sprint, and
nothing tells them until a suite in another repository goes red.
