using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using MyApp.Application;
using MyApp.Infrastructure;
using MyApp.WebUI;
using MyApp.WebUI.Services;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddRadzenComponents();

builder.Services
    .AddApiHttpClientWithAuth(Consts.ApiHttpClientName, builder.Configuration, builder.HostEnvironment.BaseAddress);

builder.Services
    .AddAuthorizationCore(options => options.AddPermissionPolicies())
    .AddOidcAuthentication(options =>
    {
        builder.Configuration.Bind("EntraID", options.ProviderOptions);
        options.ProviderOptions.DefaultScopes.Add("email");
        options.ProviderOptions.RedirectUri = $"{builder.HostEnvironment.BaseAddress}authentication/login-callback";
    })
    .AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, RemoteUserAccount, CustomPrincipalFactory>();

builder.Services
    .AddWebUIFeatures()
    .AddApplication()
    .AddInfrastructure(Consts.ApiHttpClientName);

var app = builder.Build();

var env = app.Services.GetRequiredService<IWebAssemblyHostEnvironment>();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Environment: {ENV}", env.Environment);

app.Services.UseApplicationFeatures();
app.Services.UseWebUIFeatures();

await app.RunAsync();
