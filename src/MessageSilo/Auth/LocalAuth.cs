using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace MessageSilo.Auth
{
    public class LocalAuthSchemeOptions: AuthenticationSchemeOptions
    {

    }

    public class LocalAuthHandler: AuthenticationHandler<LocalAuthSchemeOptions>
    {
        public LocalAuthHandler(
            IOptionsMonitor<LocalAuthSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
            {
                return Task.FromResult(AuthenticateResult.Fail("Header Not Found."));
            }

            var header = Request.Headers[HeaderNames.Authorization].ToString();

            if (Guid.TryParse(header, out Guid userId))
            {
                var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Role, "local-user") };

                var claimsIdentity = new ClaimsIdentity(claims,
                            nameof(LocalAuthHandler));

                var ticket = new AuthenticationTicket(
                    new ClaimsPrincipal(claimsIdentity), this.Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));

            }

            return Task.FromResult(AuthenticateResult.Fail("Invalid Token."));
        }
    }
}
