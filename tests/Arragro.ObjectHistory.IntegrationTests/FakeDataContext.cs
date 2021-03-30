using System;
using System.Collections.Generic;

namespace Arragro.ObjectHistory.IntegrationTests
{
    public class FakeData
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public List<FakeData> FakeDatas { get; set; }

        public FakeData(int id, string data)
        {
            Id = id;
            Data = data;
        }
    }

    public class FakeDataContext
    {
        public List<FakeData> FakeDatas { get; set; }

        private List<FakeData> GenerateFakeDatas(int amount, bool generateSubFakeDatas = true)
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

        public void InitialiseFakeData()
        {
            FakeDatas = GenerateFakeDatas(100);
        }
    }
}
