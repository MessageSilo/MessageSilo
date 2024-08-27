namespace MessageSilo.Domain.Interfaces
{
    public interface IAIService
    {
        Task<string> Chat(string command, string message);
    }
}
