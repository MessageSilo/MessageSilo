namespace MessageSilo.Domain.Entities
{
    public record RetrySettings(int MaxRetryAttempts = 1);

    [Serializable]
    public class ValidationFailure
    {
        public string ErrorMessage { get; set; }

        public ValidationFailure(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }

    public record EntityValidationErrors(string EntityName, IEnumerable<ValidationFailure> ValidationFailures);
}
