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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace Arragro.ObjectHistory.IntegrationTests
{
    public class EFCoreStorageTests : IDisposable
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ObjectHistorySettings _objectHistorySettings;
        protected const string AzureStorageConnectionString = "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

        private static string fileName = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\arragro-object-history.db";
        private static string sqliteConnectionString = $"Data Source={fileName};";

        public EFCoreStorageTests(StorageType storageType)
        {
            switch(storageType)
            {
                case StorageType.SqlServer:
                    DockerExtentions.StartDockerServicesAsync(new List<Func<DockerClient, Task<ContainerListResponse>>>
                    {
                        AzuriteMicrosoft.StartAzuriteMicrosoft,
                        SqlServer.StartSqlServer
                    }).Wait();
                    break;
                case StorageType.Postgres:
                    DockerExtentions.StartDockerServicesAsync(new List<Func<DockerClient, Task<ContainerListResponse>>>
                    {
                        AzuriteMicrosoft.StartAzuriteMicrosoft,
                        Postgres.StartPostgres
                    }).Wait();
                    break;
                case StorageType.Sqlite:
                    DockerExtentions.StartDockerServicesAsync(new List<Func<DockerClient, Task<ContainerListResponse>>>
                    {
                        AzuriteMicrosoft.StartAzuriteMicrosoft
                    }).Wait();
                    break;

            }

            if (File.Exists(fileName))
                File.Delete(fileName);

            var postgresConnectionString = "host=localhost;port=5432;database=arragro-object-history;user id=postgres;password=password1;";
            var sqlServerConnectionString = "Server=127.0.0.1,1435;Database=arragro-object-history;User Id=sa;Password=P@ssword123;";

            var connectionString = "";
            switch (storageType)
            {
                case StorageType.SqlServer:
                    connectionString = sqlServerConnectionString;
                    break;
                case StorageType.Postgres:
                    connectionString = postgresConnectionString;
                    break;
                case StorageType.Sqlite:
                    connectionString = sqliteConnectionString;
                    break;
            }

            var serviceCollection = new ServiceCollection();

            _objectHistorySettings = new ObjectHistorySettings(
                AzureStorageConnectionString,
                connectionString,
                "Arragro.ObjectHistory.IntegrationTests",
                storageType);

            serviceCollection.AddSingleton(_objectHistorySettings)
                .AddArragroObjectHistoryClient(_objectHistorySettings);

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public void Dispose()
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            DockerExtentions.RemoveDockerServicesAsync(true).Wait();
        }

        protected async Task CreateProcessAndRead()
        {
            var objectHistoryClient = _serviceProvider.GetRequiredService<IObjectHistoryClient>();
            var objectHistoryProcessor = _serviceProvider.GetRequiredService<ObjectHistoryProcessor>();
            var fakeDataContext = new FakeDataContext();

            fakeDataContext.InitialiseFakeData();

            var folder = Guid.NewGuid();

            foreach (var fakeData in fakeDataContext.FakeDatas)
            {
                await objectHistoryClient.QueueObjectHistoryAsync<FakeData>(() => $"{fakeData.Id}", fakeData, "User1", folder, $"meta-{fakeData.Id}");
            }

            var queueClient = new QueueClient(AzureStorageConnectionString, _objectHistorySettings.ObjectQueueName);
            var queueProperties = await queueClient.GetPropertiesAsync();
            // Retrieve the cached approximate message count.
            int cachedMessagesCount = queueProperties.Value.ApproximateMessagesCount;
            Assert.Equal(100, cachedMessagesCount);

            await Utils.ProcessQueue(queueClient, objectHistoryProcessor);

            var pagingToken = new PagingToken(1, 100);
            var global = await objectHistoryClient.GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync(pagingToken);
            Assert.Equal(100, global.Results.Count());

            ObjectHistoryQueryResultContainer entities;
            ObjectHistoryDetailRaw raw;

            foreach (var fakeData in fakeDataContext.FakeDatas)
            {
                entities = await objectHistoryClient.GetObjectHistoryRecordsByObjectNamePartitionKeyAsync($"{typeof(FakeData).FullName}-{fakeData.Id}");
                Assert.Single(entities.Results);
                Assert.NotNull(entities.Results.First().Metadata);

                raw = await objectHistoryClient.GetObjectHistoryDetailRawAsync(entities.Results.First().PartitionKey, entities.Results.First().RowKey);
                Assert.NotNull(raw);
                Assert.True(raw.IsAdd);
                Assert.NotNull(raw.SubFolder);
                Assert.NotNull(raw.Metadata);
                Assert.Equal(1, raw.Version);
            }

            var modifyFakeObject = fakeDataContext.FakeDatas.ElementAt(0).Clone();
            modifyFakeObject.Data = "Test XX";
            await objectHistoryClient.SaveObjectHistoryAsync<FakeData>(() => $"{fakeDataContext.FakeDatas.ElementAt(0).Id}", modifyFakeObject, "User1", folder);

            global = await objectHistoryClient.GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync();
            //Assert.Equal(101, global.Results.Count());

            entities = await objectHistoryClient.GetObjectHistoryRecordsByObjectNamePartitionKeyAsync($"{typeof(FakeData).FullName}-{modifyFakeObject.Id}");
            Assert.Equal(2, entities.Results.Count());

            raw = await objectHistoryClient.GetObjectHistoryDetailRawAsync(entities.Results.First().PartitionKey, entities.Results.First().RowKey);
            Assert.NotNull(raw);
            Assert.NotNull(raw.OldJson);
            Assert.NotNull(raw.SubFolder);
            Assert.Null(raw.Metadata);
            Assert.Equal(2, raw.Version);

            raw = await objectHistoryClient.GetObjectHistoryDetailRawAsync(entities.Results.First().PartitionKey, entities.Results.Last().RowKey);
            Assert.Null(raw.OldJson);
            Assert.True(raw.IsAdd);
            Assert.NotNull(raw.SubFolder);
            Assert.NotNull(raw.Metadata);
            Assert.Equal(1, raw.Version);

            modifyFakeObject = fakeDataContext.FakeDatas.ElementAt(0).Clone();
            modifyFakeObject.Data = "Test XXX";
            await objectHistoryClient.QueueObjectHistoryAsync<FakeData>(() => $"{fakeDataContext.FakeDatas.ElementAt(0).Id}", modifyFakeObject, "User1", folder, $"meta-{fakeDataContext.FakeDatas.ElementAt(0).Id}");
            await Utils.ProcessQueue(queueClient, objectHistoryProcessor);

            global = await objectHistoryClient.GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync();
            //Assert.Equal(101, global.Results.Count());

            entities = await objectHistoryClient.GetObjectHistoryRecordsByObjectNamePartitionKeyAsync($"{typeof(FakeData).FullName}-{modifyFakeObject.Id}");
            Assert.Equal(3, entities.Results.Count());

            raw = await objectHistoryClient.GetObjectHistoryDetailRawAsync(entities.Results.First().PartitionKey, entities.Results.First().RowKey);
            Assert.NotNull(raw);
            Assert.NotNull(raw.OldJson);
            Assert.NotNull(raw.SubFolder);
            Assert.NotNull(raw.Metadata);
            Assert.Equal(3, raw.Version);

            raw = await objectHistoryClient.GetObjectHistoryDetailRawAsync(entities.Results.First().PartitionKey, entities.Results.Last().RowKey);
            Assert.Null(raw.OldJson);
            Assert.True(raw.IsAdd);
            Assert.NotNull(raw.SubFolder);
            Assert.NotNull(raw.Metadata);
            Assert.Equal(1, raw.Version);

            await objectHistoryClient.QueueObjectHistoryAsync<FakeData>(() => $"{fakeDataContext.FakeDatas.ElementAt(0).Id}", modifyFakeObject, "User1", folder);
            await Utils.ProcessQueue(queueClient, objectHistoryProcessor);

            entities = await objectHistoryClient.GetObjectHistoryRecordsByObjectNamePartitionKeyAsync($"{typeof(FakeData).FullName}-{modifyFakeObject.Id}");
            Assert.Equal(3, entities.Results.Count());

            var removeFakeData = fakeDataContext.FakeDatas.ElementAt(0);
            var removeFakeDataId = removeFakeData.Id;
            await objectHistoryClient.SaveObjectHistoryDeletedAsync(() => $"{removeFakeDataId}", removeFakeData, "User1", folder);
            fakeDataContext.FakeDatas.Remove(removeFakeData);
            var deletedFakeData = await objectHistoryClient.GetLatestObjectHistoryDeletedDetailRawAsync($"{typeof(FakeData).FullName}-{removeFakeDataId}");
            Assert.NotNull(deletedFakeData);
            using (var scope = _serviceProvider.CreateScope())
            {
                var tempObjectHistoryClient = scope.ServiceProvider.GetRequiredService<IObjectHistoryClient>();
                await tempObjectHistoryClient.DeletedObjectHistoryDeletedByPartitionKey($"{typeof(FakeData).FullName}-{removeFakeDataId}");
            }
            var deletedFakeDataTest = await objectHistoryClient.GetLatestObjectHistoryDeletedDetailRawAsync($"{typeof(FakeData).FullName}-{removeFakeDataId}");
            Assert.Null(deletedFakeDataTest);
            fakeDataContext.FakeDatas = fakeDataContext.FakeDatas.Prepend(JsonConvert.DeserializeObject<FakeData>(deletedFakeData.NewJson)).ToList();
        }
    }

    public class EFCoreStorageSqliteTests : EFCoreStorageTests
    {
        public EFCoreStorageSqliteTests() : base(StorageType.Sqlite)
        {
        }

        [Fact]
        public async Task test_create_process_read_history()
        {
            await CreateProcessAndRead();
        }
    }

    public class EFCoreStorageSqlServerTests : EFCoreStorageTests
    {
        public EFCoreStorageSqlServerTests() : base(StorageType.SqlServer)
        {
        }

        [Fact]
        public async Task test_create_process_read_history()
        {
            await CreateProcessAndRead();
        }
    }

    public class EFCoreStoragePostgresTests : EFCoreStorageTests
    {
        public EFCoreStoragePostgresTests() : base(StorageType.Postgres)
        {
        }

        [Fact]
        public async Task test_create_process_read_history()
        {
            await CreateProcessAndRead();
        }
    }
}
