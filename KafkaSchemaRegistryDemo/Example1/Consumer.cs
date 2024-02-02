using Common;
using Xunit;
using Xunit.Abstractions;

namespace Example1;

public class Consumer(ConfluentCloudFixture fixture, ITestOutputHelper testOutput) : IClassFixture<ConfluentCloudFixture>
{
    [Fact]
    public void Consume()
    {
        var consumer = fixture.CreateConsumer("test-topic");
        while (true)
        {
            var consumeResult = consumer.Consume();
            if (consumeResult.IsPartitionEOF)
            {
                continue;
            }

            testOutput.WriteLine($"Consumed message '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'.");
        }
    }
}
