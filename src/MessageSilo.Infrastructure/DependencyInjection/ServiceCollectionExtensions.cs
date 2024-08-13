using MessageSilo.Infrastructure.Interfaces;
using MessageSilo.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageSilo.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IYamlConverterService, YamlConverterService>();

            // Register other infrastructure services
            //services.AddTransient<IEmailService, EmailService>();
            //services.AddTransient<ILoggingService, LoggingService>();

            // Optional: Register Orleans-related services if using Orleans
            //services.AddOrleansSilo(configuration);

            return services;
        }
    }
}
