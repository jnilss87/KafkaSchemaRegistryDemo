using Chat.V5;
using Common;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry.Serdes;
using Xunit;
using Xunit.Abstractions;

namespace Example3;

public class Consumer(ConfluentCloudFixture fixture, ITestOutputHelper testOutput) : IClassFixture<ConfluentCloudFixture>
{
    [Fact]
    public void ConsumeProtobufMessages()
    {
        // Create a ProtobufDeserializer with the fetched schema
        var protobufDeserializer = new ProtobufDeserializer<ChatServer>();

        // Create the consumer with the ProtobufDeserializer
        var consumer = fixture.CreateConsumer(Example3Config.Topic, protobufDeserializer.AsSyncOverAsync());

        // Store the servers, users and channels in memory
        var users = new Dictionary<string, User>();
        var servers = new Dictionary<string, Server>();
        var channels = new Dictionary<string, (string, Channel)>();

        while (true)
        {
            ConsumeResult<string, ChatServer>? consumeResult = null;
            try
            {
                consumeResult = consumer.Consume();
                if (consumeResult.IsPartitionEOF)
                {
                    continue;
                }

                var message = consumeResult.Message.Value;
                switch (message.OneOfCase)
                {
                    case ChatServer.OneOfOneofCase.None:
                        break;
                    case ChatServer.OneOfOneofCase.ChatMessage:
                    {
                        channels.TryGetValue(message.ChatMessage.ChannelId, out var outValue);
                        if (outValue.Item1 != null)
                        {
                            var (serverId, channel) = outValue;
                            servers.TryGetValue(serverId, out var server);
                            users.TryGetValue(message.ChatMessage.UserId, out var user);
                            // Write a header with the server and channel
                            testOutput.WriteLine($"---Server '{server?.Name}' Channel '{channel?.Name}' User '{user?.Name}' ---");
                        }

                        testOutput.WriteLine(
                            $"{new DateTime(message.ChatMessage.Timestamp)}: '{message.ChatMessage.Content}'");
                    }
                        break;
                    case ChatServer.OneOfOneofCase.ServerPermission:
                        // ignore for now
                        break;
                    case ChatServer.OneOfOneofCase.Server:
                        servers[message.Server.Id] = message.Server;
                        testOutput.WriteLine($"Server '{message.Server.Name}' created");
                        foreach (var channel in message.Server.Channels)
                        {
                            channels[channel.Id] = (message.Server.Id, channel);
                            testOutput.WriteLine($"Channel '{channel.Name}' created on server '{message.Server.Name}'");
                        }

                        break;
                    case ChatServer.OneOfOneofCase.User:
                        users[message.User.Id] = message.User;
                        testOutput.WriteLine($"User '{message.User.Name}' created");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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
}
