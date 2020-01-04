using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaigaPlanningPokerListMaker
{
    public class User
    {
       
        public string email { get; set; }
        public string full_name { get; set; }
        public string full_name_display { get; set; }
        public long id { get; set; }
      
        public string username { get; set; }
       

        public static async Task<List<User>> GetAll(long projectId, AuthHttpClient client)
        {
            using (var response = await client.GetAsync("users?project=" + projectId))
            {
                var content = await response.Content.ReadAsStringAsync();
                if (content.Length != 0)
                {
                    return JArray.Parse(content).ToObject<List<User>>();
                }
            }
            return new List<User>();
        }

    }
}
