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
    public sealed class UserStoryMap : ClassMap<UserStory>
    {
        public UserStoryMap()
        {
            AutoMap();
            Map(m => m.assigned_to).Ignore();
            Map(m => m.project).Ignore();
            Map(m => m.status).Ignore();
            Map(m => m.milestone).Ignore();
            Map(m => m.comment).Ignore();
            Map(m => m.is_closed).Ignore();

        }
    }

    public class UserStory
    {
        [Name("Project")]
        public string project_str { get; set; }
        [Name("Assigned_To")]
        public string assigned_to_name { get; set; }
        [Name("Subject")]
        public string subject { get; set; }
        [Name("Status")]
        public string status_str { get; set; }
        [Name("Backlog_Order")]
        public long? backlog_order { get; set; }
        [Name("Is_Blocked")]
        public bool is_blocked { get; set; }
        [Name("Blocked_Note")]
        public string blocked_note { get; set; }

        public string milestone { get; set; }
     
        public string comment { get; set; }

     
        public bool is_closed { get; set; }
      
       
        public long? project { get; set; }

        public long? assigned_to { get; set; }
        public long? status { get; set; }
      


        public static async Task<List<UserStory>> GetAll(long projectId, AuthHttpClient httpClient)
        {

            var _usList = new List<UserStory>();
            var dict = new Dictionary<string, List<UserStory>>();

            using (var response = await httpClient.GetAsync("userstories?project=" + projectId))
            {
                var content = await response.Content.ReadAsStringAsync();
                if (content.Length != 0)
                {
                    _usList = JArray.Parse(content).ToObject<List<UserStory>>();

                }
            }

            var _status = await Status.GetAllUserStoryStatus(httpClient, projectId);
      


            foreach (var u in _usList)
            {
                u.status_str = _status.FirstOrDefault(x => x.id == u.status).name;
                

            }
            return _usList;
        }
    }
}
