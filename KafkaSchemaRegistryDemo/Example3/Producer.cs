using AutoFixture;
using Chat.V5;
using Common;
using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Xunit;
using Xunit.Abstractions;

namespace Example3;

public class Producer(ConfluentCloudFixture fixture, ITestOutputHelper testOutputHelper) : IClassFixture<ConfluentCloudFixture>
{
    [Fact]
    public async Task ProduceNewChannelMessage()
    {
        var schemaRegistry = fixture.CreateSchemaRegistryClient();
        var protobufSerializer = new ProtobufSerializer<ChatServer>(schemaRegistry);
        var producer = fixture.CreateProducer(protobufSerializer);

        // create a server message
        var serverMessage = new Server
        {
            Id = "1",
            Name = "KafkaChat",
        };
        serverMessage.Channels.Add(new Channel { Id = "1", Name = "General" });

        var chatServerMessage = new ChatServer
        {
            Server = serverMessage
        };

        var message = new Message<string, ChatServer> { Key = "Server" + DateTime.Now, Value = chatServerMessage };
        var result = await producer.ProduceAsync(Example3Config.Topic, message);
        testOutputHelper.WriteLine(result.Status != PersistenceStatus.Persisted
            ? $"Failed to deliver message: {result.Status}"
            : $"Delivered '{result.Key}' to '{result.TopicPartitionOffset}'");
    }

    [Fact]
    public async Task ProduceNewUserMessage()
    {
        var schemaRegistry = fixture.CreateSchemaRegistryClient();
        var protobufSerializer = new ProtobufSerializer<ChatServer>(schemaRegistry);
        var producer = fixture.CreateProducer(protobufSerializer);

        var chatServerMessage = new User
        {
            Id = "1",
            Name = new Bogus.DataSets.Name().FullName(),
        };
        var message = new Message<string, ChatServer>
            { Key = "ServerPermission" + DateTime.Now, Value = new ChatServer { User = chatServerMessage } };
        var result = await producer.ProduceAsync(Example3Config.Topic, message);
        testOutputHelper.WriteLine(result.Status != PersistenceStatus.Persisted
            ? $"Failed to deliver message: {result.Status}"
            : $"Delivered '{result.Key}' to '{result.TopicPartitionOffset}'");
    }

    [Fact]
    public async Task ProduceNewServerPermissionMessage()
    {
        var schemaRegistry = fixture.CreateSchemaRegistryClient();
        var protobufSerializer = new ProtobufSerializer<ChatServer>(schemaRegistry);
        var producer = fixture.CreateProducer(protobufSerializer);
        var autoFixture = new Fixture();

        var chatServerMessage = new ChatServer
        {
            ServerPermission = autoFixture.Create<ServerPermission>()
        };
        var message = new Message<string, ChatServer> { Key = "ServerPermission" + DateTime.Now, Value = chatServerMessage };
        var result = await producer.ProduceAsync(Example3Config.Topic, message);
        testOutputHelper.WriteLine(result.Status != PersistenceStatus.Persisted
            ? $"Failed to deliver message: {result.Status}"
            : $"Delivered '{result.Key}' to '{result.TopicPartitionOffset}'");
    }

    [Fact]
    public async Task ProduceNewChatMessageMessage()
    {
        var schemaRegistry = fixture.CreateSchemaRegistryClient();
        var protobufSerializer = new ProtobufSerializer<ChatServer>(schemaRegistry);
        var producer = fixture.CreateProducer(protobufSerializer);
        var autoFixture = new Fixture();

        var chatServerMessage = new ChatServer
        {
            ChatMessage = new ChatMessage
            {
                ChannelId = "1",
                Content = new Bogus.DataSets.Lorem().Sentence(),
                Timestamp = DateTime.Now.Ticks,
                UserId = "1"
            }
        };

        var message = new Message<string, ChatServer> { Key = "ServerPermission" + DateTime.Now, Value = chatServerMessage };
        var result = await producer.ProduceAsync(Example3Config.Topic, message);
        testOutputHelper.WriteLine(result.Status != PersistenceStatus.Persisted
            ? $"Failed to deliver message: {result.Status}"
            : $"Delivered '{result.Key}' to '{result.TopicPartitionOffset}'");
    }
}
