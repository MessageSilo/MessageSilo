﻿using MessageSilo.Features.Connection;
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

        [HttpGet("User/{token}/Targets")]
        public async Task<IEnumerable<TargetDTO>> Index(string token)
        {
            var result = new List<TargetDTO>();

            var targets = await repo.Query(EntityKind.Target, token);

            foreach (var t in targets)
            {
                var target = client!.GetGrain<ITargetGrain>(t.Id);
                result.Add(await target.GetState());
            }

            return result;
        }

        [HttpGet("User/{token}/Targets/{name}")]
        public async Task<TargetDTO> Show(string token, string name)
        {
            var id = $"{token}|{name}";
            var targets = await repo.Query(EntityKind.Target, token);

            if (!targets.Any(p => p.Id == id))
            {
                httpContextAccessor!.HttpContext!.Response.StatusCode = StatusCodes.Status404NotFound;
                return null!;
            }

            var target = client!.GetGrain<ITargetGrain>(id);

            return await target.GetState();
        }

        [HttpPut("User/{token}/Targets/{name}")]
        public async Task<TargetDTO> Update(string token, string name, [FromBody] TargetDTO dto)
        {
            var id = $"{token}|{name}";

            await repo.Add(new[]
            {
                new Entity()
                {
                    PartitionKey = token,
                    RowKey = name,
                    Kind = EntityKind.Target
                }
            });

            var target = client!.GetGrain<ITargetGrain>(id);
            await target.Update(dto);

            return await target.GetState();
        }
    }
}