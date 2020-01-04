using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TaigaPlanningPokerListMaker
{
    public class Project
    {

        public List<UserStory> userStories { get; set; }
        public List<Issue> issues { get; set; }
        public long id { get; set; }
        public string logo_small_url { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public long? total_story_points { get; set; }
        public long? total_activity { get; set; }
        public long? total_activity_last_month { get; set; }
        public long? total_activity_last_week { get; set; }
        public long? total_activity_last_year { get; set; }
        public long? total_closed_milestones { get; set; }
        public long? total_milestones { get; set; }
       

        static public async Task<List<Project>> GetAll(AuthHttpClient client)
        {
            using (var response = await client.GetAsync("projects"))
            {
                var content = await response.Content.ReadAsStringAsync();
                if (content.Length != 0)
                {
                    return JArray.Parse(content).ToObject<List<Project>>();

                }
                return null;
            }

        }
    }
}
