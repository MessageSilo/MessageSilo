using MessageSilo.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace MessageSilo.Controllers
{
    [Route("api/v1/[controller]")]
    public class EntitiesController : MessageSiloControllerBase
    {
        public EntitiesController(
            ILogger<MessageSiloControllerBase> logger,
            IHttpContextAccessor httpContextAccessor,
            IClusterClient client) : base(logger, httpContextAccessor, client)
        {
        }

        [HttpGet()]
        public async Task<ApiContract<IEnumerable<Entity>>> Index()
        {
            var result = await entityManagerGrain.GetAll();

            return new ApiContract<IEnumerable<Entity>>(httpContextAccessor, StatusCodes.Status200OK, data: result);
        }
    }
}
