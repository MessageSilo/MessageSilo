using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
