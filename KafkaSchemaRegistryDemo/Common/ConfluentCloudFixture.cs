using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace Common;

public class ConfluentCloudFixture
{
    private static readonly ClientConfig ClientConfig = new()
    {
        BootstrapServers = "pkc-e8mp5.eu-west-1.aws.confluent.cloud:9092",
        SecurityProtocol = Confluent.Kafka.SecurityProtocol.SaslSsl,
        SaslMechanism = Confluent.Kafka.SaslMechanism.Plain,
        SaslUsername = "", // TODO: Add your key here
        SaslPassword = "" // TODO: Add your secret here
    };

    private static readonly ConsumerConfig ConsumerConfig = new(ClientConfig)
    {
        GroupId = "example-one",
        AutoOffsetReset = AutoOffsetReset.Earliest
    };

    private static readonly ProducerConfig ProducerConfig = new(ClientConfig)
    {
    };

    public async Task CreateTopic(string testTopic)
    {
        var adminClientConfig = new AdminClientConfig(ClientConfig);
        using var adminClient = new AdminClientBuilder(adminClientConfig).Build();
        await adminClient.CreateTopicsAsync(new[]
        {
            new TopicSpecification
            {
                Name = testTopic,
                Configs = [],
                NumPartitions = 1
            }
        });
    }

    public async Task DeleteTopic(string testTopic)
    {
        var adminClientConfig = new AdminClientConfig(ClientConfig);
        using var adminClient = new AdminClientBuilder(adminClientConfig).Build();
        await adminClient.DeleteTopicsAsync(new[] { testTopic });
    }

    public IConsumer<string, byte[]> CreateByteArrayConsumer(string testTopic)
    {
        var consumer = new ConsumerBuilder<string, byte[]>(ConsumerConfig).Build();
        consumer.Subscribe(testTopic);
        return consumer;
    }

    public IProducer<string, byte[]> CreateByteArrayProducer()
    {
        var producer = new ProducerBuilder<string, byte[]>(ProducerConfig).Build();
        return producer;
    }
}
