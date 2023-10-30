using FluentValidation.Results;
using MessageSilo.Features.UserManager;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using MessageSilo.Shared.Validators;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Runtime;
using System.Text;

namespace MessageSilo.Features.EntityManager
{
    public class EntityManagerGrain : Grain, IEntityManagerGrain
    {
        private readonly ILogger<EntityManagerGrain> logger;

        private IPersistentState<EntityManagerState> persistence { get; set; }

        private readonly IGrainFactory grainFactory;

        public EntityManagerGrain([PersistentState("EntityManagerState")] IPersistentState<EntityManagerState> state, ILogger<EntityManagerGrain> logger, IGrainFactory grainFactory)
        {
            persistence = state;
            this.logger = logger;
            this.grainFactory = grainFactory;
        }

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var um = grainFactory.GetGrain<IUserManagerGrain>("um");
            await um.Upsert(this.GetPrimaryKeyString());

            await base.OnActivateAsync(cancellationToken);
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

            if (validationErrors.Any())
                return await Task.FromResult(validationErrors);

            if (!persistence.State.Entities.Any(p => p.Id == entity.Id))
            {
                persistence.State.Entities.Add(new Entity()
                {
                    PartitionKey = entity.PartitionKey,
                    RowKey = entity.RowKey,
                    Kind = entity.Kind
                });
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
