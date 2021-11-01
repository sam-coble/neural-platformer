using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
namespace BrainGame
{
    public class DataGetterFromServer
    {
        const string URI = "http://127.0.0.1";
        const int Port = 3000;
        private readonly HttpClient _client = new HttpClient();
        StringContent EmptyContent;

        public DataGetterFromServer()
        {
            EmptyContent = new StringContent("");
        }
        private async Task<JObject> getDataFromNodeServer()
        {
            HttpResponseMessage response;
            string responseBody;
            try
            {
                var builder = new UriBuilder(URI + "/update")
                {
                    Port = Port
                };

                response = await _client.PostAsync(builder.Uri.AbsoluteUri, EmptyContent);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                responseBody = null;
            }
            return JObject.Parse(responseBody);
            
        }
    }
}
