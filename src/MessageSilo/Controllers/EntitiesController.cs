using MessageSilo.Application.DTOs;
using MessageSilo.Domain.Entities;
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
        public async Task<IEnumerable<Entity>> Index()
        {
            var result = await entityManagerGrain.List();

            return result;
        }

        [HttpDelete()]
        public async Task Clear()
        {
            await entityManagerGrain.Clear();
        }

        [HttpPost()]
        public async Task<IEnumerable<EntityValidationErrors>?> Apply([FromBody] ApplyDTO dto)
        {
            foreach (var item in dto.Targets)
                item.UserId = loggedInUserId;

            foreach (var item in dto.Enrichers)
                item.UserId = loggedInUserId;

            foreach (var item in dto.Connections)
                item.UserId = loggedInUserId;

            var validationResults = await entityManagerGrain.Vaidate(dto);

            if (validationResults?.Count > 0)
                return validationResults;

            await Clear();

            await entityManagerGrain.Apply(dto);

            return null;
        }
    }
}
