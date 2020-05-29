using CsvHelper;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TaigaPlanningPokerListMaker
{
    
    class Program
    {
        private static DateTime upper_bound = DateTime.Now.AddDays(-31);
        private static string path = "C:\\Users\\jen\\Documents\\GitHub\\TaigaPlanningPokerListMaker\\TaigaPlanningPokerListMaker\\csvs\\";
  
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting List Maker");
              await CreateReports();
            //await CreateSpecificReport("PLP");



            //  Console.ReadLine();
        }
        static public async Task CreateReports()
        {
            List<Project> projects = new List<Project>();
            List<UserStory> userStories = new List<UserStory>();
            List<Issue> issues = new List<Issue>();
            List<UserStory> userStoriesTesting = new List<UserStory>();
            List<Issue> issuesTesting = new List<Issue>();
            Dictionary<string, List<Issue>> issuesByUser = new Dictionary<string, List<Issue>>();
            Dictionary<string, List<UserStory>> userStoriesByUser = new Dictionary<string, List<UserStory>>();
            List<string> uniqueUsers = new List<string>();

            using (var client = new AuthHttpClient())
            {
                //get token and authenticate
                await client.GetToken();

                //collect all projects
                projects = await Project.GetAll(client);

                foreach (var p in projects)
                {
                    Console.WriteLine(p.name);
                    //list of users for open issues
                    List<User> _users = await User.GetAll(p.id, client);
                    Console.WriteLine("User Count: "+_users.Count.ToString());
                    //get unique names
                    uniqueUsers = _users.Select(x => x.full_name).Distinct().ToList();

                    p.userStories = new List<UserStory>();
                    p.userStories = await UserStory.GetAll(p.id, client);
                    Console.WriteLine("User Stories: " + p.userStories.Count.ToString());
                    ;
                    //add username to the userlist
                    foreach (var u in p.userStories)
                    {

                        if (u.assigned_to != null && _users.Any(x => x.id == u.assigned_to))
                            u.assigned_to_name = _users.FirstOrDefault(x => x.id == u.assigned_to).full_name;
                        if (u.owner != null && _users.Any(x => x.id == u.owner))
                            u.owner_name = _users.FirstOrDefault(x => x.id == u.owner).full_name;

                    }




                    //take only new and todo and add to larger list
                    var t = p.userStories;//.Where(x => x.status_str.ToLower().Equals("new"));
                    //get in testing too for sprint planning
                    var testing = p.userStories.Where(x => x.status_str.ToLower().Contains("ready for test") || x.status_str.ToLower().Contains("done"));
                    //update project name
                    t.ForEach(x => x.project_str = p.name);
                    testing.ForEach(x => x.project_str = p.name);
                    userStories.AddRange(t);
                    userStoriesTesting.AddRange(testing);
                    //create user specific list


                    p.issues = new List<Issue>();
                    p.issues = await Issue.GetAll(p.id, client);
                    Console.WriteLine("Issues: " + p.issues.Count.ToString());

                    foreach (var u in p.issues)
                    {
                        if (u.assigned_to != null && _users.Any(x => x.id == u.assigned_to))
                            u.assigned_to_name = _users.FirstOrDefault(x => x.id == u.assigned_to).full_name;
                        if(u.owner !=null && _users.Any(x => x.id == u.owner))
                            u.owner_name = _users.FirstOrDefault(x => x.id == u.owner).full_name;
                    }
                    //var i = p.issues.Where(x => x.status_str.ToLower().Equals("new"));
                    var i = p.issues.Where(x => !x.status_str.ToLower().Equals("ready for test") && !x.status_str.ToLower().Equals("done") && !x.status_str.ToLower().Equals("archived"));
                    i.ForEach(x => x.project_str = p.name);
                    issues.AddRange(i);
                    var ii = p.issues.Where(x => x.status_str.ToLower().Contains("ready for test") || x.status_str.ToLower().Contains("done"));

                    ii.ForEach(x => x.project_str = p.name);
                    issuesTesting.AddRange(ii);

                }
                userStories = UserStory.Filter(userStories);
                userStories = await UserStory.GetDetails(userStories, client);
                issues = Issue.Filter(issues);
                issues = await Issue.GetDetails(issues, client);
            }//end using

         

            var unassigned_issues = issues.Where(x => x.assigned_to == null).ToList();
            unassigned_issues = Issue.Filter(unassigned_issues);

            userStoriesTesting = userStoriesTesting.OrderByDescending(x => x.milestone_start).ToList();
            issuesTesting = issuesTesting.OrderByDescending(x => x.finished_date).ToList();



            //*** CSV Writing ***//
            CSVReportWriter.OutputIssues(path, issues, "issues");
            CSVReportWriter.OutputIssues(path, unassigned_issues, "unassigned_issues");
            CSVReportWriter.OutputIssues(path, issuesTesting, "issues_completed");
            CSVReportWriter.OutputUserStories(path, userStories, "user_stories");
            CSVReportWriter.OutputUserStories(path, userStoriesTesting, "user_stories_completed_");
            CSVReportWriter.OutputByUser(path+"/ByUser", uniqueUsers, userStories, issues);
            Console.WriteLine("End");
        }

        static public async Task CreateSpecificReport(string projectName)
        {
            List<Project> projects = new List<Project>();
            
            List<Issue> issues = new List<Issue>();
            List<UserStory> userStoriesTesting = new List<UserStory>();
          
            Dictionary<string, List<Issue>> issuesByUser = new Dictionary<string, List<Issue>>();
           
            List<string> uniqueUsers = new List<string>();

            using (var client = new AuthHttpClient())
            {
                //get token and authenticate
                await client.GetToken();

                //collect all projects
                projects = await Project.GetAll(client);

                foreach (var p in projects)
                {
                    //list of users for open issues
                    List<User> _users = await User.GetAll(p.id, client);

                    //get unique names
                    uniqueUsers = _users.Select(x => x.full_name).Distinct().ToList();


                    p.issues = new List<Issue>();
                    p.issues = await Issue.GetAll(p.id, client);
                    foreach (var u in p.issues)
                    {
                        if (u.assigned_to != null && _users.Any(x => x.id == u.assigned_to))
                            u.assigned_to_name = _users.FirstOrDefault(x => x.id == u.assigned_to).full_name;
                        if (u.owner != null && _users.Any(x => x.id == u.owner))
                            u.owner_name = _users.FirstOrDefault(x => x.id == u.owner).full_name;

                    }

                    p.issues.ForEach(x => x.project_str = p.name);

                    //filter for bugs only
                    var i = p.issues.Where(x => x.type_str.ToLower().Equals("bug") && x.project_str.Equals(projectName));
                    issues.AddRange(i);
                  

                }




            }//end using

           

           

          


            using (var writer = new StreamWriter(path + "list_issues_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv"))
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.RegisterClassMap<IssueMap>();
                csv.WriteRecords(issues);
                writer.Flush();
            }
           

            /// /create csv per user

            //foreach (var name in uniqueUsers)
            //{

            //    Console.WriteLine(name);
            //    List<Issue> _issues = new List<Issue>();

            //    if (issues.Any(x => x.assigned_to_name != null && x.assigned_to_name.Equals(name)))
            //    {

            //        _issues = issues.Where(x => x.assigned_to_name != null && x.assigned_to_name.Equals(name)).DefaultIfEmpty().ToList();

            //    }


            //    List<UserStory> _us = new List<UserStory>();
            //    if (userStories.Any(x => x.assigned_to_name != null && x.assigned_to_name.Equals(name)))
            //    {
            //        _us = userStories.Where(x => x.assigned_to_name != null && x.assigned_to_name.Equals(name)).DefaultIfEmpty().ToList();
            //    }
            //    if (_us.ToList().Count > 0)
            //    {
            //        using (var writer = new StreamWriter(path + sub_path + "user_stories_" + name + "_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv"))
            //        using (var csv = new CsvWriter(writer))
            //        {
            //            csv.Configuration.RegisterClassMap<UserStoryMap>();
            //            csv.WriteRecords(_us);
            //            writer.Flush();
            //        }

            //    }

            //    if (_issues.ToList().Count > 0)
            //    {
            //        using (var writer = new StreamWriter(path + sub_path + "issues_" + name + "_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv"))
            //        using (var csv = new CsvWriter(writer))
            //        {
            //            csv.Configuration.RegisterClassMap<IssueMap>();
            //            csv.WriteRecords(_issues);
            //            writer.Flush();
            //        }
            //    }
            //}


            Console.WriteLine("End");
        }
    }
}
