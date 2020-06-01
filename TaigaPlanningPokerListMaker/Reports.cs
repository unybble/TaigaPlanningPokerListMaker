using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TaigaPlanningPokerListMaker
{

   
    public static class Reports
    {
        public static void ByMilestone(string path, List<string> users, List<UserStory> userStories)
        {
            Directory.CreateDirectory(path);

            var milestones = userStories.Where(x => x.milestone!=null).Distinct();
            Console.WriteLine("-- Milestones --");

            foreach (var m in milestones)
            {
                Console.WriteLine(m.milestone_str);

                List<UserStory> _us = new List<UserStory>();
                foreach (var name in users)
                {
                    if (userStories.Any(x => x.assigned_to_name != null && x.assigned_to_name.Equals(name)))
                    {
                        _us = userStories.Where(x => x.assigned_to_name != null && x.assigned_to_name.Equals(name)).DefaultIfEmpty().ToList();
                    }
                }

                if (_us.ToList().Count > 0)
                {
                    using var writer = new StreamWriter(path + "/" + m.milestone_str + "_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv");
                    using var csv = new CsvWriter(writer);
                    csv.Configuration.RegisterClassMap<UserStoryMap>();
                    csv.WriteRecords(_us);
                    writer.Flush();

                }

            }
        }
     

        public static void ByUser(string path, List<string> users, List<UserStory> userStories, List<Issue> issues)
        {
            Console.WriteLine("-- By User --");
            Directory.CreateDirectory(path);

            foreach (var name in users)
            {

             
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
                    using var writer = new StreamWriter(path + "/user_stories_" + name + "_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv");
                    using var csv = new CsvWriter(writer);
                    csv.Configuration.RegisterClassMap<UserStoryMap>();
                    csv.WriteRecords(_us);
                    writer.Flush();

                }

                if (_issues.ToList().Count > 0)
                {
                    using var writer = new StreamWriter(path + "/issues_" + name + "_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv");
                    using var csv = new CsvWriter(writer);
                    csv.Configuration.RegisterClassMap<IssueMap>();
                    csv.WriteRecords(_issues);
                    writer.Flush();
                }
            }

        }
    }
}
