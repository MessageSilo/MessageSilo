﻿using MessageSilo.Domain.Enums;
using MessageSilo.Infrastructure.Interfaces;

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
                var connections = (await em.List()).Where(p => p.Kind == EntityKind.Connection);
                var scale = await em.GetScale();

                foreach (var connection in connections)
                {
                    for (var scaleSet = 1; scaleSet <= scale; scaleSet++)
                    {
                        var grain = GrainFactory.GetGrain<IConnectionGrain>($"{connection.Id}#{scaleSet}");
                        await grain.Init(true);
                    }
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
        }
    }
}
