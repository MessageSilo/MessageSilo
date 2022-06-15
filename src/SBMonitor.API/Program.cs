using Microsoft.AspNetCore.Builder;
using Microsoft.Net.Http.Headers;
using Orleans;
using Orleans.Hosting;
using SBMonitor.API.Hubs;

var builder = Host.CreateDefaultBuilder(args);

builder.UseOrleans(builder =>
     {
         builder.ConfigureApplicationParts(manager =>
         {
             manager.AddApplicationPart(typeof(Program).Assembly).WithReferences();
         });

         builder.UseDashboard();
         builder.ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Debug).AddConsole());

         builder.UseSignalR(b =>
             b.Configure((innerSiloBuilder, config) =>
             {
                 innerSiloBuilder.UseLocalhostClustering();
                 innerSiloBuilder.AddMemoryGrainStorageAsDefault();
                 innerSiloBuilder.AddSimpleMessageStreamProvider("SMS");
                 innerSiloBuilder.AddMemoryGrainStorage("PubSubStore");
             })
             ).RegisterHub<MessageMonitor>();
     });

builder.ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.ConfigureServices(services =>
    {
        services.AddControllers();
    });
    webBuilder.Configure(app =>
    {
        app.UseCors(builder => builder.SetIsOriginAllowed(isOriginAllowed: _ => true).WithExposedHeaders(HeaderNames.ContentDisposition).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapHub<MessageMonitor>("/MessageMonitor");
        });
    });
});

builder.ConfigureServices(services =>
{
    services.Configure<ConsoleLifetimeOptions>(options =>
    {
        options.SuppressStatusMessages = true;
    });

    services.AddSignalR().AddOrleans();
});

await builder.RunConsoleAsync();
