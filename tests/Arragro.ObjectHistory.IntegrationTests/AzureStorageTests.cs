using Arragro.Core.Docker;
using Arragro.ObjectHistory.Client.Extensions;
using Arragro.ObjectHistory.Core;
using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Models;
using Azure.Storage.Queues;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Arragro.ObjectHistory.IntegrationTests
{
    public class AzureStorageTests : IDisposable
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ObjectHistorySettings _objectHistorySettings;
        protected const string AzureStorageConnectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
        
        public AzureStorageTests()
        {
            DockerExtentions.StartDockerServicesAsync(new List<Func<DockerClient, Task<ContainerListResponse>>>
            {
                (client) => AzuriteMicrosoftWithTables.StartAzuriteMicrosoft(client, "3.14.2")
            }).Wait();

            var serviceCollection = new ServiceCollection();

            _objectHistorySettings = new ObjectHistorySettings(
                AzureStorageConnectionString,
                "Arragro.ObjectHistory.IntegrationTests");

            QueueAndBlobStorageHelper.EnsureQueueAndContainer(_objectHistorySettings);

            serviceCollection.AddSingleton(_objectHistorySettings)
                .AddArragroObjectHistoryClient(_objectHistorySettings);

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public void Dispose()
        {
            DockerExtentions.RemoveDockerServicesAsync(true).Wait();
        }

        [Fact]
        public async Task test_create_process_read_history()
        {
            var objectHistoryClient = _serviceProvider.GetRequiredService<IObjectHistoryClient>();
            var objectHistoryProcessor = _serviceProvider.GetRequiredService<ObjectHistoryProcessor>();
            var fakeDataContext = new FakeDataContext();

            fakeDataContext.InitialiseFakeData();

            var folder = Guid.NewGuid();

            foreach (var fakeData in fakeDataContext.FakeDatas)
            {
                await objectHistoryClient.QueueObjectHistoryAsync<FakeData>(() => $"{fakeData.Id}", fakeData, "User1", folder);
            }

            var queueClient = new QueueClient(AzureStorageConnectionString, _objectHistorySettings.ObjectQueueName);
            var queueProperties = await queueClient.GetPropertiesAsync();
            // Retrieve the cached approximate message count.
            int cachedMessagesCount = queueProperties.Value.ApproximateMessagesCount;
            Assert.Equal(100, cachedMessagesCount);

            await Utils.ProcessQueue(queueClient, objectHistoryProcessor);

            var global = await objectHistoryClient.GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync(new PagingToken
            {
                PageSize = 150
            });
            Assert.Equal(100, global.Results.Count());

            ObjectHistoryQueryResultContainer entities;
            ObjectHistoryDetailRaw raw;

            foreach (var fakeData in fakeDataContext.FakeDatas)
            {
                entities = await objectHistoryClient.GetObjectHistoryRecordsByObjectNamePartitionKeyAsync($"{typeof(FakeData).FullName}-{fakeData.Id}");
                Assert.Single(entities.Results);

                raw = await objectHistoryClient.GetObjectHistoryDetailRawAsync(entities.Results.First().PartitionKey, entities.Results.First().RowKey);
                Assert.NotNull(raw);
                Assert.True(raw.IsAdd);
                Assert.NotNull(raw.SubFolder);
            }

            var modifyFakeObject = fakeDataContext.FakeDatas.ElementAt(0).Clone();
            modifyFakeObject.Data = "Test XX";
            await objectHistoryClient.SaveObjectHistoryAsync<FakeData>(() => $"{fakeDataContext.FakeDatas.ElementAt(0).Id}", fakeDataContext.FakeDatas.ElementAt(0), "User1", folder);

            global = await objectHistoryClient.GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync();
            //Assert.Equal(101, global.Results.Count());

            entities = await objectHistoryClient.GetObjectHistoryRecordsByObjectNamePartitionKeyAsync($"{typeof(FakeData).FullName}-{modifyFakeObject.Id}");
            Assert.Equal(2, entities.Results.Count());

            raw = await objectHistoryClient.GetObjectHistoryDetailRawAsync(entities.Results.First().PartitionKey, entities.Results.First().RowKey);
            Assert.NotNull(raw);
            Assert.NotNull(raw.OldJson);
            Assert.NotNull(raw.SubFolder);

            raw = await objectHistoryClient.GetObjectHistoryDetailRawAsync(entities.Results.First().PartitionKey, entities.Results.Last().RowKey);
            Assert.Null(raw.OldJson);
            Assert.True(raw.IsAdd);
            Assert.NotNull(raw.SubFolder);

            var removeFakeData = fakeDataContext.FakeDatas.ElementAt(0);
            var removeFakeDataId = removeFakeData.Id;
            await objectHistoryClient.SaveObjectHistoryDeletedAsync(() => $"{removeFakeDataId}", removeFakeData, "User1", folder);
            fakeDataContext.FakeDatas.Remove(removeFakeData);
            var deletedFakeData = await objectHistoryClient.GetLatestObjectHistoryDeletedDetailRawAsync($"{typeof(FakeData).FullName}-{removeFakeDataId}");
            Assert.NotNull(deletedFakeData);
            await objectHistoryClient.DeletedObjectHistoryDeletedByPartitionKey($"{typeof(FakeData).FullName}-{removeFakeDataId}");
            var deletedFakeDataTest = await objectHistoryClient.GetLatestObjectHistoryDeletedDetailRawAsync($"{typeof(FakeData).FullName}-{removeFakeDataId}");
            Assert.Null(deletedFakeDataTest);
            fakeDataContext.FakeDatas = fakeDataContext.FakeDatas.Prepend(JsonConvert.DeserializeObject<FakeData>(deletedFakeData.NewJson)).ToList();
        }
    }
}
