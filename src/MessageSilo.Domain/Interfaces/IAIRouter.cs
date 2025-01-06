namespace MessageSilo.Domain.Interfaces
{
    public interface IAIRouter
    {
        Task<IEnumerable<string>> GetTargetNames(string message);
    }
}
