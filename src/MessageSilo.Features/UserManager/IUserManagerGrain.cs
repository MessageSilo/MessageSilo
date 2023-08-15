using FluentValidation.Results;
using MessageSilo.Shared.Models;
using Orleans;

namespace MessageSilo.Features.UserManager
{
    public interface IUserManagerGrain : IGrainWithStringKey
    {
        Task<IEnumerable<string>> GetAll();

        Task Upsert(string userId);

        Task Delete(string userId);
    }
}
