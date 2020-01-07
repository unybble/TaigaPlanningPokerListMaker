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
        private static string path = "/Users/Jen/Projects/TaigaPlanningPokerListMaker/TaigaPlanningPokerListMaker/csvs/";
        private static string sub_path = "ByUser/";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting List Maker");

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
                    //list of users for open issues
                    List<User> _users = await User.GetAll(p.id, client);

                    //get unique names
                    uniqueUsers = _users.Select(x => x.full_name).Distinct().ToList();

                    p.userStories = new List<UserStory>();
                    p.userStories = await UserStory.GetAll(p.id, client);

                    //add username to the userlist
                    foreach (var u in p.userStories)
                    {

                        if (u.assigned_to != null && _users.Any(x => x.id == u.assigned_to))
                            u.assigned_to_name = _users.FirstOrDefault(x => x.id == u.assigned_to).full_name;

                    }



                    //take only new and todo and add to larger list
                    var t = p.userStories.Where(x => x.status_str.ToLower().Equals("new"));
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
                    foreach (var u in p.issues)
                    {
                        if (u.assigned_to != null && _users.Any(x => x.id == u.assigned_to))
                            u.assigned_to_name = _users.FirstOrDefault(x => x.id == u.assigned_to).full_name;
                    }
                    var i = p.issues.Where(x => x.status_str.ToLower().Equals("new"));
                  
                    i.ForEach(x => x.project_str = p.name);
                    issues.AddRange(i);
                    var ii = p.issues.Where(x => x.status_str.ToLower().Contains("ready for test") || x.status_str.ToLower().Contains("done"));

                    ii.ForEach(x => x.project_str = p.name);
                    issuesTesting.AddRange(ii);

                }
             
                


            }//end using

            //do whatever filtering
            //1. no ecriss 3.0
            issues = issues.Where(x =>
                !x.subject.ToLower().Contains("alpha30") &&
                !x.subject.ToLower().Contains("beta40") &&
                !x.subject.ToLower().Contains("[3.0]"))
                .ToList();

            var unassigned_issues = issues.Where(x => x.assigned_to == null);

            userStories = userStories.Where(x =>
               !x.subject.ToLower().Contains("alpha30") &&
               !x.subject.ToLower().Contains("beta40") &&
               x.milestone==null &&
               !x.subject.ToLower().Contains("[3.0]")) 
               .OrderByDescending(x=>x.backlog_order)
               .ToList();

            userStoriesTesting = userStoriesTesting.OrderByDescending(x => x.milestone_start).ToList();
            issuesTesting = issuesTesting.OrderByDescending(x => x.finished_date).ToList();


            using (var writer = new StreamWriter(path + "issues_"+DateTime.Now.ToString("MM-dd-yyyy")+".csv"))
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.RegisterClassMap<IssueMap>();
                csv.WriteRecords(issues);
                writer.Flush();
            }
            using (var writer = new StreamWriter(path + "unassigned_issues_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv"))
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.RegisterClassMap<IssueMap>();
                csv.WriteRecords(unassigned_issues);
                writer.Flush();
            }
            using (var writer = new StreamWriter(path + "user_stories_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv"))
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.RegisterClassMap<UserStoryMap>();
                csv.WriteRecords(userStories);
                writer.Flush();
            }



            ///testing

            using (var writer = new StreamWriter(path + "issues_completed_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv"))
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.RegisterClassMap<IssueMap>();
                csv.WriteRecords(issuesTesting);
                writer.Flush();
            }
            using (var writer = new StreamWriter(path + "user_stories_completed_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv"))
            using (var csv = new CsvWriter(writer))
            {
                csv.Configuration.RegisterClassMap<UserStoryMap>();
                csv.WriteRecords(userStoriesTesting);
                writer.Flush();
            }
            /// /create csv per user

            foreach (var name in uniqueUsers)
            {

                Console.WriteLine(name);
                List<Issue> _issues = new List<Issue>();

                if (issues.Any(x => x.assigned_to_name != null && x.assigned_to_name.Equals(name)))
                {

                    _issues = issues.Where(x => x.assigned_to_name != null && x.assigned_to_name.Equals(name)).DefaultIfEmpty().ToList();

                }


                List<UserStory> _us = new List<UserStory>();
                if (userStories.Any(x => x.assigned_to_name != null && x.assigned_to_name.Equals(name)))
                {
                    _us = userStories.Where(x => x.assigned_to_name != null && x.assigned_to_name.Equals(name)).DefaultIfEmpty().ToList();
                }
                if (_us.ToList().Count > 0)
                {
                    using (var writer = new StreamWriter(path + sub_path+"user_stories_" + name + "_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv"))
                    using (var csv = new CsvWriter(writer))
                    {
                        csv.Configuration.RegisterClassMap<UserStoryMap>();
                        csv.WriteRecords(_us);
                        writer.Flush();
                    }

                }

                if (_issues.ToList().Count > 0)
                {
                    using (var writer = new StreamWriter(path + sub_path+ "issues_" + name + "_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv"))
                    using (var csv = new CsvWriter(writer))
                    {
                        csv.Configuration.RegisterClassMap<IssueMap>();
                        csv.WriteRecords(_issues);
                        writer.Flush();
                    }
                }
            }


            Console.WriteLine("End");
          //  Console.ReadLine();
        }
    }
}
