using MessageSilo.Application.Interfaces;
using MessageSilo.Application.Services;
using MessageSilo.Domain.Entities;
using MessageSilo.Infrastructure.Interfaces;
using MessageSilo.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Serialization;

namespace MessageSilo.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IYamlConverterService, YamlConverterService>();

            services.AddScoped<IConnectionValidator, ConnectionValidator>();
            services.AddScoped<IEnricherValidator, EnricherValidator>();
            services.AddScoped<ITargetValidator, TargetValidator>();

            services.AddSerializer(serializerBuilder =>
            {
                serializerBuilder.AddNewtonsoftJsonSerializer(
                    isSupported: type => typeof(Entity).Namespace.StartsWith("MessageSilo"));
            });

            return services;
        }
    }
}
