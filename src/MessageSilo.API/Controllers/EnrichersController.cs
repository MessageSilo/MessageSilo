using MessageSilo.Features.Enricher;
using MessageSilo.Shared.DataAccess;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace MessageSilo.API.Controllers
{
    [Route("api/v1")]
    public class EnrichersController : MessageSiloControllerBase
    {
        public EnrichersController(ILogger<ConnectionsController> logger, IClusterClient client, IHttpContextAccessor httpContextAccessor, IEntityRepository repo)
            : base(logger, httpContextAccessor, client, repo)
        {
        }

        [HttpGet("Enrichers")]
        public async Task<IEnumerable<EnricherDTO>> Index()
        {
            var result = new List<EnricherDTO>();

            var enrichers = await repo.Query(EntityKind.Enricher, loggedInUserId);

            foreach (var t in enrichers)
            {
                var enricher = client!.GetGrain<IEnricherGrain>(t.Id);
                result.Add(await enricher.GetState());
            }

            return result;
        }

        [HttpGet("Enrichers/{name}")]
        public async Task<EnricherDTO> Show(string name)
        {
            var id = $"{loggedInUserId}|{name}";
            var enrichers = await repo.Query(EntityKind.Enricher, loggedInUserId);

            if (!enrichers.Any(p => p.Id == id))
            {
                httpContextAccessor!.HttpContext!.Response.StatusCode = StatusCodes.Status404NotFound;
                return null!;
            }

            var enricher = client!.GetGrain<IEnricherGrain>(id);

            return await enricher.GetState();
        }

        [HttpPut("Enrichers/{name}")]
        public async Task<EnricherDTO> Update(string name, [FromBody] EnricherDTO dto)
        {
            dto.PartitionKey = loggedInUserId;

            await repo.Add(new[]
            {
                new Entity()
                {
                    PartitionKey = loggedInUserId,
                    RowKey = name,
                    Kind = EntityKind.Enricher
                }
            });

            var enricher = client!.GetGrain<IEnricherGrain>(dto.Id);
            await enricher.Update(dto);

            return await enricher.GetState();
        }
    }
}
