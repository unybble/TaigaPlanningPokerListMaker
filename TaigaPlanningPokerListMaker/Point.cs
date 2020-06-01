using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaigaPlanningPokerListMaker
{
    public class Point
    {
        public int id { get; set; }
        public string name { get; set; }
        public int order { get; set; }
        public int project { get; set; }
        public int? value { get; set; }

        public static async Task<List<Point>> GetAll(long projectId, AuthHttpClient client)
        {
            string content;
            using (var response = await client.GetAsync($"points?project=" + +projectId))
            {
                content = await response.Content.ReadAsStringAsync();
            }

            if (content.Length != 0 && content.Length > 3)
            {
                var list = JArray.Parse(content).ToObject<List<Point>>();
                return list.DefaultIfEmpty().ToList();
            }
            return new List<Point>();
        }
    }
}
