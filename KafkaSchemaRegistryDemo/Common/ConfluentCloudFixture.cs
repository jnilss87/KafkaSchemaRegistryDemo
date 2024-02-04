using System.Net.Http.Headers;
using System.Text;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Confluent.SchemaRegistry;

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
        GroupId = "consumer-group",
        AutoOffsetReset = AutoOffsetReset.Earliest
    };

    private static readonly ProducerConfig ProducerConfig = new(ClientConfig)
    {
    };

    private static SchemaRegistryConfig SchemaRegistryConfig()
    {
        var config = new SchemaRegistryConfig
        {
            BasicAuthUserInfo = "", // TODO: Add the schema registry API key and secret <api-key>:<api-secret>
            Url = "" // TODO: Add the schema registry URL
        };
        config.Set("auto.register.schemas",
            "false"); // "true" or "false" (defaults to "true"), used to enable or disable automatic registration of schemas
        return config;
    }

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

    public IConsumer<string, T> CreateConsumer<T>(string testTopic, IDeserializer<T> serializer)
    {
        var consumer = new ConsumerBuilder<string, T>(ConsumerConfig).SetValueDeserializer(serializer).Build();
        consumer.Subscribe(testTopic);
        return consumer;
    }

    public IProducer<string, T> CreateProducer<T>(IAsyncSerializer<T> serializer)
    {
        var producer = new ProducerBuilder<string, T>(ProducerConfig)
            .SetValueSerializer(serializer)
            .Build();
        return producer;
    }

    public CachedSchemaRegistryClient CreateSchemaRegistryClient()
    {
        return new(SchemaRegistryConfig());
    }

    public async Task DeleteSubject(string subject, bool permanent = false)
    {
        var httpClient = GetSchemaRegistryHttpClient();
        var responseMessage = await httpClient.DeleteAsync($"/subjects/{subject}?permanent={permanent}");

        // Exception if not successful
        if (!responseMessage.IsSuccessStatusCode)
        {
            var responseContent = await responseMessage.Content.ReadAsStringAsync();
            throw new Exception(
                $"Failed to delete schema for subject {subject}. Status code: {responseMessage.StatusCode}. Response: {responseContent}");
        }
    }

    public HttpClient GetSchemaRegistryHttpClient()
    {
        var client = new HttpClient();
        var schemaRegistryConfig = SchemaRegistryConfig();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes(schemaRegistryConfig.BasicAuthUserInfo)));
        client.BaseAddress = new Uri(schemaRegistryConfig.Url);

        return client;
    }
}
