using FluentValidation.Results;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using MessageSilo.Shared.Validators;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;

namespace MessageSilo.Features.UserManager
{
    public class UserManagerGrain : Grain, IUserManagerGrain
    {
        private readonly ILogger<UserManagerGrain> logger;

        private IPersistentState<UserManagerState> persistence { get; set; }

        public UserManagerGrain([PersistentState("UserManagerState")] IPersistentState<UserManagerState> state, ILogger<UserManagerGrain> logger)
        {
            this.persistence = state;
            this.logger = logger;
        }

        public async Task<IEnumerable<string>> GetAll()
        {
            return await Task.FromResult(persistence.State.Users);
        }

        public async Task Upsert(string userId)
        {
            if (persistence.State.Users.Contains(userId))
                return;

            persistence.State.Users.Add(userId);
            await persistence.WriteStateAsync();
        }

        public async Task Delete(string userId)
        {
            persistence.State.Users.Remove(userId);
            await persistence.WriteStateAsync();
        }
    }
}
