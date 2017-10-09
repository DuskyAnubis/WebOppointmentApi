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
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string timeSpan = Convert.ToInt64(ts.TotalSeconds).ToString();
            string token = Encrypt.Md5Encrypt(apiOptions.SecretKey + apiOptions.FromType + timeSpan);

            var client = new HttpClient
            {
                BaseAddress = new Uri(apiOptions.BaseUri)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("token", Encrypt.Base64Encode(token));
            client.DefaultRequestHeaders.Add("version", Encrypt.Base64Encode(apiOptions.Version));
            client.DefaultRequestHeaders.Add("fromtype", Encrypt.Base64Encode(apiOptions.FromType));
            client.DefaultRequestHeaders.Add("sessionid", Encrypt.Base64Encode(apiOptions.FromType + timeSpan));
            client.DefaultRequestHeaders.Add("time", Encrypt.Base64Encode(timeSpan));

            return client;
        }

        public async Task<string> DoPostAsync<T>(string url, T var) where T : class
        {
            HttpClient client = GetHttpClient();

            var response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(var), Encoding.UTF8, "application/json")).ContinueWith(x => x.Result);

            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
