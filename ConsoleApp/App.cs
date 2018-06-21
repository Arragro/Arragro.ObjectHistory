using System;
using System.Threading;
using System.Threading.Tasks;
using Arragro.ObjectHistory.ObjectHistoryClientProvider;
using ConsoleApp.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsoleApp
{
    public class App : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ObjectHistoryClient _objectHistoryClient;

        public App(ILogger<App> logger, ObjectHistoryClient objectHistoryClient)
        {
            _logger = logger;
            _objectHistoryClient = objectHistoryClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting");

            MakeChanges();

            return Task.CompletedTask;
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

            await _objectHistoryClient.SaveObjectHistoryAsync<Parent>(() => $"{parent.ParentId}", parent, parent2);

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
