using System;
using System.Threading;
using System.Threading.Tasks;
using Arragro.ObjectHistory.Client;
using Arragro.ObjectHistory.Server;
using ConsoleApp.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleApp
{
    public class App : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ObjectHistoryClient _objectHistoryClient;
        private readonly ObjectHistoryServer _objectHistoryServer;

        public App(ILogger<App> logger, ObjectHistoryClient objectHistoryClient, ObjectHistoryServer objectHistoryServer)
        {
            _logger = logger;
            _objectHistoryClient = objectHistoryClient;
            _objectHistoryServer = objectHistoryServer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting");

            //MakeChanges();
            // MakeOutOfBandChange();
            Thread.Sleep(5000);
            //Thread.Sleep(5000);
            while (true)
            {
                Console.WriteLine("Waiting 20 seconds");
                Thread.Sleep(20000);
                CheckQueue();


            }
            

            //Thread.Sleep(5000);

            //QueryTable();

            return Task.CompletedTask;
        }

        private async void CheckQueue()
        {
            Console.WriteLine("processing messages");
           await _objectHistoryServer.ProcessMessages();
        }

        public async void QueryTable()
        {
            var parent = new Parent
            {
                ParentId = 1,
                Name = "Prof. X"
            };

            var entities = await _objectHistoryClient.GetObjectHistoryAsync<Parent>(() => $"{parent.ParentId}");

            Console.WriteLine(entities);

            //foreach (ObjectHistoryEntity entity in entities)
            //{
            //    Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
            //        entity.Folder, entity.User);
            //}
        }

        public async void MakeOutOfBandChange()
        {
            var parent = new Parent
            {
                ParentId = 1,
                Name = "Rogue"
            };

            var child = new Child
            {
                ChildId = 1,
                Name = "Storm",
                Parent = parent
            };

            parent.Child = child;

            var updateParent = new Parent
            {
                ParentId = 1,
                Name = "Professor Xavier"
            };

            var child2 = new Child
            {
                ChildId = 1,
                Name = "David",
                Parent = updateParent
            };

            parent.Child = child;
            updateParent.Child = child2;

            await _objectHistoryClient.SaveObjectHistoryAsync<Parent>(() => $"{parent.ParentId}", parent, updateParent, "prof. X");
        }

        public async void MakeChanges()
        {
            var parent = new Parent
            {
                ParentId = 1,
                Name = "Prof. X"
            };

            var child = new Child
            {
                ChildId = 1,
                Name = "Legion",
                Parent = parent
            };

            parent.Child = child;

            var updateParent = new Parent
            {
                ParentId = 1,
                Name = "Logan"
            };

            var child2 = new Child
            {
                ChildId = 1,
                Name = "Laura",
                Parent = updateParent
            };

            parent.Child = child;
            updateParent.Child = child2;

            await _objectHistoryClient.SaveObjectHistoryAsync<Parent>(() => $"{parent.ParentId}", parent, updateParent, "prof. X");

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
        }
    }
}
