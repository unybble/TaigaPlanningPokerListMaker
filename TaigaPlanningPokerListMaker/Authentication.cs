using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TaigaPlanningPokerListMaker
{
    public class AuthHttpClient : HttpClient
    {



        public AuthHttpClient()
        {

            DefaultRequestHeaders.Accept.Clear();
            DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            DefaultRequestHeaders.Add("x-disable-pagination", "True");
            BaseAddress = new Uri(Configurations.Url);



        }
        public async Task GetToken()
        {
            HttpResponseMessage response;
            using (var content = new FormUrlEncodedContent(Configurations.Values))
            {

                response = await PostAsync("auth", content);
            }

            var auth_object_json = await response.Content.ReadAsStringAsync();
            var auth_object = JObject.Parse(auth_object_json);
            dynamic auth_token = auth_object.GetValue("auth_token");
            DefaultRequestHeaders.Add("Authorization", "Bearer " + auth_token);

        }
    }
}
