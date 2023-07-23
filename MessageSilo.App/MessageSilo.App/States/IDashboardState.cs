using MessageSilo.Shared.Models;

namespace MessageSilo.App.States
{
    public interface IDashboardState
    {
        ConnectionSettingsDTO Queue { get; set; }

        EnricherDTO Enricher { get; set; }

        Message Output { get; set; }

        Task Init();
    }
}
