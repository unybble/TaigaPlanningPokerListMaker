using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TaigaPlanningPokerListMaker
{
    public static class CSVReportWriter
    {
        public static void OutputIssues(string path, List<Issue> issues, string fileName)
        {
            Directory.CreateDirectory(path);
            using var writer = new StreamWriter(path + fileName + "_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv");
            using var csv = new CsvWriter(writer);
            csv.Configuration.RegisterClassMap<IssueMap>();
            csv.WriteRecords(issues);
            writer.Flush();
        }

        public static void OutputUserStories(string path, List<UserStory> userStories, string fileName)
        {
            Directory.CreateDirectory(path);
            using var writer = new StreamWriter(path + fileName + "_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv");
            using var csv = new CsvWriter(writer);
            csv.Configuration.RegisterClassMap<UserStoryMap>();
            csv.WriteRecords(userStories);
            writer.Flush();
        }


        public static void OutputByUser(string path,List<string> users, List<UserStory> userStories, List<Issue> issues)
        {

            Directory.CreateDirectory(path);

            foreach (var name in users)
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
                    using var writer = new StreamWriter(path + "user_stories_" + name + "_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv");
                    using var csv = new CsvWriter(writer);
                    csv.Configuration.RegisterClassMap<UserStoryMap>();
                    csv.WriteRecords(_us);
                    writer.Flush();

                }

                if (_issues.ToList().Count > 0)
                {
                    using var writer = new StreamWriter(path + "issues_" + name + "_" + DateTime.Now.ToString("MM-dd-yyyy") + ".csv");
                    using var csv = new CsvWriter(writer);
                    csv.Configuration.RegisterClassMap<IssueMap>();
                    csv.WriteRecords(_issues);
                    writer.Flush();
                }
            }

        }
    }
}
