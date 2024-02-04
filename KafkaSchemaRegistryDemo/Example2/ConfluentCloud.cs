using Common;

namespace Example2;

public class ConfluentCloud(ConfluentCloudFixture fixture) : IClassFixture<ConfluentCloudFixture>
{
    [Fact]
    public async Task CreateTestTopic()
    {
        await fixture.CreateTopic(Example2Config.Topic);
    }

    [Fact]
    public async Task DeleteTestTopic()
    {
        await fixture.DeleteTopic(Example2Config.Topic);
    }
}
