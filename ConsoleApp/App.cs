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

            QueryTable();

            //MakeChanges();

            Thread.Sleep(5000);

            //CheckQueue();

            return Task.CompletedTask;
        }

        private async void CheckQueue()
        {
           await _objectHistoryServer.ProcessMessages();
        }

        public async void QueryTable()
        {
            var parent = new Parent
            {
                ParentId = 1,
                Test = "Test"
            };

            var entities = await _objectHistoryClient.GetObjectHistoryAsync<Parent>(() => $"{parent.ParentId}", null);

            Console.WriteLine(entities);

            //foreach (ObjectHistoryEntity entity in entities)
            //{
            //    Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
            //        entity.Folder, entity.User);
            //}
        }

        public async void MakeChanges()
        {
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
                ParentId = 1,
                Test = "Test 2"
            };

            var child2 = new Child
            {
                ChildId = 1,
                Test = "Test 2",
                Parent = parent2
            };

            parent.Child = child;
            parent2.Child = child2;

            await _objectHistoryClient.SaveObjectHistoryAsync<Parent>(() => $"{parent.ParentId}", parent, parent2, "prof. X");

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
