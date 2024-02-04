using Common;
using Confluent.SchemaRegistry;
using Schemas;
using Xunit;
using Xunit.Abstractions;

namespace Example2;

public class Schema(ConfluentCloudFixture fixture, ITestOutputHelper testOutputHelper) : IClassFixture<ConfluentCloudFixture>
{
    [Fact]
    public async Task GetCompatibilitySettingsForSubject()
    {
        var schemaRegistry = fixture.CreateSchemaRegistryClient();
        var compatibility = await schemaRegistry.GetCompatibilityAsync(Example2Config.Topic + "-value");
        testOutputHelper.WriteLine($"Compatibility: {compatibility}");
    }

    [Fact]
    public async Task RegisterSchemaV1() => await RegisterSchema(1);

    [Fact]
    public async Task RegisterSchemaV2() => await RegisterSchema(2);

    [Fact]
    public async Task RegisterSchemaV3() => await RegisterSchema(3);

    [Fact]
    public async Task RegisterSchemaV4() => await RegisterSchema(4);


    private async Task RegisterSchema(int version)
    {
        var schemaRegistry = fixture.CreateSchemaRegistryClient();

        // Get the schema for the chat message, there is probably a better way to do this but this is the easiest right now
        var proto = SchemaHelper.GetSchemaChatMessage(version);

        // Construct a schema object, clarify that it is a protobuf schema
        var schema = new Confluent.SchemaRegistry.Schema(proto, SchemaType.Protobuf);

        // Check if the schema is compatible with the latest version
        if (version > 1)
        {
            var result = await schemaRegistry.IsCompatibleAsync($"{Example2Config.Topic}-value", schema);
            if (!result)
            {
                throw new Exception("Schema is not compatible with the latest version");
            }
        }

        // POST /subjects/{subject}/versions?normalize=false with the schema as the body
        // This will register the schema on the subject using TopicNameStrategy for topic "test-topic-example1".
        // The -value suffix will make it the value schema
        var schemaRegistryResult = await schemaRegistry.RegisterSchemaAsync($"{Example2Config.Topic}-value", schema);

        testOutputHelper.WriteLine($"Registered schema with id {schemaRegistryResult}");
    }

    [Fact]
    public async Task GetAllSchemaVersions()
    {
        var schemaRegistry = fixture.CreateSchemaRegistryClient();
        var schemaVersions = await schemaRegistry.GetSubjectVersionsAsync(Example2Config.Topic + "-value");
        foreach (var schemaVersion in schemaVersions)
        {
            testOutputHelper.WriteLine(new string('-', 80));
            testOutputHelper.WriteLine($"Schema version: {schemaVersion}");
            var schema = await schemaRegistry.GetRegisteredSchemaAsync(Example2Config.Topic + "-value", schemaVersion);
            testOutputHelper.WriteLine(schema.SchemaString);
        }
    }

    [Fact]
    public async Task CheckCompatibilityV1() => await CheckCompatibility(1);

    [Fact]
    public async Task CheckCompatibilityV2() => await CheckCompatibility(2);

    [Fact]
    public async Task CheckCompatibilityV3() => await CheckCompatibility(3);

    [Fact]
    public async Task CheckCompatibilityV3Incompatible() => await CheckCompatibility(3, true);

    [Fact]
    public async Task CheckCompatibilityV4() => await CheckCompatibility(4);

    private async Task CheckCompatibility(int version, bool incompatible = false)
    {
        var schemaRegistry = fixture.CreateSchemaRegistryClient();
        var nextSchema = SchemaHelper.GetSchemaChatMessage(version, incompatible);
        var schema = new Confluent.SchemaRegistry.Schema(nextSchema, SchemaType.Protobuf);
        var result = await schemaRegistry.IsCompatibleAsync($"{Example2Config.Topic}-value", schema);
        testOutputHelper.WriteLine($"Compatibility: {result}");
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteSchemasSoft()
    {
        await fixture.DeleteSubject(Example2Config.Topic + "-value");
    }

    [Fact]
    public async Task DeleteSchemasPermanent()
    {
        await fixture.DeleteSubject(Example2Config.Topic + "-value", permanent: true);
    }


    [Fact]
    public void PrintSchemaV1()
    {
        // Print the schema for the chat message
        var schema = SchemaHelper.GetSchemaChatMessageV1();
        testOutputHelper.WriteLine(schema);
    }

    [Fact]
    public void PrintSchemaV2()
    {
        // Print the schema for the chat message
        var schema = SchemaHelper.GetSchemaChatMessageV2();
        testOutputHelper.WriteLine(schema);
    }

    [Fact]
    public void PrintSchemaV3()
    {
        // Print the schema for the chat message
        var schema = SchemaHelper.GetSchemaChatMessageV3();
        testOutputHelper.WriteLine(schema);
    }

    [Fact]
    public void PrintSchemaV3Incompatible()
    {
        // Print the schema for the chat message
        var schema = SchemaHelper.GetSchemaChatMessageV3Incompatible();
        testOutputHelper.WriteLine(schema);
    }

    [Fact]
    public void PrintSchemaV4()
    {
        // Print the schema for the chat message
        var schema = SchemaHelper.GetSchemaChatMessageV4();
        testOutputHelper.WriteLine(schema);
    }

    [Fact]
    public async Task SetBackwardCompatibility() => await SetCompatibilitySettingsForSubject(Compatibility.Backward, $"{Example2Config.Topic}-value");

    [Fact]
    public async Task SetForwardCompatibility() => await SetCompatibilitySettingsForSubject(Compatibility.Forward, $"{Example2Config.Topic}-value");

    private async Task SetCompatibilitySettingsForSubject(Compatibility compatibility, string subject)
    {
        var schemaRegistry = fixture.CreateSchemaRegistryClient();
        var result = await schemaRegistry.UpdateCompatibilityAsync(compatibility, subject);
        testOutputHelper.WriteLine($"Compatibility: {result}");
    }
}
