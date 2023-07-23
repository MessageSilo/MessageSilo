using Blazored.LocalStorage;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;

namespace MessageSilo.App.States
{
    public class DashboardState : IDashboardState
    {
        private readonly ILocalStorageService localStorage;

        public Guid UserId { get; set; }

        public ConnectionSettingsDTO Queue { get; set; } = new ConnectionSettingsDTO()
        {
            RowKey = "test_conn",
            Enrichers = new[] { "test_enricher" },
            ReceiveMode = ReceiveMode.ReceiveAndDelete
        };

        public EnricherDTO Enricher { get; set; } = new EnricherDTO()
        {
            RowKey = "test_enricher"
        };

        public Message Output { get; set; } = new Message();

        public DashboardState(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }

        public async Task Init()
        {
            if (!(await localStorage.ContainKeyAsync("userId")))
                await localStorage.SetItemAsync<Guid>("userId", Guid.NewGuid());

            UserId = await localStorage.GetItemAsync<Guid>("userId");
        }
    }
}
