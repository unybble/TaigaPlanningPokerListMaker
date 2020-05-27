using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
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
            Map(m => m.tags).Ignore();
            Map(m => m.tag_list).Ignore();
            Map(m => m.id).Ignore();
        }
    }

    public class UserStory
    {
        [Name("Id")]
        public string id { get; set; }
        [Name("Project")]
        public string project_str { get; set; }
        [Name("Assigned_To")]
        public string assigned_to_name { get; set; }
        [Name("Subject")]
        public string subject { get; set; }
        
        [Name("Tags")]
        public string tags_as_string {
          
        get {
                var retValue = "--";
                if (tags.Count> 0)
                {
                    var sb = new StringBuilder();
                    foreach (JArray t in tags)
                    {
                        sb.Append(t.FirstOrDefault());
                        sb.Append(", ");
                    }
                    var str = sb.ToString();
                    retValue = str.Substring(0, str.Length-2);   
                }
                return retValue;
            } 
        }

        [Name("Description")]
        public string description { get; set; }
        [Name("HasAcceptanceCriteria")]
        public bool has_acceptance_criteria
        {
            get
            {
                if (description != null)
                    return description.ToLower().Contains("acceptance criteria");
                return false;
            }
        }
        public List<string> tag_list
        {
            get
            {
                List<string> list = new List<string>();
                if (tags.Count > 0)
                {
                    foreach (JArray t in tags)
                        list.Add(t.FirstOrDefault().ToString());
                }
                return list;
            }
        }
        public JArray tags { get; set; }
        [Name("Status")]
        public string status_str { get; set; }
        [Name("Backlog_Order")]
        public long? backlog_order { get; set; }
        [Name("Is_Blocked")]
        public bool is_blocked { get; set; }
        [Name("Blocked_Note")]
        public string blocked_note { get; set; }
        [Name("Milestone")]
        public string milestone_str { get; set; }
        [Name("Milestone_Date")]
        public DateTime milestone_start { get; set; }
        [Name("Milestone_Points")]
        public decimal? total_points { get; set; }
        public long? milestone { get; set; }
     
        public string comment { get; set; }

     
        public bool is_closed { get; set; }
        
       
       
        public long? project { get; set; }

        public long? assigned_to { get; set; }
        public long? status { get; set; }
      

        public static async Task<List<UserStory>> GetDetails(List<UserStory> stories, AuthHttpClient httpClient)
        {
            var _stories = new List<UserStory>();
            foreach (var s in stories)
            {
                using (var response = await httpClient.GetAsync("userstories/" + s.id))
                {

                    var content = await response.Content.ReadAsStringAsync();
                    if (content.Length != 0)
                    {
                        UserStory story = JObject.Parse(content).ToObject<UserStory>();
                        _stories.Add(story);

                    }
                }
            }
            return _stories;
        }
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
            var _milestone = await Milestone.GetAll(projectId, httpClient);


            foreach (var u in _usList)
            {
              u.status_str = _status.FirstOrDefault(x => x.id == u.status).name;
                if (u.milestone != null)
                {
                    u.milestone_str = _milestone.FirstOrDefault(x => x.id == u.milestone).name;
                    u.milestone_start= _milestone.FirstOrDefault(x => x.id == u.milestone).created_date;
                    if(_milestone.FirstOrDefault(x => x.id == u.milestone).total_points>0)
                     u.total_points = _milestone.FirstOrDefault(x => x.id == u.milestone).total_points;

                }

            }
            return _usList;
        }
    }
}
