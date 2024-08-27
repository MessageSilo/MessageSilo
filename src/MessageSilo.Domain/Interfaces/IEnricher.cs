namespace MessageSilo.Domain.Interfaces
{
    public interface IEnricher
    {
        Task<string> TransformMessage(string message);
    }
}
