namespace MessageSilo.Shared.Models
{
    [GenerateSerializer]
    public class RetrySettings
    {
        [Id(0)]
        public int MaxRetryAttempts { get; set; } = 1;
        public RetrySettings() { }
    }
}
