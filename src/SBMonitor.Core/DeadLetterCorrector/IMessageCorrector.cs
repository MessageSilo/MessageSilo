using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.DeadLetterCorrector
{
    public interface IMessageCorrector
    {
        string Correct(string message, string currectorFuncBody);
    }
}
