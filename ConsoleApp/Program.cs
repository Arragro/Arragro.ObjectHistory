using System;
using ConsoleApp.Models;

using System.Threading.Tasks;
using Arragro.ObjectHistory.ObjectHistoryClientProvider;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
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
            var objectHistoryClient = new Arragro.ObjectHistory.ObjectHistoryClientProvider.ObjectHistoryClient("UseDevelopmentStorage=true");
            await objectHistoryClient.SaveObjectHistoryAsync<Parent>(() => $"{parent.ParentId}", parent, parent2);
            Console.WriteLine("Complete");

            Console.ReadKey();
        }
    }
}
