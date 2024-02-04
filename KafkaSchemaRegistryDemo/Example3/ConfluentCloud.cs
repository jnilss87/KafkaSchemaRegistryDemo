using Common;
using Xunit;

namespace Example3;

public class ConfluentCloud(ConfluentCloudFixture fixture) : IClassFixture<ConfluentCloudFixture>
{
    [Fact]
    public async Task CreateTestTopic()
    {
        await fixture.CreateTopic(Example3Config.Topic);
    }

    [Fact]
    public async Task DeleteTestTopic()
    {
        await fixture.DeleteTopic(Example3Config.Topic);
    }
}
