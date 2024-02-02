using Common;
using Confluent.Kafka;
using Xunit;
using Xunit.Abstractions;

namespace Example1;

public class Producer(ConfluentCloudFixture fixture, ITestOutputHelper testOutputHelper) : IClassFixture<ConfluentCloudFixture>
{

    [Fact]
    public async Task ProduceMessagesToTopic()
    {
        var producer = fixture.CreateProducer();

        for (var i = 0; i < 60; i++)
        {
            var result = await producer.ProduceAsync("test-topic", new Message<string, string>
            {
                Key = i.ToString(),
                Value = $"test message {i}"
            });
            testOutputHelper.WriteLine($"Delivered '{result.Value}' to '{result.TopicPartitionOffset}'");
            await Task.Delay(5000);
        }
    }
}
