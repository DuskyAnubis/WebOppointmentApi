using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WebOppointmentApi.Common
{
    public class OppointmentApi
    {
        private readonly OppointmentApiOptions apiOptions;

        public OppointmentApi(OppointmentApiOptions apiOptions)
        {
            this.apiOptions = apiOptions;
        }

        public HttpClient GetHttpClient()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(apiOptions.BaseUri)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public async Task<string> DoPostAsync<T>(string url, string json)
        {
            HttpClient client = GetHttpClient();

            var response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json")).ContinueWith(x => x.Result);

            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
