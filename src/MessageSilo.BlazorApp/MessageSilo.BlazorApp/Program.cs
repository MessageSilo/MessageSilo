using Blazorise;
using Blazorise.Icons.Material;
using Blazorise.Material;
using MessageSilo.BlazorApp;
using MessageSilo.BlazorApp.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient<IMessageSiloAPIService, MessageSiloAPIService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7132");
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
