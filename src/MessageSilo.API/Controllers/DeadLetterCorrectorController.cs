using MessageSilo.Features.DeadLetterCorrector;
using MessageSilo.Shared.DataAccess;
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

        private readonly IMessageRepository<CorrectedMessage> messages;

        public DeadLetterCorrectorController(ILogger<DeadLetterCorrectorController> logger, IClusterClient client, IHttpContextAccessor httpContextAccessor, IMessageRepository<CorrectedMessage> messages) : base(logger, client, httpContextAccessor)
        {
            user = client.GetGrain<IUserGrain>(/*loggedInUserId*/"test");
            this.messages = messages;
        }

        [HttpGet("DeadLetterCorrector")]
        public async Task<List<ConnectionSettingsDTO>> List()
        {
            return await user.GetDeadLetterCorrectors();
        }

        [HttpGet("DeadLetterCorrector/{id}")]
        public async Task<ConnectionSettingsDTO?> Find(Guid id)
        {
            return (await user.GetDeadLetterCorrectors()).FirstOrDefault(p => p.Id == id);
        }

        [HttpPut("DeadLetterCorrector")]
        public async Task Upsert([FromBody] ConnectionSettingsDTO dto)
        {
            await user.AddDeadLetterCorrector(dto);
        }

        [HttpGet("DeadLetterCorrector/{id}/Messages")]
        public async Task<IEnumerable<CorrectedMessage>> Messages(Guid id)
        {
            return await messages.Query(id.ToString(), DateTimeOffset.UtcNow.AddHours(-3000), DateTimeOffset.UtcNow);
        }
    }
}
