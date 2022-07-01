using Microsoft.AspNetCore.Builder;
using Microsoft.Net.Http.Headers;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using SBMonitor.API.Hubs;
using System.Net;

var builder = Host.CreateDefaultBuilder(args);

builder.UseOrleans((context, sb) =>
     {
         sb.ConfigureApplicationParts(manager =>
         {
             manager.AddApplicationPart(typeof(Program).Assembly).WithReferences();
         });

         //sb.UseDashboard();
         sb.ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Information).AddConsole());

         sb.UseSignalR(b =>
             b.Configure((innerSiloBuilder, config) =>
             {
                 innerSiloBuilder.UseLocalhostClustering();

                 innerSiloBuilder.AddSimpleMessageStreamProvider("SMS");
                 innerSiloBuilder.AddAzureTableGrainStorageAsDefault(
                     configureOptions: options =>
                     {
                         options.TableName = "MessageSiloState";
                         options.UseJson = true;
                         options.ConfigureTableServiceClient(context.Configuration["ConnectionStrings:MessageSiloState"]);
                     });
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
