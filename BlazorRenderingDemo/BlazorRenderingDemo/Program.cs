using BlazorRenderingDemo.Client.Pages;
using BlazorRenderingDemo.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// DI registration: makes the app *capable* of Server (circuits) and WebAssembly render modes, which InteractiveAuto requires both of.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
// Endpoint configuration: actually exposes those modes (the /_blazor hub, the WASM _framework files) and discovers routable pages in the Client project.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorRenderingDemo.Client._Imports).Assembly);

app.Run();
