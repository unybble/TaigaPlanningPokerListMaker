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
    public sealed class IssueMap : ClassMap<Issue>
    {
        public IssueMap()
        {
            AutoMap();
            Map(m => m.assigned_to).Ignore();
            Map(m => m.project).Ignore();
            Map(m => m.severity).Ignore();
            Map(m => m.status).Ignore();
            Map(m => m.priority).Ignore();
            Map(m => m.type).Ignore();
        }
    }

    public class IssueType
    {
        public long? id { get; set; }
        public string name { get; set; }
        public long? project { get; set; }

        public static async Task<List<IssueType>> GetAll(long projectId, AuthHttpClient client)
        {
            using (var response = await client.GetAsync("issue-types?project=" + projectId))
            {
                var content = await response.Content.ReadAsStringAsync();

                if (content.Length != 0)
                {
                    return JArray.Parse(content).ToObject<List<IssueType>>();
                }
                return new List<IssueType>();
            }
        }
    }

    public class Issue
    {
        [Name("Project")]
        public string project_str { get; set; }
        [Name("Assigned_To")]
        public string assigned_to_name { get; set; }
        [Name("Subject")]
        public string subject { get; set; }
        [Name("Status")]
        public string status_str { get; set; }
        [Name("Priority")]
        public string priority_str { get; set; }
        [Name("Type")]
        public string type_str { get; set; }
        [Name("Severity")]
        public string severity_str { get; set; }
        [Name("Blocked_Note")]
        public string blocked_note { get; set; }
        [Name("Is_Blocked")]
        public bool is_blocked { get; set; }
        [Name("Finished")]
       public string finished_date { get; set; }
        public long? project { get; set; }
        public long? status { get; set; }
        public long? priority { get; set; }
        public long? type { get; set; }
        public long? severity { get; set; }
        public long? assigned_to { get; set; }

        public static async Task<List<Issue>> GetAll(long projectId, AuthHttpClient client)
        {

            var _usList = new List<Issue>();
            var dict = new Dictionary<string, List<Issue>>();

            using (var response = await client.GetAsync("issues?project=" + projectId))
            {
                var content = await response.Content.ReadAsStringAsync();
                content = content.Replace("ref", "reference");
                if (content.Length != 0)
                {
                    _usList = JArray.Parse(content).ToObject<List<Issue>>();
                }

            }

            var _status = await Status.GetAllIssueStatus(client, projectId);
            var _severity = await Severity.GetAll(client, projectId);
            var _priority = await Priority.GetAll(client, projectId);
            var _type = await IssueType.GetAll( projectId, client);

            foreach (var u in _usList)
            {
                u.status_str = _status.FirstOrDefault(x => x.id == u.status).name;
                u.priority_str = _priority.FirstOrDefault(x => x.id == u.priority).name;
                u.severity_str = _severity.FirstOrDefault(x => x.id == u.severity).name;
                u.type_str = _type.FirstOrDefault(x => x.id == u.type).name;

            }




            return _usList;
        }




    }
}
