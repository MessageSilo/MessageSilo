using MessageSilo.Application.DTOs;
using MessageSilo.Application.Interfaces;
using MessageSilo.Application.Services;
using MessageSilo.Domain.Entities;
using MessageSilo.Domain.Enums;
using MessageSilo.Infrastructure.Interfaces;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Logging;

namespace MessageSilo.Infrastructure.Services
{
    public class EntityManagerGrain : Grain, IEntityManagerGrain
    {
        private readonly ILogger<EntityManagerGrain> logger;

        private IPersistentState<EntityManagerState> persistence { get; set; }

        private readonly IGrainFactory grainFactory;

        private readonly IYamlConverterService yamlConverterService;

        private readonly IConnectionValidator connectionValidator;

        private readonly IEnricherValidator enricherValidator;

        private readonly ITargetValidator targetValidator;

        public EntityManagerGrain(
            [PersistentState("EntityManagerState")] IPersistentState<EntityManagerState> state, ILogger<EntityManagerGrain> logger,
            IGrainFactory grainFactory, IYamlConverterService yamlConverterService,
            IConnectionValidator connectionValidator,
            IEnricherValidator enricherValidator,
            ITargetValidator targetValidator
            )
        {
            persistence = state;
            this.logger = logger;
            this.grainFactory = grainFactory;
            this.yamlConverterService = yamlConverterService;
            this.connectionValidator = connectionValidator;
            this.enricherValidator = enricherValidator;
            this.targetValidator = targetValidator;

        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var um = grainFactory.GetGrain<IUserManagerGrain>("um");
            await um.Upsert(this.GetPrimaryKeyString());

            await base.OnActivateAsync(cancellationToken);
        }

        public async Task<IEnumerable<Entity>> List()
        {
            return await Task.FromResult(persistence.State.Entities);
        }

        public async Task<List<EntityValidationErrors>> Vaidate(ApplyDTO dto)
        {
            var result = new List<EntityValidationErrors>();

            var entities = new List<Entity>().Concat(dto.Targets).Concat(dto.Enrichers).Concat(dto.Connections);

            var entitiesValidationErrors = await new EntitiesValidator(entities).ValidateAsync(entities);

            if (entitiesValidationErrors is not null && entitiesValidationErrors.Count() > 0)
                result.Add(new EntityValidationErrors("Global", entitiesValidationErrors));

            foreach (var target in dto.Targets)
            {
                var res = await targetValidator.ValidateAsync(target);

                if (res is not null && res.Count() > 0)
                    result.Add(new EntityValidationErrors(target.Name, res));
            }

            foreach (var enricher in dto.Enrichers)
            {
                var res = await enricherValidator.ValidateAsync(enricher);

                if (res is not null && res.Count() > 0)
                    result.Add(new EntityValidationErrors(enricher.Name, res));
            }

            foreach (var conn in dto.Connections)
            {
                var res = await connectionValidator.ValidateAsync(conn);

                if (res is not null && res.Count() > 0)
                    result.Add(new EntityValidationErrors(conn.Name, res));
            }

            return result;
        }

        public async Task Apply(ApplyDTO dto)
        {
            foreach (var target in dto.Targets)
            {
                persistence.State.Entities.Add(new Entity()
                {
                    UserId = target.UserId,
                    Name = target.Name,
                    Kind = target.Kind,
                    YamlDefinition = yamlConverterService.Serialize(target)
                });
            }

            foreach (var enricher in dto.Enrichers)
            {
                persistence.State.Entities.Add(new Entity()
                {
                    UserId = enricher.UserId,
                    Name = enricher.Name,
                    Kind = enricher.Kind,
                    YamlDefinition = yamlConverterService.Serialize(enricher)
                });
            }

            foreach (var conn in dto.Connections)
            {
                persistence.State.Entities.Add(new Entity()
                {
                    UserId = conn.UserId,
                    Name = conn.Name,
                    Kind = conn.Kind,
                    YamlDefinition = yamlConverterService.Serialize(conn)
                });
            }

            persistence.State.Scale = dto.Scale;
            await persistence.WriteStateAsync();

            foreach (var enricher in dto.Enrichers)
            {
                for (int scaleSet = 1; scaleSet <= persistence.State.Scale; scaleSet++)
                {
                    var grain = grainFactory.GetGrain<IEnricherGrain>($"{enricher.Id}#{scaleSet}");
                    await grain.Init(enricher);
                }
            }

            foreach (var conn in dto.Connections)
            {
                for (int scaleSet = 1; scaleSet <= persistence.State.Scale; scaleSet++)
                {
                    var grain = grainFactory.GetGrain<IConnectionGrain>($"{conn.Id}#{scaleSet}");
                    await grain.Init(true);
                }
            }
        }

        public async Task<ConnectionSettingsDTO> GetConnectionSettings(string name)
        {
            var result = persistence.State.Entities.FirstOrDefault(p => p.Name == name);

            if (result == null)
                return null;

            return yamlConverterService.Deserialize<ConnectionSettingsDTO>(result.YamlDefinition);
        }

        public async Task<TargetDTO> GetTargetSettings(string name)
        {
            var result = persistence.State.Entities.FirstOrDefault(p => p.Name == name);

            if (result == null)
                return null;

            return yamlConverterService.Deserialize<TargetDTO>(result.YamlDefinition);
        }

        public async Task<EnricherDTO> GetEnricherSettings(string name)
        {
            var result = persistence.State.Entities.FirstOrDefault(p => p.Name == name);

            if (result == null)
                return null;

            return yamlConverterService.Deserialize<EnricherDTO>(result.YamlDefinition);
        }

        public async Task Clear()
        {
            var connections = persistence.State.Entities.Where(p => p.Kind == EntityKind.Connection);

            foreach (var connection in connections)
            {
                for (int scaleSet = 1; scaleSet <= persistence.State.Scale; scaleSet++)
                {
                    var connGrain = grainFactory.GetGrain<IConnectionGrain>($"{connection.Id}#{scaleSet}");
                    await connGrain.Delete();
                }
            }

            persistence.State.Entities = new List<Entity>();

            await persistence.WriteStateAsync();
        }

        public async Task<int> GetScale()
        {
            return persistence.State.Scale;
        }
    }
}
