using JsonDiffPatchDotNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ConsoleApp
{
    public class Parent
    {
        public int ParentId { get; set; }
        public string Test { get; set; }

        public Child Child { get; set; }
    }

    public class Child
    {
        public int ChildId { get; set; }
        public string Test { get; set; }

        public Parent Parent { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var parent = new Parent
            {
                ParentId = 1,
                Test = "Test"
            };

            var child = new Child
            {
                ChildId = 1,
                Test = "Test",
                Parent = parent
            };

            parent.Child = child;

            var parent2 = new Parent
            {
                ParentId = 2,
                Test = "Test 2"
            };

            var child2 = new Child
            {
                ChildId = 2,
                Test = "Test 2",
                Parent = parent2
            };

            parent.Child = child;
            parent2.Child = child2;

            var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings
            {
                // PreserveReferencesHandling = PreserveReferencesHandling.All
            };

            var json = JsonConvert.SerializeObject(parent, Formatting.Indented, jsonSettings);
            var json2 = JsonConvert.SerializeObject(parent2, Formatting.Indented, jsonSettings);

            var jdp = new JsonDiffPatch();
            var left = JToken.Parse(json);
            var right = JToken.Parse(json2);

            JToken patch = jdp.Diff(left, right);

            Console.WriteLine(json);
            Console.WriteLine(json2);
            Console.WriteLine(patch);

            Console.ReadKey();
        }
    }
}
