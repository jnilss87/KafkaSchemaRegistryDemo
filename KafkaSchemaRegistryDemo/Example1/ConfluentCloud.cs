using Common;
using Xunit;

namespace Example1;

public class ConfluentCloud(ConfluentCloudFixture fixture) : IClassFixture<ConfluentCloudFixture>
{
    [Fact]
    public async Task CreateTestTopic()
    {
        await fixture.CreateTopic(Example1Config.Topic);
    }

    [Fact]
    public async Task DeleteTestTopic()
    {
        await fixture.DeleteTopic(Example1Config.Topic);
    }
}
