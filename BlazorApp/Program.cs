using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using BlazorApp.Features.Auth;
using BlazorApp.Application;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddHttpClient("API", client =>
    {
        var baseAddress = builder.Configuration.GetValue<string>("BaseAddress") ??
            builder.HostEnvironment.BaseAddress;
        client.BaseAddress = new Uri(baseAddress);
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
    .AddApplication();

var app = builder.Build();

var env = app.Services.GetRequiredService<IWebAssemblyHostEnvironment>();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("Environment: {ENV}", env.Environment);

await app.RunAsync();
