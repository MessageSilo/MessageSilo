namespace MessageSilo.Domain.Entities
{
    public readonly record struct RetrySettings(int MaxRetryAttempts);

    public readonly record struct ValidationFailure(string ErrorMessage);

    public readonly record struct EntityValidationErrors(string EntityName, IEnumerable<ValidationFailure> ValidationFailures);
}
