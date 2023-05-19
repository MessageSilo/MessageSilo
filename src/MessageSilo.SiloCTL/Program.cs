using CommandLine;
using ConsoleTables;
using InfluxDB.Client.Api.Domain;
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
            var interactiveMode = args.Any(p => p == "-it");

            do
            {
                var config = new CTLConfig();
                config.CreateIfNotExist();

                if (interactiveMode)
                    args = Console.ReadLine()!.Split();

                Parser.Default.ParseArguments<ShowOptions, ApplyOptions, ConfigOptions, DeleteOptions, LogoutOptions>(args)
                           .WithParsed<ShowOptions>(o =>
                           {
                               o.Show();
                           })
                           .WithParsed<ApplyOptions>(o =>
                           {
                               var targets = o.InitTargets();
                               o.InitEnrichers();
                               o.InitConnections(targets);
                           })
                           .WithParsed<ConfigOptions>(o =>
                           {
                               o.Show();
                           })
                           .WithParsed<DeleteOptions>(o =>
                           {
                               o.Delete();
                           })
                           .WithParsed<LogoutOptions>(o =>
                           {
                               o.Logout();
                           });
            } while (interactiveMode);
        }
    }
}