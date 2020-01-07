using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaigaPlanningPokerListMaker
{

    public class Milestone
    {
        public bool closed { get; set; }
        public long? closed_points { get; set; }
        public DateTime created_date { get; set; }
        public long? id { get; set; }
        public DateTime modified_date { get; set; }
        public string name { get; set; }
        public long? project { get; set; }
        public string slug { get; set; }
        public long? total_points { get; set; }
        public List<UserStory> user_stories { get; set; }
        public DateTime? estimated_finish { get; set; }
        public DateTime? estimated_start { get; set; }

        public static async Task<List<Milestone>> GetAll(long projectId, AuthHttpClient client)
        {
            string content;
            using (var response = await client.GetAsync($"milestones?project=" + +projectId))
            {
                content = await response.Content.ReadAsStringAsync();
            }

            if (content.Length != 0 && content.Length > 3)
            {
                var list = JArray.Parse(content).ToObject<List<Milestone>>();
                return list.DefaultIfEmpty().ToList();
            }
            return new List<Milestone>();
        }
    }
}
