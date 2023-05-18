using Esprima.Ast;
using FluentValidation;
using FluentValidation.Results;
using MessageSilo.Shared.DataAccess;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using MessageSilo.Shared.Validators;
using Microsoft.Extensions.Logging;
using Orleans;

namespace MessageSilo.Features.EntityManager
{
    public class EntityManagerGrain : Grain, IEntityManagerGrain
    {
        private readonly ILogger<EntityManagerGrain> logger;

        private readonly IEntityRepository repo;

        private readonly IGrainFactory grainFactory;

        private List<Entity> entities { get; set; } = new List<Entity>();

        public EntityManagerGrain(IEntityRepository repo, IGrainFactory grainFactory, ILogger<EntityManagerGrain> logger)
        {
            this.grainFactory = grainFactory;
            this.repo = repo;
            this.logger = logger;
        }

        public override async Task OnActivateAsync()
        {
            entities = (await repo.Query(userId: this.GetPrimaryKeyString())).ToList();

            await base.OnActivateAsync();
        }

        public async Task<IEnumerable<Entity>> GetAll()
        {
            return await Task.FromResult(entities);
        }

        public async Task<List<ValidationFailure>?> Upsert(Entity entity)
        {
            var validationErrors = new List<ValidationFailure>();

            switch (entity.Kind)
            {
                case EntityKind.Connection:
                    {
                        var res = await new ConnectionValidator(entities).ValidateAsync(entity as ConnectionSettingsDTO);
                        validationErrors = res.Errors;
                    }
                    break;
                case EntityKind.Target:
                    {
                        var res = await new TargetValidator(entities).ValidateAsync(entity as TargetDTO);
                        validationErrors = res.Errors;
                    }
                    break;
                case EntityKind.Enricher:
                    {
                        var res = await new EnricherValidator(entities).ValidateAsync(entity as EnricherDTO);
                        validationErrors = res.Errors;
                    }
                    break;
            }

            if (entities.Count() + 1 > 5)
                validationErrors.Add(new ValidationFailure("subscription", "Count of max. allowed entities reached! ==WIP: Only in BETA and FREE tier=="));

            if (validationErrors.Any())
                return await Task.FromResult(validationErrors);

            if (!entities.Any(p => p.Id == entity.Id))
                entities.Add(entity);

            return await Task.FromResult<List<ValidationFailure>?>(null);
        }

        public async Task<List<ValidationFailure>?> Delete(string entityName)
        {
            var validationErrors = new List<ValidationFailure>();

            //TODO: Delete validations

            if (validationErrors.Any())
                return await Task.FromResult(validationErrors);

            entities.RemoveAll(p => p.RowKey == entityName);

            return await Task.FromResult<List<ValidationFailure>?>(null);
        }
    }
}
