using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageSilo.SiloCTL.Options
{
    [Verb("logout", HelpText = "Logout from Message Silo.")]
    public class LogoutOptions : Options
    {
        protected readonly AuthAPIService authApi;

        public LogoutOptions() : base()
        {
            authApi = new AuthAPIService(config);
        }

        public void Logout()
        {
            if (authApi.Logout())
            {
                config.Delete();
                Console.WriteLine("LOGOUT SUCCEEDED");
                return;
            }

            Console.WriteLine("LOGOUT FAILED");
        }
    }
}
