using Microsoft.Net.Http.Headers;
using RestSharp;
using RestSharp.Authenticators;
using System.Diagnostics;
using System.Net;

namespace MessageSilo.SiloCTL.Options
{
    public abstract class AuthorizedOptions : Options
    {
        protected readonly AuthAPIService authApi;

        protected readonly MessageSiloAPI api;

        public AuthorizedOptions() : base()
        {
            var restOptions = new RestClientOptions(this.config.ApiUrl)
            {
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                CalculateResponseStatus = httpResponse =>
                httpResponse.IsSuccessStatusCode ||
                httpResponse.StatusCode == HttpStatusCode.NotFound ||
                httpResponse.StatusCode == HttpStatusCode.BadRequest
                ? ResponseStatus.Completed : ResponseStatus.Error
            };

            if (config.ApiUrl.StartsWith(CTLConfig.DEFAULT_API_URL))
            {

                authApi = new AuthAPIService(config);

                if (!authApi.IsValidtoken())
                {
                    Process.Start(new ProcessStartInfo(authApi.GetAuthUrl()) { UseShellExecute = true });

                    var code = authApi.HandleAuthResponse();

                    config.Token = authApi.GetToken(code);

                    config.Save();
                }

                restOptions.Authenticator = new JwtAuthenticator(this.config.Token);
            }

            var client = new RestClient(restOptions);

            if (!config.ApiUrl.StartsWith(CTLConfig.DEFAULT_API_URL))
                client.AddDefaultHeader(HeaderNames.Authorization, config.Id);

            api = new MessageSiloAPI(client);
        }
    }
}
