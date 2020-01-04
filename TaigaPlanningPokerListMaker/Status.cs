using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaigaPlanningPokerListMaker
{
    public class Status
    {
        public string color { get; set; }
        public int id { get; set; }
        public bool is_archived { get; set; }
        public bool is_closed { get; set; }
        public string name { get; set; }
        public int order { get; set; }
        public int project { get; set; }
        public string slug { get; set; }
        public int? wip_limit { get; set; }

        public static async Task<List<Status>> GetAllUserStoryStatus(AuthHttpClient client, long projectId)
        {
            var response = await client.GetAsync("userstory-statuses?project=" + projectId);
            var content = await response.Content.ReadAsStringAsync();
            if (content.Length != 0)
            {
                return JArray.Parse(content).ToObject<List<Status>>();
            }
            return new List<Status>();

        }

        static public async Task<List<Status>> GetAllIssueStatus(AuthHttpClient client, long projectId)
        {

            using (var response = await client.GetAsync("issue-statuses?project=" + projectId))
            {
                var content = await response.Content.ReadAsStringAsync();

                if (content.Length != 0)
                {
                    return JArray.Parse(content).ToObject<List<Status>>();
                }
                return new List<Status>();
            }
        }
    }
}
