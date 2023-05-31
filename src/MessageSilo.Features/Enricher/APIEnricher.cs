using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Reactive.Joins;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MessageSilo.Features.Enricher
{
    public class APIEnricher : IEnricher
    {
        private readonly string url;

        private readonly Method method;

        private Regex rg = new Regex("{[a-zA-Z0-9_.-]*}");

        private IRestClient client = new RestClient();

        public APIEnricher(string url, Method method)
        {
            this.url = url;
            this.method = method;
        }

        public async Task<string> TransformMessage(string message)
        {
            var request = new RestRequest(url, method);

            if (method == Method.Post || method == Method.Put)
                request.AddBody(message, contentType: ContentType.Json);
            else
            {
                var matches = rg.Matches(url);
                //VIP
                var replacedURL = url;
                var jsonReader = new JsonTextReader(new StringReader(message));
                jsonReader.FloatParseHandling = FloatParseHandling.Decimal;
                jsonReader.Culture = new CultureInfo("en-US");

                var messageObj = JObject.Load(jsonReader);

                foreach (Match match in matches)
                {
                    var propPath = match.Value.TrimStart('{').TrimEnd('}');
                    var prop = messageObj.SelectToken(propPath);

                    if (prop is not null)
                        replacedURL = replacedURL.Replace(match.Value, prop.ToString());
                }
            }

            var response = await client.ExecuteAsync(request);

            return response.Content;
        }
    }
}
