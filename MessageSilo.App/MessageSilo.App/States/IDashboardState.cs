using FluentValidation.Results;
using MessageSilo.Shared.Models;

namespace MessageSilo.App.States
{
    public interface IDashboardState
    {
        ConnectionState Queue { get; }

        EnricherDTO Enricher { get; }

        Message Output { get; }

        Task Init();

        Task<IEnumerable<ValidationFailure>> SaveQueueChanges(ConnectionSettingsDTO queue);
    }
}
