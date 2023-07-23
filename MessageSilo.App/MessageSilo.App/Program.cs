using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using MessageSilo.App;
using MessageSilo.App.States;
using MessageSilo.Features.MessageSiloApi;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<IMessageSiloAPI, MessageSiloAPI>();
builder.Services.AddScoped<IDashboardState, DashboardState>();

builder.Services.AddBlazoredLocalStorage();

AddBlazorise(builder.Services);

await builder.Build().RunAsync();


void AddBlazorise(IServiceCollection services)
{
    services
        .AddBlazorise();
    services
        .AddBootstrap5Providers()
        .AddFontAwesomeIcons();

}
