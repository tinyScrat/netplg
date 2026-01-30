using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using BlazorApp.Features.Auth;
using BlazorApp.Application;
using BlazorApp.Infrastructure;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddHttpClient("API", (sp, client) =>
    {
        var logger = sp.GetRequiredService<ILogger<Program>>();

        var configuredBaseAddress = builder.Configuration.GetValue<string>("BaseAddress");

        var baseAddress =
            string.IsNullOrWhiteSpace(configuredBaseAddress)
                ? builder.HostEnvironment.BaseAddress
                : configuredBaseAddress;

        if (!Uri.TryCreate(baseAddress, UriKind.Absolute, out var uri))
        {
            logger.LogWarning(
                "Invalid BaseAddress '{BaseAddress}', falling back to '{Fallback}'",
                baseAddress,
                builder.HostEnvironment.BaseAddress);

            uri = new Uri(builder.HostEnvironment.BaseAddress);
        }

        client.BaseAddress = uri;
        
        logger.LogInformation("API HttpClient BaseAddress set to {BaseAddress}", client.BaseAddress);
    })
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services
    .AddScoped(sp =>
    {
        var factory = sp.GetRequiredService<IHttpClientFactory>();
        return factory.CreateClient("API");
    });

builder.Services
    .AddOidcAuthentication(options =>
    {
        builder.Configuration.Bind("EntraID", options.ProviderOptions);
        options.ProviderOptions.DefaultScopes.Add("email");
    })
    .AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, RemoteUserAccount, CustomPrincipalFactory>();

builder.Services
    .AddApplication()
    .AddInfrastructure();

builder.Services
    .AddUIAuthFeatures();

var app = builder.Build();

var env = app.Services.GetRequiredService<IWebAssemblyHostEnvironment>();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Environment: {ENV}", env.Environment);

app.Services.UseAuthFeatures();

await app.RunAsync();
