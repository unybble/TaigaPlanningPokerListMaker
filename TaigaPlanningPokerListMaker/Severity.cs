using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaigaPlanningPokerListMaker
{
    public class Severity
    {
        
        public int id { get; set; }
        public string name { get; set; }


        public static async Task<List<Severity>> GetAll(AuthHttpClient client, long projectId)
        {
            var response = await client.GetAsync("severities?project=" + projectId);
            var content = await response.Content.ReadAsStringAsync();
            if (content.Length != 0)
            {
                return JArray.Parse(content).ToObject<List<Severity>>();
            }
            return new List<Severity>();

        }

      
    }
}
