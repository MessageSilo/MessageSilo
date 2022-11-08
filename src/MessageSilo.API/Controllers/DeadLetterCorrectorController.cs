using MessageSilo.Shared.Grains;
using MessageSilo.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace MessageSilo.API.Controllers
{
    [Route("api/v1")]
    public class DeadLetterCorrectorController : MessageSiloControllerBase
    {
        private readonly IUserGrain user;

        public DeadLetterCorrectorController(ILogger<DeadLetterCorrectorController> logger, IClusterClient client, IHttpContextAccessor httpContextAccessor) : base(logger, client, httpContextAccessor)
        {
            user = client.GetGrain<IUserGrain>(loggedInUserId);
        }

        [HttpGet("DeadLetterCorrector")]
        public async Task<List<ConnectionSettingsDTO>> List()
        {
            return await user.GetDeadLetterCorrectors();
        }

        [HttpPut("DeadLetterCorrector")]
        public async Task Upsert([FromBody] ConnectionSettingsDTO dto)
        {
            await user.AddDeadLetterCorrector(dto);
        }
    }
}
