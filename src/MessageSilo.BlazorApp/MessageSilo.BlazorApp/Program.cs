using Blazorise;
using Blazorise.Icons.Material;
using Blazorise.Material;
using MessageSilo.BlazorApp;
using MessageSilo.BlazorApp.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient<IMessageSiloAPIService, MessageSiloAPIService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5001");
}).AddHttpMessageHandler(sp =>
{
    var handler = sp.GetService<AuthorizationMessageHandler>()!
    .ConfigureHandler(
         authorizedUrls: new[] { "https://localhost:5001" }
     );
    return handler;
});

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]);
});

AddBlazorise(builder.Services);

await builder.Build().RunAsync();


void AddBlazorise(IServiceCollection services)
{
    services
        .AddBlazorise();
    services
        .AddMaterialProviders()
        .AddMaterialIcons();

}
