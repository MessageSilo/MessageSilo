namespace MessageSilo.Domain.Entities
{
    public readonly record struct RetrySettings(int MaxRetryAttempts);
}
