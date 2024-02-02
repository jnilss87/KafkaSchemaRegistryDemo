using Common;
using Xunit;

namespace Example1;

public class ConfluentCloud(ConfluentCloudFixture fixture) : IClassFixture<ConfluentCloudFixture>
{
    [Fact]
    public void CreateTestTopic()
    {
        fixture.CreateTopic("test-topic");
    }

    [Fact]
    public void DeleteTestTopic()
    {
        fixture.DeleteTopic("test-topic");
    }
}
