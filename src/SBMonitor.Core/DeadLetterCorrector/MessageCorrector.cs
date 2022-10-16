using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SBMonitor.Core.DeadLetterCorrector
{
    public class MessageCorrector : IMessageCorrector
    {
        public string Correct(string message, string currectorFuncBody)
        {
            using var engine = new V8ScriptEngine();

            engine.ExecuteCommand($"correct = {currectorFuncBody}");

            return engine.Script.correct(message);
        }
    }
}
