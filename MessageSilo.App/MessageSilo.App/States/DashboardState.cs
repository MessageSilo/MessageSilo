using Blazored.LocalStorage;
using FluentValidation.Results;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;

namespace MessageSilo.App.States
{
    public class DashboardState : IDashboardState
    {
        private readonly MessageSiloAPI messageSiloAPI;

        public Guid UserId { get; set; }

        public ConnectionState Queue { get; set; }

        public EnricherDTO Enricher { get; set; }

        public Message QueueOutput { get; set; } = new Message();

        public Message EnricherOutput { get; set; } = new Message();

        public DashboardState(MessageSiloAPI messageSiloAPI)
        {
            this.messageSiloAPI = messageSiloAPI;
        }

        public async Task Init()
        {
            var entities = await messageSiloAPI.GetEntities();

            if (entities.Data is not null)
            {
                if (entities.Data.Any(p => p.RowKey == "test_enricher"))
                    Enricher = (await messageSiloAPI.Get<EnricherDTO>("Enrichers", "test_enricher")).Data!;
                else
                {
                    Enricher = new EnricherDTO()
                    {
                        RowKey = "test_enricher",
                        Type = EnricherType.Inline,
                        Function = "(x) => { return { ...x, myNewProp: 'test' }; }"
                    };
                    await messageSiloAPI.Update<EnricherDTO, EnricherDTO>("Enrichers", Enricher);
                }

                if (entities.Data.Any(p => p.RowKey == "test_conn"))
                    Queue = (await messageSiloAPI.Get<ConnectionState>("Connections", "test_conn")).Data!;
                else
                    Queue = new ConnectionState()
                    {
                        ConnectionSettings = new ConnectionSettingsDTO()
                        {
                            RowKey = "test_conn",
                            Enrichers = new[] { "test_enricher" },
                            ReceiveMode = ReceiveMode.ReceiveAndDelete,
                            Type = MessagePlatformType.Azure_Queue
                        }
                    };
            }
        }

        public async Task<IEnumerable<ValidationFailure>> SaveQueueChanges(ConnectionSettingsDTO queue)
        {
            var response = await messageSiloAPI.Update<ConnectionSettingsDTO, ConnectionState>("Connections", queue);
            if (!response.Errors.Any())
                Queue.ConnectionSettings = queue.GetCopy();
            return response.Errors;
        }

        public async Task<IEnumerable<ValidationFailure>> SaveEnricherChanges(EnricherDTO enricher)
        {
            var response = await messageSiloAPI.Update<EnricherDTO, EnricherDTO>("Enrichers", enricher);
            if (!response.Errors.Any())
                Enricher = enricher.GetCopy();
            return response.Errors;
        }

        public async Task FillOutputs()
        {
            QueueOutput = (await messageSiloAPI.GetLastMessage("Connections", "test_conn")).Data?.Output ?? new Message();
            EnricherOutput = (await messageSiloAPI.GetLastMessage("Enrichers", "test_enricher")).Data?.Output ?? new Message();
        }
    }
}
