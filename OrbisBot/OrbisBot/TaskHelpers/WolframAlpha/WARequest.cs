using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using WolframAlphaNET;
using WolframAlphaNET.Misc;
using WolframAlphaNET.Objects;

namespace OrbisBot.TaskHelpers.WolframAlpha
{
    /// <summary>
    /// Condensed wolfram alpha request package that returns everything in a single image. Requires an API key
    /// This api key will be taken from App.config with the key name of WolframAlphaAPIKey
    /// </summary>
    public class WARequest
    {
        private static string _appKey = ConfigurationManager.AppSettings["WolframAlphaAPIKey"];

        private WolframAlphaNET.WolframAlpha _wolfram;

        /// <summary>
        /// creates an instance of the Wolfram Alpha request class, throws an exception if no key is provided
        /// </summary>
        public WARequest()
        {
            _wolfram = new WolframAlphaNET.WolframAlpha(_appKey);
            _wolfram.UseTLS = true; //Use encryption
        }

        /// <summary>
        /// creates a request with the request as a string. Returns the full image
        /// </summary>
        public Image WolframRequest(string request)
        {
            QueryResult results = _wolfram.Query(request);
            results.RecalculateResults();

            var imageGenerator = new WAImageGenerator(results.Pods);
            return imageGenerator.GenerateImage();
        }
    }
}
