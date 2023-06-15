using RestSharp;
using RestSharp.Authenticators;
using System.Diagnostics;
using System.Net;

namespace MessageSilo.SiloCTL.Options
{
    public abstract class AuthorizedOptions : Options
    {
        protected readonly AuthAPIService authApi;

        protected readonly MessageSiloAPIService api;

        public AuthorizedOptions() : base()
        {
            authApi = new AuthAPIService(config);

            if (!authApi.IsValidtoken())
            {
                Process.Start(new ProcessStartInfo(authApi.GetAuthUrl()) { UseShellExecute = true });

                var code = authApi.HandleAuthResponse();

                config.Token = authApi.GetToken(code);

                config.Save();
            }

            var restOptions = new RestClientOptions(this.config.ApiUrl)
            {
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                Authenticator = new JwtAuthenticator(this.config.Token),
                CalculateResponseStatus = httpResponse =>
                httpResponse.IsSuccessStatusCode ||
                httpResponse.StatusCode == HttpStatusCode.NotFound ||
                httpResponse.StatusCode == HttpStatusCode.BadRequest
                ? ResponseStatus.Completed : ResponseStatus.Error
            };

            api = new MessageSiloAPIService(new RestClient(restOptions));
        }
    }
}
