using FluentValidation.Results;

namespace MessageSilo.Shared.Models
{
    [GenerateSerializer]
    public class EntityValidationErrors
    {
        [Id(0)]
        public required string EntityName { get; set; }

        [Id(1)]
        public List<ValidationFailure> ValidationFailures { get; set; } = new List<ValidationFailure>();
    }
}
