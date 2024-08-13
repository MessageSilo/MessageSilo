using FluentValidation;
using FluentValidation.Results;
using MessageSilo.Domain.Entities;
using MessageSilo.Domain.Enums;
using MessageSilo.Features.Connection;
using MessageSilo.Features.Enricher;
using MessageSilo.Features.Target;
using MessageSilo.Features.UserManager;
using MessageSilo.Infrastructure.Interfaces;
using MessageSilo.Shared.Models;
using MessageSilo.Shared.Validators;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;

namespace MessageSilo.Features.EntityManager
{
    public class EntityManagerGrain : Grain, IEntityManagerGrain
    {
        private readonly ILogger<EntityManagerGrain> logger;

        private IPersistentState<EntityManagerState> persistence { get; set; }

        private readonly IGrainFactory grainFactory;

        private readonly IYamlConverterService yamlConverterService;

        public EntityManagerGrain([PersistentState("EntityManagerState")] IPersistentState<EntityManagerState> state, ILogger<EntityManagerGrain> logger, IGrainFactory grainFactory, IYamlConverterService yamlConverterService)
        {
            persistence = state;
            this.logger = logger;
            this.grainFactory = grainFactory;
            this.yamlConverterService = yamlConverterService;
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

            var targetValidator = new TargetValidator(entities);
            var enricherValidator = new EnricherValidator(entities);
            var connectionValidator = new ConnectionValidator(entities);

            foreach (var target in dto.Targets)
            {
                var res = await targetValidator.ValidateAsync(target);
                if (res.Errors.Count > 0)
                    result.Add(new EntityValidationErrors()
                    {
                        EntityName = target.Name,
                        ValidationFailures = res.Errors
                    });
            }

            foreach (var enricher in dto.Enrichers)
            {
                var res = await enricherValidator.ValidateAsync(enricher);
                if (res.Errors.Count > 0)
                    result.Add(new EntityValidationErrors()
                    {
                        EntityName = enricher.Name,
                        ValidationFailures = res.Errors
                    });
            }

            foreach (var conn in dto.Connections)
            {
                var res = await connectionValidator.ValidateAsync(conn);
                if (res.Errors.Count > 0)
                    result.Add(new EntityValidationErrors()
                    {
                        EntityName = conn.Name,
                        ValidationFailures = res.Errors
                    });
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

            foreach (var target in dto.Targets)
            {
                for (int scaleSet = 1; scaleSet <= persistence.State.Scale; scaleSet++)
                {
                    var grain = grainFactory.GetGrain<ITargetGrain>($"{target.Id}#{scaleSet}");
                    await grain.Init(target);
                }
            }

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

        public async Task<List<ValidationFailure>?> Upsert(Entity entity)
        {
            var validationErrors = new List<ValidationFailure>();

            switch (entity.Kind)
            {
                case EntityKind.Connection:
                    {
                        var res = await new ConnectionValidator(persistence.State.Entities).ValidateAsync(entity as ConnectionSettingsDTO);
                        validationErrors = res.Errors;
                    }
                    break;
                case EntityKind.Target:
                    {
                        var res = await new TargetValidator(persistence.State.Entities).ValidateAsync(entity as TargetDTO);
                        validationErrors = res.Errors;
                    }
                    break;
                case EntityKind.Enricher:
                    {
                        var res = await new EnricherValidator(persistence.State.Entities).ValidateAsync(entity as EnricherDTO);
                        validationErrors = res.Errors;
                    }
                    break;
            }

            if (validationErrors.Any())
                return validationErrors;

            if (!persistence.State.Entities.Any(p => p.Id == entity.Id))
            {
                persistence.State.Entities.Add(new Entity()
                {
                    UserId = entity.UserId,
                    Name = entity.Name,
                    Kind = entity.Kind,
                    YamlDefinition = yamlConverterService.Serialize(entity)
                });
                await persistence.WriteStateAsync();
            }

            return null;
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
