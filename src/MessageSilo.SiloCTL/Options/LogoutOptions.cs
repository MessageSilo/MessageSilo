using CommandLine;

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
                config.ClearToken();
                Console.WriteLine("LOGOUT SUCCEEDED");
                return;
            }

            Console.WriteLine("LOGOUT FAILED");
        }
    }
}
