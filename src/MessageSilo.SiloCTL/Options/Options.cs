namespace MessageSilo.SiloCTL.Options
{
    public abstract class Options
    {
        protected readonly CTLConfig config;

        public Options()
        {
            config = new CTLConfig();
            config.Load();
        }
    }
}
