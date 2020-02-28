using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace UiTTeamBuilder.GraphCode
{
    public class ProtectedApiCallHelper
    {
        protected HttpClient HttpClient { get; private set; }
        public ProtectedApiCallHelper(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public async Task CallWebApiAndProcessResultASync( string webApiUrl, string accessToken, Action<JObject> processResult)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                var defaultRequetHeaders = HttpClient.DefaultRequestHeaders;
                if (defaultRequetHeaders.Accept == null ||
                !defaultRequetHeaders.Accept.Any(m => m.MediaType == "application/json"))
                {
                    HttpClient.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }
                defaultRequetHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", accessToken);
                HttpResponseMessage response =
                await HttpClient.GetAsync(webApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JObject result = JsonConvert.DeserializeObject(json) as JObject;
                    processResult(result);
                }
                else
                {
                    string content =
                    await response.Content.ReadAsStringAsync();
                    // Note that if you got reponse.Code == 403 
                    // and reponse.content.code == "Authorization_RequestDenied"
                    // this is because the tenant admin as not granted 
                    // consent for the application to call the Web API
                    Console.WriteLine($"Content: {content}");
                }
            }
        }
    }
}
