using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebOppointmentApi.Common
{
    public class OppointmentApi
    {
        private readonly OppointmentApiOptions apiOptions;

        public OppointmentApi()
        {

        }

        public HttpClient GetHttpClient(string baseUri)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(baseUri)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public async Task<string> DoPostAsync(string baseUri, string url, string head, string body)
        {
            HttpClient client = GetHttpClient(baseUri);

            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                {"head",head},
                {"body",body}
            });
            var response = await client.PostAsync(url, content).ContinueWith(x => x.Result);

            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
