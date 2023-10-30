namespace MessageSilo.Shared.Models
{
    public interface IEntityGrain<DTO, S> : IGrainWithStringKey where DTO : Entity where S : class
    {
        Task Update(DTO state);

        Task Delete();

        Task<S> GetState();

        Task<LastMessage> GetLastMessage();
    }
}
