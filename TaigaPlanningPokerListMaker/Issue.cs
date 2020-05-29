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
            Map(m => m.id).Ignore();
            Map(m => m.owner).Ignore();
            Map(m => m.assigned_to).Ignore();
            Map(m => m.project).Ignore();
            Map(m => m.severity).Ignore();
            Map(m => m.status).Ignore();
            Map(m => m.priority).Ignore();
            Map(m => m.type).Ignore();
            Map(m => m.tags).Ignore();
            Map(m => m.tag_list).Ignore();
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
        [Name("Id")]
        public string id { get; set; }
        [Name("Project")]
        public string project_str { get; set; }
        [Name("Assigned_To")]
        public string assigned_to_name { get; set; }
        [Name("Owner_To")]
        public string owner_name { get; set; }
        [Name("Reference")]
        public string reference { get; set; }
        [Name("Subject")]
        public string subject { get; set; }
        [Name("Tags")]
        public string tags_as_string
        {

            get
            {
                var retValue = "--";
                if (tags.Count > 0)
                {
                    var sb = new StringBuilder();
                    foreach (JArray t in tags)
                    {
                        sb.Append(t.FirstOrDefault());
                        sb.Append(", ");
                    }
                    var str = sb.ToString();
                    retValue = str.Substring(0, str.Length - 2);
                }
                return retValue;
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
        [Name("Description")]
        public string description { get; set; }
        [Name("HasExpectedActual")]
        public bool has_acceptance_criteria
        {
            get
            {
                if(description!=null)
                    return description.ToLower().Contains("expect") && description.ToLower().Contains("actual");
                return false;
            }
        }
        public long? project { get; set; }
        public long? status { get; set; }
        public long? priority { get; set; }
        public long? type { get; set; }
        public long? severity { get; set; }
        public long? assigned_to { get; set; }
        public long? owner { get; set; }

        public static async Task<List<Issue>> GetDetails(List<Issue> issues, AuthHttpClient httpClient)
        {
            
            foreach (var s in issues)
            {
                using var response = await httpClient.GetAsync("issues/" + s.id);
                var content = await response.Content.ReadAsStringAsync();
                if (content.Length != 0)
                {
                    Issue issue = JObject.Parse(content).ToObject<Issue>();
                    s.description = issue.description;

                }
            }
            return issues;
        }

        public static List<Issue> Filter(List<Issue> issues)
        {
            List<Issue> filtered = new List<Issue>();
            if (FilterOptions.Tags.Count > 0)
            {
                foreach (var filter in FilterOptions.Tags)
                {

                    if (issues.Any(x => x.tag_list.Contains(filter)))
                    {
                        var _filtered = issues.Where(x => x.tag_list.Contains(filter));
                        filtered.AddRange(_filtered);
                    }
                }
                return filtered;
            }

            return issues;
        }

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
