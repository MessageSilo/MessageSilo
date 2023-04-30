using MessageSilo.Features.Connection;
using MessageSilo.Features.MessageCorrector;
using MessageSilo.Features.Target;
using MessageSilo.Shared.DataAccess;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Amqp.Framing;
using Orleans;

namespace MessageSilo.API.Controllers
{
    [Route("api/v1")]
    public class TargetsController : MessageSiloControllerBase
    {
        public TargetsController(ILogger<ConnectionsController> logger, IClusterClient client, IHttpContextAccessor httpContextAccessor, IEntityRepository repo)
            : base(logger, httpContextAccessor, client, repo)
        {
        }

        [HttpGet("Targets")]
        public async Task<IEnumerable<TargetDTO>> Index()
        {
            var result = new List<TargetDTO>();

            var targets = await repo.Query(EntityKind.Target, loggedInUserId);

            foreach (var t in targets)
            {
                var target = client!.GetGrain<ITargetGrain>(t.Id);
                result.Add(await target.GetState());
            }

            return result;
        }

        [HttpGet("Targets/{name}")]
        public async Task<TargetDTO> Show(string name)
        {
            var id = $"{loggedInUserId}|{name}";
            var targets = await repo.Query(EntityKind.Target, loggedInUserId);

            if (!targets.Any(p => p.Id == id))
            {
                httpContextAccessor!.HttpContext!.Response.StatusCode = StatusCodes.Status404NotFound;
                return null!;
            }

            var target = client!.GetGrain<ITargetGrain>(id);

            return await target.GetState();
        }

        [HttpPut("Targets/{name}")]
        public async Task<TargetDTO> Update(string name, [FromBody] TargetDTO dto)
        {
            dto.PartitionKey = loggedInUserId;

            await repo.Add(new[]
            {
                new Entity()
                {
                    PartitionKey = loggedInUserId,
                    RowKey = name,
                    Kind = EntityKind.Target
                }
            });

            var target = client!.GetGrain<ITargetGrain>(dto.Id);
            await target.Update(dto);

            return await target.GetState();
        }
    }
}
