using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using MessageSilo.App;
using MessageSilo.App.States;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Net.Http.Headers;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<MessageSiloAPI>();
builder.Services.AddScoped<IDashboardState, DashboardState>();

builder.Services.AddBlazoredLocalStorage();

AddBlazorise(builder.Services);

var localStorage = builder.Services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<ILocalStorageService>();

if (!(await localStorage.ContainKeyAsync("userId")))
    await localStorage.SetItemAsync("userId", Guid.NewGuid());

var userId = await localStorage.GetItemAsync<Guid>("userId");

builder.Services.AddScoped(p =>
{
    var client = new RestClient(new RestClientOptions("https://localhost:5000/api/v1")
    {
        CalculateResponseStatus = httpResponse =>
        httpResponse.IsSuccessStatusCode ||
        httpResponse.StatusCode == HttpStatusCode.NotFound ||
        httpResponse.StatusCode == HttpStatusCode.BadRequest
        ? ResponseStatus.Completed : ResponseStatus.Error
    });

    client.AddDefaultHeader(HeaderNames.Authorization, userId.ToString());

    return client;
});

var state = builder.Services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<IDashboardState>();
await state.Init();

await builder.Build().RunAsync();

void AddBlazorise(IServiceCollection services)
{
    services
        .AddBlazorise();
    services
        .AddBootstrap5Providers()
        .AddFontAwesomeIcons();

}
