using Amazon.Auth.AccessControlPolicy;
using MessageSilo.Features.Connection;
using MessageSilo.Features.EntityManager;
using MessageSilo.Features.UserManager;
using MessageSilo.Shared.Enums;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Configuration;
using System.Net;

namespace MessageSilo
{
    public class StartupService : IHostedService
    {
        public IGrainFactory GrainFactory { get; }

        protected readonly ILogger<StartupService> logger;

        public StartupService(IGrainFactory grainFactory, ILogger<StartupService> logger)
        {
            GrainFactory = grainFactory;
            this.logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //Init connections
            var um = GrainFactory.GetGrain<IUserManagerGrain>("um");
            var users = await um.GetAll();

            foreach (var user in users)
            {
                var em = GrainFactory.GetGrain<IEntityManagerGrain>(user);
                var connections = (await em.GetAll()).Where(p => p.Kind == EntityKind.Connection);

                foreach (var entity in connections)
                {
                    var conn = GrainFactory.GetGrain<IConnectionGrain>(entity.Id);
                    await conn.GetState();
                    logger.LogInformation($"Connection ({entity.Id}) initialized.");
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
