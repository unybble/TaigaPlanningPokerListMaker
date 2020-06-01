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
          
        }
        static public async Task CreateReports()
        {
            List<Project> projects = new List<Project>();
            List<UserStory> userStories = new List<UserStory>();
            List<Issue> issues = new List<Issue>();
        
            List<string> uniqueUsers = new List<string>();

            using (var client = new AuthHttpClient())
            {
                //get token and authenticate
                await client.GetToken();

                //collect all projects
                projects = await Project.GetAll(client);

                foreach (var p in projects)
                {
                    //get milestones
                    var _milestones = await Milestone.GetAll(p.id, client);
                    //get points
                    var _points = await Point.GetAll(p.id, client);
                    Console.WriteLine(p.name);
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
                        if (u.owner != null && _users.Any(x => x.id == u.owner))
                            u.owner_name = _users.FirstOrDefault(x => x.id == u.owner).full_name;
                        if (u.milestone != null && _milestones.Any(x => x.project == p.id))
                            u.milestone_str = _milestones.FirstOrDefault(x => x.id == u.milestone).name;
                        
                        
                        int? k = 0;
                        foreach (var pt in u.points)
                        {

                            if (_points.Any(x => x.id.ToString().Equals(pt.Value.ToString())))
                            {
                               k += _points.Where(x => x.id.ToString().Equals(pt.Value.ToString())).FirstOrDefault().value;
                            }
                            
                        }
                        u.total_us_points = k;
                    }

                    //update project name
                    var j = p.userStories.Where(x => !x.status_str.ToLower().Equals("archived"));
                    j.ForEach(x => x.project_str = p.name);
                    userStories.AddRange(j);
                    //create user specific list


                    p.issues = new List<Issue>();
                    p.issues = await Issue.GetAll(p.id, client);
                

                    foreach (var u in p.issues)
                    {
                        
                        if (u.assigned_to != null && _users.Any(x => x.id == u.assigned_to))
                            u.assigned_to_name = _users.FirstOrDefault(x => x.id == u.assigned_to).full_name;
                        if(u.owner !=null && _users.Any(x => x.id == u.owner))
                            u.owner_name = _users.FirstOrDefault(x => x.id == u.owner).full_name;
                       
                    }
                    var i = p.issues.Where(x =>  !x.status_str.ToLower().Equals("archived"));
                    i.ForEach(x => x.project_str = p.name);
                    issues.AddRange(i);

                 

                }
                // userStories = UserStory.Filter(userStories);
                Console.WriteLine("US details");
                userStories = await UserStory.GetDetails(userStories, client);
                // issues = Issue.Filter(issues);
                Console.WriteLine("Issue details");
                issues = await Issue.GetDetails(issues, client);
            }//end using

         

            //*** CSV Writing ***//
            CSVReportWriter.OutputIssues(path, issues, "issues");
            CSVReportWriter.OutputUserStories(path, userStories, "user_stories");
            Reports.ByUser(path+"/ByUser", uniqueUsers, userStories, issues);
            Console.WriteLine("End");
        }

      
    }
}
