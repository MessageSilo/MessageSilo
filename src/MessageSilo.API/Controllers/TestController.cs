using MessageSilo.Shared.Grains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace MessageSilo.API.Controllers
{
    public class TestController : MessageSiloControllerBase
    {
        public TestController(ILogger<TestController> logger, IClusterClient client, IHttpContextAccessor httpContextAccessor) : base(logger, client, httpContextAccessor)
        {
        }

        [AllowAnonymous]
        public IActionResult Pub()
        {
            var ug = client.GetGrain<IUserGrain>("test");
            return new JsonResult(new { result = "ok" });
        }

        public IActionResult NotPub()
        {
            var ug = client.GetGrain<IUserGrain>("test");
            return new JsonResult(new { result = "ok", userId = loggedInUserId });
        }
    }
}
