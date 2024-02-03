using Common;
using Example1.Models;
using Xunit;
using Xunit.Abstractions;

namespace Example1;

public class Consumer(ConfluentCloudFixture fixture, ITestOutputHelper testOutput) : IClassFixture<ConfluentCloudFixture>
{
    [Fact]
    public void ConsumeRawCSharpMessages()
    {
        var consumer = fixture.CreateByteArrayConsumer("test-topic-example1");
        while (true)
        {
            var consumeResult = consumer.Consume();
            if (consumeResult.IsPartitionEOF)
            {
                continue;
            }

            try
            {
                var message = ProtobufSerializer.DeserializeCSharpFromByteArray<ChatMessage>(consumeResult.Message.Value);
                if (message == null)
                {
                    testOutput.WriteLine("Failed to deserialize message");
                    continue;
                }
                testOutput.WriteLine($"Consumed message '{message.Message}' from '{message.User.Name}'");
            }
            catch (Exception e)
            {
                testOutput.WriteLine($"Failed to deserialize message: {e.Message}");
            }
        }
    }
}
