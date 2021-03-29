using Arragro.Core.Docker;
using Arragro.ObjectHistory.Client.Extensions;
using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Interfaces;
using Arragro.ObjectHistory.Core.Models;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arragro.ObjectHistory.IntegrationTests
{
    public class BaseTest : IDisposable
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ObjectHistorySettings _objectHistorySettings;
        protected const string AzureStorageConnectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

        public BaseTest()
        {
            DockerExtentions.StartDockerServicesAsync(new List<Func<DockerClient, Task<ContainerListResponse>>>
            {
                AzuriteMicrosoft.StartAzuriteMicrosoft,
                AzuriteTables.StartAzuriteTables
            }).Wait();

            var serviceCollection = new ServiceCollection();

            _objectHistorySettings = new ObjectHistorySettings(
                AzureStorageConnectionString,
                "Arragro.ObjectHistory.IntegrationTests");

            serviceCollection.AddSingleton(_objectHistorySettings)
                .AddArragroObjectHistoryClient<FakeObjectLogsSecurityAttribute>(_objectHistorySettings)
                .AddSingleton<IStorageHelper, AzureStorageHelper>()
                .AddScoped<ObjectHistoryProcessor>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public void Dispose()
        {
            DockerExtentions.RemoveDockerServicesAsync(true).Wait();
        }
    }
}