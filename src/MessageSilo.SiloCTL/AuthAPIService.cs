using Azure;
using RestSharp;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using static System.Net.WebRequestMethods;

namespace MessageSilo.SiloCTL
{
    public class AuthAPIService
    {
        private readonly CTLConfig config;

        public AuthAPIService(CTLConfig config)
        {
            this.config = config;
        }

        public string? HandleAuthResponse()
        {
            using var listener = new HttpListener();
            listener.Prefixes.Add($"{config.Auth0RedirectUrl}/");
            listener.Start();

            var ctx = listener.GetContext();
            var req = ctx.Request;
            var res = ctx.Response;

            var query = HttpUtility.ParseQueryString(req.Url.Query);

            var code = query.Get("code");
            var error = query.Get("error");
            var errorDescription = query.Get("error_description");

            if (code is not null)
            {
                writeResponse(res, @$"
                <html>
                    <body>
                        <h1>Message Silo</h1>
                        <h2>LOGIN SUCCEEDED</h2>
                    </body>
                </html>");
                return code;
            }

            writeResponse(res, @$"
            <html>
                <body>
                    <h1>Message Silo</h1>
                    <h2>LOGIN FAILED</h2>
                    <div>${error}</div>
                    <div>${errorDescription}
                </body>
            </html>");
            return null;
        }

        public string GetAuthUrl()
        {
            return $"https://{config.Auth0Domain}/authorize?prompt=login&response_type=code&code_challenge_method=S256&code_challenge={config.Id}&client_id={config.Auth0ClinetID}&redirect_uri={config.Auth0RedirectUrl}&scope=openid%20profile%20email&audience={config.Auth0Audiance}";
        }

        public string GetToken(string code)
        {
            var client = new RestClient($"https://{config.Auth0Domain}");

            var request = new RestRequest("/oauth/token", Method.Post);

            var contentType = "application/x-www-form-urlencoded";

            request.AddHeader("content-type", contentType);
            request.AddParameter(contentType, $"grant_type=authorization_code&client_id={config.Auth0ClinetID}&code_verifier={config.Id}&code={code}&redirect_uri={config.Auth0RedirectUrl}", ParameterType.RequestBody);

            var response = client.Execute(request);

            if (!response.IsSuccessStatusCode)
                return null;

            var data = JsonSerializer.Deserialize<JsonNode>(response.Content!);

            return data["access_token"].GetValue<string>();
        }

        public bool IsValidtoken()
        {
            var client = new RestClient($"https://{config.Auth0Domain}");

            var request = new RestRequest("/userinfo", Method.Get);

            request.AddHeader("Authorization", $"Bearer {config.Token}");

            var response = client.Execute(request);

            return response.IsSuccessStatusCode;
        }

        public bool Logout()
        {
            var client = new RestClient($"https://{config.Auth0Domain}");

            var request = new RestRequest("/oidc/logout", Method.Post);

            var contentType = "application/x-www-form-urlencoded";

            request.AddHeader("content-type", contentType);
            request.AddParameter(contentType, $"federated&client_id={config.Auth0ClinetID}&logout_hint=SESSION_ID", ParameterType.RequestBody);

            var response = client.Execute(request);

            return response.IsSuccessStatusCode;
        }

        private void writeResponse(HttpListenerResponse res, string responseString)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);

            res.ContentLength64 = buffer.Length;
            using var output = res.OutputStream;
            output.Write(buffer, 0, buffer.Length);
        }
    }
}
