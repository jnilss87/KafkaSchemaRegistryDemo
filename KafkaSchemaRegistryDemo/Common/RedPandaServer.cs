using Confluent.Kafka;
using Confluent.Kafka.Admin;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Testcontainers.Redpanda;

namespace Common;

// ReSharper disable once ClassNeverInstantiated.Global
public class RedPandaServer : IAsyncDisposable
{
    private const int CreateTopicTimeoutMilliseconds = 5000;
    private readonly RedpandaContainer _kafkaContainer;
    private IContainer? _redPandaConsoleContainer = null;
    private string? _bootStrapServers;
    private string? _schemaRegistryAddress;

    private IAdminClient? _admin;

    public RedPandaServer()
    {
        var kafkaBuilder = new RedpandaBuilder()
            .WithName($"RedPanda-{Guid.NewGuid():N}");

        _kafkaContainer = kafkaBuilder.Build();
    }

    public async Task InitializeAsync()
    {
        if (IsRunning)
            return;

        await _kafkaContainer.StartAsync();
        _bootStrapServers = _kafkaContainer.GetBootstrapAddress();
        _schemaRegistryAddress = _kafkaContainer.GetSchemaRegistryAddress();
        _admin = new AdminClientBuilder(new ClientConfig()
        {
            BootstrapServers = _bootStrapServers
        }).Build();
        _redPandaConsoleContainer = new ContainerBuilder()
            .WithImage("docker.redpanda.com/redpandadata/console:v2.4.1")
            .WithName($"RedPandaConsole-{Guid.NewGuid():N}")
            .WithEntrypoint("/bin/sh", "-c")
            .WithCommand("""
                         printenv CONSOLE_CONFIG_FILE && echo "$CONSOLE_CONFIG_FILE" > "/tmp/config.yml" && /app/console
                         """)
            .WithEnvironment("CONFIG_FILEPATH", "/tmp/config.yml")
            .WithEnvironment("CONSOLE_CONFIG_FILE",
                $"""
                 kafka:
                     brokers:
                      - {ReplaceLocalhost(_bootStrapServers)}
                     tls:
                         enabled: false
                     sasl:
                         enabled: false
                     schemaRegistry:
                         enabled: true
                         urls: ["{ReplaceLocalhost(_schemaRegistryAddress)}"]
                         tls:
                              enabled: false
                     protobuf:
                         enabled: true
                         schemaRegistry:
                          enabled: true
                          refreshInterval: 5m
                 """)
            .WithPortBinding(8082, true)
            .Build();
        await _redPandaConsoleContainer.StartAsync();
    }

    public bool IsRunning => _kafkaContainer is { State: TestcontainersStates.Running };
    public bool IsReady => !string.IsNullOrWhiteSpace(_bootStrapServers);

    public string GetBootstrapServers() => _bootStrapServers ?? string.Empty;

    public string GetSchemaRegistryUrl() => _schemaRegistryAddress ?? string.Empty;

    public string ReplaceLocalhost(string input) => input.Replace("localhost", "host.docker.internal").Replace("127.0.0.1", "host.docker.internal");

    public async Task<string> CreateTopic(string fullTopicName)
    {
        var metaData = _admin?.GetMetadata(TimeSpan.FromSeconds(5));
        var topicInfo = metaData?.Topics.FirstOrDefault(tp => string.Equals(fullTopicName, tp.Topic, StringComparison.OrdinalIgnoreCase));

        if (topicInfo == null && _admin != null)
        {
            var t = new TopicSpecification
            {
                Name = fullTopicName,
                NumPartitions = 1
            };
            try
            {
                await _admin.CreateTopicsAsync(new[] { t },
                        new CreateTopicsOptions { OperationTimeout = TimeSpan.FromMilliseconds(CreateTopicTimeoutMilliseconds) })
                    .ConfigureAwait(false);
            }
            catch (CreateTopicsException e)
            {
                Console.WriteLine($"An error occurred creating topic {e.Results[0].Topic}: {e.Results[0].Error.Reason}");
                throw;
            }
        }

        return fullTopicName;
    }


    public async ValueTask DisposeAsync()
    {
        _admin?.Dispose();
        await _kafkaContainer.StopAsync();
    }
}
