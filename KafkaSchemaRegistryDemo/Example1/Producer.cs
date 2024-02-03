using AutoFixture;
using Common;
using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Xunit;
using Xunit.Abstractions;

namespace Example1;

public class Producer(ConfluentCloudFixture fixture, ITestOutputHelper testOutputHelper) : IClassFixture<ConfluentCloudFixture>
{
    [Fact]
    public async Task ProduceRawCSharpMessages()
    {
        var producer = fixture.CreateProducer(new ByteArraySerializer<Models.ChatMessage>());
        var autoFixture = new AutoFixture.Fixture();

        for (var i = 0; i < 60; i++)
        {
            // Create a new chat message
            var chatMessage = autoFixture.Create<Models.ChatMessage>();

            var message = new Message<string, Models.ChatMessage>
            {
                Key = i.ToString(),
                Value = chatMessage
            };

            var result = await producer.ProduceAsync(Example1Config.Topic, message);
            testOutputHelper.WriteLine(result.Status != PersistenceStatus.Persisted
                ? $"Failed to deliver message: {result.Status}"
                : $"Delivered '{result.Key}' to '{result.TopicPartitionOffset}'");

            await Task.Delay(5000);
        }
    }

    [Fact]
    public async Task ProduceProtobufMessages()
    {
        var schmeaRegistry = fixture.CreateSchemaRegistryClient();
        var protobufSerializer = new ProtobufSerializer<Chat.V1.ChatMessage>(schmeaRegistry);
        var producer = fixture.CreateProducer(protobufSerializer);
        var autoFixture = new AutoFixture.Fixture();

        for (var i = 0; i < 60; i++)
        {
            // Create a new chat message
            var chatMessage = autoFixture.Build<Chat.V1.ChatMessage>()
                // Give the user a "real" name
                .With(x => x.User, autoFixture.Build<Chat.V1.User>()
                    .With(x => x.Name, autoFixture.Create<string>())
                    .Create())
                .Create<Chat.V1.ChatMessage>();

            var message = new Message<string, Chat.V1.ChatMessage>
            {
                Key = i.ToString(),
                Value = chatMessage
            };

            var result = await producer.ProduceAsync(Example1Config.Topic, message);
            testOutputHelper.WriteLine(result.Status != PersistenceStatus.Persisted
                ? $"Failed to deliver message: {result.Status}"
                : $"Delivered '{result.Key}' to '{result.TopicPartitionOffset}'");

            await Task.Delay(5000);
        }
    }
}
