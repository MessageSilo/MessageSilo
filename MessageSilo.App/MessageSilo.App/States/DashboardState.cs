using Blazored.LocalStorage;
using FluentValidation.Results;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;

namespace MessageSilo.App.States
{
    public class DashboardState : IDashboardState
    {
        private readonly ILocalStorageService localStorage;
        private readonly MessageSiloAPI messageSiloAPI;

        public Guid UserId { get; private set; }

        public ConnectionState Queue { get; private set; } = new ConnectionState()
        {
            ConnectionSettings = new ConnectionSettingsDTO()
            {
                RowKey = "test_conn",
                Enrichers = new[] { "test_enricher" },
                ReceiveMode = ReceiveMode.ReceiveAndDelete,
                Type = MessagePlatformType.Azure_Queue
            }
        };

        public EnricherDTO Enricher { get; private set; } = new EnricherDTO()
        {
            RowKey = "test_enricher",
            Type = EnricherType.Inline,
            Function = "(x) => { return { ...x, myNewProp: 'test' }; }"
        };

        public Message Output { get; private set; } = new Message();

        public DashboardState(ILocalStorageService localStorage, MessageSiloAPI messageSiloAPI)
        {
            this.localStorage = localStorage;
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
                    await messageSiloAPI.Update<EnricherDTO, EnricherDTO>("Enrichers", Enricher);

                if (entities.Data.Any(p => p.RowKey == "test_conn"))
                    Queue = (await messageSiloAPI.Get<ConnectionState>("Connections", "test_conn")).Data!;
            }
        }

        public async Task<IEnumerable<ValidationFailure>> SaveQueueChanges(ConnectionSettingsDTO queue)
        {
            Queue.ConnectionSettings = queue.GetCopy();
            var response = await messageSiloAPI.Update<ConnectionSettingsDTO, ConnectionState>("Connections", Queue.ConnectionSettings);
            return response.Errors;
        }
    }
}
