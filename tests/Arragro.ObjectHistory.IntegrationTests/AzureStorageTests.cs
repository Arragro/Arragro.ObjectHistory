using Arragro.ObjectHistory.Core;
using Arragro.ObjectHistory.Core.Helpers;
using Arragro.ObjectHistory.Core.Models;
using Azure.Storage.Queues;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Arragro.ObjectHistory.IntegrationTests
{
    public class FakeData
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public List<FakeData> FakeDatas { get;set; }

        public FakeData(int id, string data)
        {
            Id = id;
            Data = data;
        }
    }

    public static class FakeDataContext
    {
        public static List<FakeData> FakeDatas { get; set; }

        private static List<FakeData> GenerateFakeDatas(int amount, bool generateSubFakeDatas = true)
        {
            var random = new Random();
            var fakeDatas = new List<FakeData>();
            for (var i = 0; i < amount; i++)
            {
                fakeDatas.Add(new FakeData(i + 1, $"Test {i + 1}")
                {
                    FakeDatas = generateSubFakeDatas ? GenerateFakeDatas(random.Next(100), false) : new List<FakeData>()
                });
            }

            return fakeDatas;
        }

        public static void InitialiseFakeData()
        {
            FakeDatas = GenerateFakeDatas(100);
        }
    }

    public class AzureStorageTests : BaseTest
    {
        private async Task ProcessQueue(QueueClient queueClient)
        {
            var objectHistoryProcessor = _serviceProvider.GetRequiredService<ObjectHistoryProcessor>();
            do
            {
                var queueMessage = await queueClient.ReceiveMessageAsync();
                if (queueMessage.Value == null)
                    break;
                var objectHistoryMessge = JsonConvert.DeserializeObject<ObjectHistoryMessge>(queueMessage.Value.MessageText);
                await objectHistoryProcessor.ProcessQueueMessageAsync(objectHistoryMessge.Message);
            } while (true);
        }


        [Fact]
        public async Task test_create_process_read_history()
        {
            var objectHistoryClient = _serviceProvider.GetRequiredService<IObjectHistoryClient>();

            FakeDataContext.InitialiseFakeData();

            foreach (var fakeData in FakeDataContext.FakeDatas)
            {
                await objectHistoryClient.QueueObjectHistoryAsync<FakeData>(() => $"{fakeData.Id}", fakeData, "User1");
            }

            var queueClient = new QueueClient(AzureStorageConnectionString, _objectHistorySettings.ObjectQueueName);
            var queueProperties = await queueClient.GetPropertiesAsync();
            // Retrieve the cached approximate message count.
            int cachedMessagesCount = queueProperties.Value.ApproximateMessagesCount;
            Assert.Equal(100, cachedMessagesCount);

            await ProcessQueue(queueClient);

            var global = await objectHistoryClient.GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync();
            Assert.Equal(100, global.Results.Count());

            ObjectHistoryQueryResultContainer entities;
            ObjectHistoryDetailRaw raw;

            foreach (var fakeData in FakeDataContext.FakeDatas)
            {
                entities = await objectHistoryClient.GetObjectHistoryRecordsByObjectNamePartitionKeyAsync($"{typeof(FakeData).FullName}-{fakeData.Id}");
                Assert.Single(entities.Results);

                raw = await objectHistoryClient.GetObjectHistoryDetailRawAsync(entities.Results.First().PartitionKey, entities.Results.First().RowKey);
                Assert.NotNull(raw);
                Assert.True(raw.IsAdd);
            }

            var modifyFakeObject = FakeDataContext.FakeDatas.ElementAt(0).Clone();
            modifyFakeObject.Data = "Test XX";
            await objectHistoryClient.SaveObjectHistoryAsync<FakeData>(() => $"{FakeDataContext.FakeDatas.ElementAt(0).Id}", FakeDataContext.FakeDatas.ElementAt(0), "User1");

            global = await objectHistoryClient.GetObjectHistoryRecordsByApplicationNamePartitionKeyAsync();
            Assert.Equal(101, global.Results.Count());

            entities = await objectHistoryClient.GetObjectHistoryRecordsByObjectNamePartitionKeyAsync($"{typeof(FakeData).FullName}-{modifyFakeObject.Id}");
            Assert.Equal(2, entities.Results.Count());

            raw = await objectHistoryClient.GetObjectHistoryDetailRawAsync(entities.Results.First().PartitionKey, entities.Results.First().RowKey);
            Assert.NotNull(raw);
            Assert.NotNull(raw.OldJson);

            raw = await objectHistoryClient.GetObjectHistoryDetailRawAsync(entities.Results.First().PartitionKey, entities.Results.Last().RowKey);
            Assert.Null(raw.OldJson);
            Assert.True(raw.IsAdd);
        }
    }
}
