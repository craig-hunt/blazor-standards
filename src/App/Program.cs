using System.Diagnostics.CodeAnalysis;
using BlazorStandards.App;
using BlazorStandards.App.Components;
using BlazorStandards.App.Services;

var builder = WebApplication.CreateBuilder(args);

// User secrets load in every environment, not only Development. A deployed run
// finds nothing there and reads environment variables instead, which is the
// point: one key name resolves from whichever store the target provides.
builder.Configuration.AddUserSecrets<Program>(optional: true);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Scoped means one instance per circuit under Interactive Server, so a
// visitor's task list belongs to that visitor. A singleton would share one list
// across every browser reaching this server.
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(ErrorRoutes.Error, createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute(ErrorRoutes.NotFound, createScopeForStatusCodePages: true);

// Antiforgery runs for every form post. Blazor's form handling depends on it,
// and turning it off to make a form work is the wrong fix for the right symptom.
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync().ConfigureAwait(false);

/// <summary>
/// Named so the regression suite can host this application in process. The
/// router also reaches it through typeof(Program).Assembly.
/// </summary>
/// <remarks>
/// This is the one type in the application that stays public. A test host takes
/// the entry point as a generic argument, and InternalsVisibleTo grants access
/// without granting accessibility, so an internal type cannot serve.
/// </remarks>
[SuppressMessage(
    "Design",
    "CA1515:Consider making public types internal",
    Justification = "A test host names this type as a generic argument, which requires it to be public.")]
public partial class Program
{
}
