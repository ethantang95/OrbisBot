using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.OrbScript
{
    public class OrbScriptEvaluator
    {
        OrbScriptExtractorResults _extractedInfo;

        public OrbScriptEvaluator(OrbScriptExtractorResults extractedInfo)
        {
            _extractedInfo = extractedInfo;
        }
    }
}
