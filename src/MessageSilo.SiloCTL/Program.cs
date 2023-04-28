using CommandLine;
using ConsoleTables;
using MessageSilo.Shared.Enums;
using MessageSilo.Shared.Models;
using MessageSilo.Shared.Serialization;
using MessageSilo.SiloCTL;
using MessageSilo.SiloCTL.Options;
using RestSharp;
using RestSharp.Authenticators;
using System.Diagnostics;

namespace MessageSilo.SiloCTL
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctlConfig = new CTLConfig();
            ctlConfig.CreateIfNotExist();
            ctlConfig.Load();

            var authApi = new AuthAPIService(ctlConfig);

            if (!authApi.IsValidtoken(ctlConfig.Token))
            {
                Process.Start(new ProcessStartInfo(authApi.GetAuthUrl(ctlConfig.Id)) { UseShellExecute = true });

                var code = authApi.HandleAuthResponse(ctlConfig.Id);
                ctlConfig.Token = authApi.GetToken(ctlConfig.Id, code);
                ctlConfig.Save();
            }

            var restOptions = new RestClientOptions(ctlConfig.ApiUrl)
            {
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                Authenticator = new JwtAuthenticator(ctlConfig.Token)
            };

            var api = new MessageSiloAPIService(new RestClient(restOptions));

            Parser.Default.ParseArguments<ShowOptions, ApplyOptions, ConfigOptions, DeleteOptions>(args)
                       .WithParsed<ShowOptions>(o =>
                       {
                           o.Show(ctlConfig.Token, api);
                       })
                       .WithParsed<ApplyOptions>(o =>
                       {
                           var targets = o.InitTargets(ctlConfig.Token, api);
                           o.InitConnections(ctlConfig.Token, api, targets);
                       })
                       .WithParsed<ConfigOptions>(o =>
                       {
                           Console.WriteLine(ctlConfig.ToString());
                       })
                       .WithParsed<DeleteOptions>(o =>
                       {
                           o.Delete(ctlConfig.Token, api);
                       });
        }
    }
}