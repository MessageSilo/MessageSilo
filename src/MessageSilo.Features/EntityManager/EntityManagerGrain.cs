using FluentValidation.Results;
using MessageSilo.Shared.DataAccess;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using MessageSilo.Shared.Validators;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using System.Text;

namespace MessageSilo.Features.EntityManager
{
    public class EntityManagerGrain : Grain, IEntityManagerGrain
    {
        private readonly ILogger<EntityManagerGrain> logger;

        private readonly IEntityRepository repo;

        private IPersistentState<EntityManagerState> persistence { get; set; }

        private List<Entity> entities { get; set; } = new List<Entity>();

        public EntityManagerGrain([PersistentState("EntityManagerState")] IPersistentState<EntityManagerState> state, IEntityRepository repo, ILogger<EntityManagerGrain> logger)
        {
            this.repo = repo;
            this.logger = logger;
            this.persistence = state;
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

        public async Task IncreaseUsedThroughput(string messageBody)
        {
            var amount = Encoding.UTF8.GetByteCount(messageBody ?? string.Empty) * 0.001;
            var currentDate = DateTime.UtcNow.Year * 100 + DateTime.UtcNow.Month;

            if (!persistence.State.Throughput.ContainsKey(currentDate))
                persistence.State.Throughput.Add(currentDate, amount);
            else
                persistence.State.Throughput[DateTime.UtcNow.Year] += amount;

            await persistence.WriteStateAsync();
        }

        public async Task<double> GetUsedThroughput(int date)
        {
            var output = persistence.State.Throughput[date];
            return await Task.FromResult(output);
        }
    }
}
