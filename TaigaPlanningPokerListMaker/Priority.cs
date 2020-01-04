using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaigaPlanningPokerListMaker
{
    public class Priority
    {
        
        public int id { get; set; }
        public string name { get; set; }


        public static async Task<List<Priority>> GetAll(AuthHttpClient client, long projectId)
        {
            var response = await client.GetAsync("priorities?project=" + projectId);
            var content = await response.Content.ReadAsStringAsync();
            if (content.Length != 0)
            {
                return JArray.Parse(content).ToObject<List<Priority>>();
            }
            return new List<Priority>();

        }

      
    }
}
