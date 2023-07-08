using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MessageSilo.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost(template: "login")]
        public async Task<IActionResult> Login([FromBody] Guid userId)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.NameIdentifier, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, $"obapp-{userId.ToString()}"));
            identity.AddClaim(new Claim(ClaimTypes.Role, "obapp-user"));
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(1)
                });

            return Ok();
        }

        [HttpGet(template: "unauthorized")]
        public IActionResult GetUnauthorized()
        {
            return Unauthorized();
        }

        [HttpGet(template: "forbidden")]
        public IActionResult GetForbidden()
        {
            return Forbid();
        }
    }
}
