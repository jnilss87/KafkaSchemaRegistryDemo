using Common;
using Confluent.SchemaRegistry;
using Schemas;
using Xunit;
using Xunit.Abstractions;

namespace Example3;

public class Schema(ConfluentCloudFixture fixture, ITestOutputHelper testOutputHelper) : IClassFixture<ConfluentCloudFixture>
{
    [Fact]
    public async Task GetAllSchemaVersions()
    {
        var schemaRegistry = fixture.CreateSchemaRegistryClient();
        var schemaVersions = await schemaRegistry.GetSubjectVersionsAsync(Example3Config.Topic + "-value");
        foreach (var schemaVersion in schemaVersions)
        {
            testOutputHelper.WriteLine(new string('-', 80));
            testOutputHelper.WriteLine($"Schema version: {schemaVersion}");
            var schema = await schemaRegistry.GetRegisteredSchemaAsync(Example3Config.Topic + "-value", schemaVersion);
            testOutputHelper.WriteLine(schema.SchemaString);
        }
    }

    [Fact]
    public async Task DeleteSchemasSoft()
    {
        await fixture.DeleteSubject(Example3Config.Topic + "-value");
    }

    [Fact]
    public async Task DeleteSchemasPermanent()
    {
        await fixture.DeleteSubject(Example3Config.Topic + "-value", permanent: true);
    }
}
