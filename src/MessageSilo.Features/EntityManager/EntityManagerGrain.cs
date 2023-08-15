﻿using FluentValidation.Results;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using MessageSilo.Shared.Validators;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace MessageSilo.Features.EntityManager
{
    public class EntityManagerGrain : Grain, IEntityManagerGrain
    {
        private readonly ILogger<EntityManagerGrain> logger;

        private IPersistentState<EntityManagerState> persistence { get; set; }

        public EntityManagerGrain([PersistentState("EntityManagerState")] IPersistentState<EntityManagerState> state, ILogger<EntityManagerGrain> logger)
        {
            this.persistence = state;
            this.logger = logger;
        }

        public async Task<IEnumerable<Entity>> GetAll()
        {
            return await Task.FromResult(persistence.State.Entities);
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

            if (persistence.State.Entities.Count() + 1 > 5)
                validationErrors.Add(new ValidationFailure("subscription", "Count of max. allowed entities reached! ==WIP: Only in BETA and FREE tier=="));

            if (validationErrors.Any())
                return await Task.FromResult(validationErrors);

            if (!persistence.State.Entities.Any(p => p.Id == entity.Id))
            {
                persistence.State.Entities.Add(entity);
                await persistence.WriteStateAsync();
            }

            return await Task.FromResult<List<ValidationFailure>?>(null);
        }

        public async Task<List<ValidationFailure>?> Delete(string entityName)
        {
            var validationErrors = new List<ValidationFailure>();

            //TODO: Delete validations

            if (validationErrors.Any())
                return await Task.FromResult(validationErrors);

            persistence.State.Entities.RemoveAll(p => p.RowKey == entityName);

            await persistence.WriteStateAsync();

            return await Task.FromResult<List<ValidationFailure>?>(null);
        }
    }
}
