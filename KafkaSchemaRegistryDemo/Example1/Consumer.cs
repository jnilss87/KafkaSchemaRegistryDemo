using Common;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry.Serdes;
using Example1.Models;
using Xunit;
using Xunit.Abstractions;

namespace Example1;

public class Consumer(ConfluentCloudFixture fixture, ITestOutputHelper testOutput) : IClassFixture<ConfluentCloudFixture>
{
    [Fact]
    public void ConsumeRawCSharpMessages()
    {
        var consumer = fixture.CreateConsumer(Example1Config.Topic, new ChatMessageDeserializer());
        while (true)
        {
            ConsumeResult<string, ChatMessage>? consumeResult = null;
            try
            {
                consumeResult = consumer.Consume();
                if (consumeResult.IsPartitionEOF)
                {
                    continue;
                }

                var message = consumeResult.Message.Value;
                if (message == null)
                {
                    testOutput.WriteLine("Failed to deserialize message");
                    continue;
                }

                testOutput.WriteLine($"{message.Timestamp}: User '{message.User.Name}' sent message '{message.Content}'");
            }
            catch (Exception e)
            {
                testOutput.WriteLine($"Failed to deserialize message: {e.Message}");
                // The message failed to deserialize so we should indicate to the broker that we have not processed the message and it should be redelivered to another consumer
                if (consumeResult != null)
                {
                    consumer.Commit(consumeResult);
                }

                break;
            }
        }
    }

    [Fact]
    public void ConsumeProtobufMessages()
    {
        // Create a ProtobufDeserializer with the fetched schema
        var protobufDeserializer = new ProtobufDeserializer<Chat.V1.ChatMessage>();

        // Create the consumer with the ProtobufDeserializer
        var consumer = fixture.CreateConsumer(Example1Config.Topic, protobufDeserializer.AsSyncOverAsync());
        while (true)
        {
            ConsumeResult<string, Chat.V1.ChatMessage>? consumeResult = null;
            try
            {
                consumeResult = consumer.Consume();
                if (consumeResult.IsPartitionEOF)
                {
                    continue;
                }

                var message = consumeResult.Message.Value;
                if (message == null)
                {
                    testOutput.WriteLine("Failed to deserialize message");
                    continue;
                }

                testOutput.WriteLine($"{message.Timestamp}: User '{message.User.Name}' sent message '{message.Content}'");
            }
            catch (Exception e)
            {
                testOutput.WriteLine($"Failed to deserialize message: {e.Message}");
                // The message failed to deserialize so we should indicate to the broker that we have not processed the message and it should be redelivered to another consumer
                consumer.Commit(consumeResult);
                break;
            }
        }
    }
}
