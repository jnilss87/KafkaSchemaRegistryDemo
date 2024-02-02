using Confluent.Kafka;

namespace Common;

public class ConfluentCloudFixture
{
    public void CreateTopic(string testTopic)
    {
    }

    public void DeleteTopic(string testTopic)
    {
    }

    public IConsumer<string, string> CreateConsumer(string testTopic)
    {
        var consumer = new ConsumerBuilder<string, string>(new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "test-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        }).Build();
        consumer.Subscribe(testTopic);
        return consumer;
    }

    public IProducer<string, string> CreateProducer()
    {
        var producer = new ProducerBuilder<string, string>(new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        }).Build();
        return producer;
    }
}
