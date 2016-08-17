using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.OrbScript
{
    public class OrbScriptException : Exception
    {
        public OrbScriptException(string message) : base(message)
        {

        }
    }
}
