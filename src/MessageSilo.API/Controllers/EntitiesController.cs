using MessageSilo.Features.EntityManager;
using MessageSilo.Shared.DataAccess;
using MessageSilo.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace MessageSilo.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class EntitiesController : MessageSiloControllerBase
    {
        protected readonly IEntityRepository repo;

        public EntitiesController(
            ILogger<MessageSiloControllerBase> logger,
            IHttpContextAccessor httpContextAccessor,
            IEntityRepository repo,
            IClusterClient client) : base(logger, httpContextAccessor, client)
        {
            this.repo = repo;
        }

        [HttpGet()]
        public async Task<ApiContract<IEnumerable<Entity>>> Index()
        {
            var result = await entityManagerGrain.GetAll();

            return new ApiContract<IEnumerable<Entity>>(httpContextAccessor, StatusCodes.Status200OK, data: result);
        }
    }
}
