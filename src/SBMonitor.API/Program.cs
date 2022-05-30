using Microsoft.Net.Http.Headers;
using Orleans;
using Orleans.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.UseOrleans(builder =>
     {
         builder.ConfigureApplicationParts(manager =>
         {
             manager.AddApplicationPart(typeof(Program).Assembly).WithReferences();
         });
         builder.UseLocalhostClustering();
         builder.AddMemoryGrainStorageAsDefault();
         builder.AddSimpleMessageStreamProvider("SMS");
         builder.AddMemoryGrainStorage("PubSubStore");
         builder.UseDashboard();
         builder.ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Debug).AddConsole());
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
        });
    });
});

builder.ConfigureServices(services =>
{
    services.Configure<ConsoleLifetimeOptions>(options =>
    {
        options.SuppressStatusMessages = true;
    });
});

await builder.RunConsoleAsync();
