namespace MessageSilo.Infrastructure.Interfaces
{
    public interface IUserManagerGrain : IGrainWithStringKey
    {
        Task<IEnumerable<string>> GetAll();

        Task Upsert(string userId);

        Task Delete(string userId);
    }
}
