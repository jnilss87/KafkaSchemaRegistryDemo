using Common;
using Xunit;

namespace Example1;

public class ConfluentCloud(ConfluentCloudFixture fixture) : IClassFixture<ConfluentCloudFixture>
{
    [Fact]
    public async Task CreateTestTopic()
    {
        await fixture.CreateTopic("test-topic-example1");
    }

    [Fact]
    public async Task DeleteTestTopic()
    {
        await fixture.DeleteTopic("test-topic-example1");
    }
}
