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


    }
}
