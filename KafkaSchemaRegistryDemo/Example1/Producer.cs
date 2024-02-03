using System.Text;
using Common;
using Confluent.Kafka;
using Example1.Models;
using Xunit;
using Xunit.Abstractions;

namespace Example1;

public class Producer(ConfluentCloudFixture fixture, ITestOutputHelper testOutputHelper) : IClassFixture<ConfluentCloudFixture>
{
    [Fact]
    public async Task ProduceRawCSharpMessages()
    {
        IProducer<string, byte[]> producer = fixture.CreateByteArrayProducer();
        var autoFixture = new AutoFixture.Fixture();

        for (var i = 0; i < 60; i++)
        {
            // Create a new chat message
            var chatMessage = autoFixture.Build<ChatMessage>();

            var message = new Message<string, byte[]>
            {
                Key = i.ToString(),
                Value = ProtobufSerializer.SerializeCSharpToByteArray(chatMessage)
            };

            var result = await producer.ProduceAsync("test-topic-example1", message);
            testOutputHelper.WriteLine(result.Status != PersistenceStatus.Persisted
                ? $"Failed to deliver message: {result.Status}"
                : $"Delivered '{result.Key}' to '{result.TopicPartitionOffset}'");

            await Task.Delay(5000);
        }
    }
}
