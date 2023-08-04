using FluentValidation.Results;
using MessageSilo.Shared.Models;

namespace MessageSilo.App.States
{
    public interface IDashboardState
    {
        ConnectionState Queue { get; set; }

        EnricherDTO Enricher { get; set; }

        Message QueueOutput { get; set; }

        Message EnricherOutput { get; set; }

        Task Init();

        Task<IEnumerable<ValidationFailure>> SaveQueueChanges(ConnectionSettingsDTO queue);

        Task<IEnumerable<ValidationFailure>> SaveEnricherChanges(EnricherDTO enricher);
    }
}
