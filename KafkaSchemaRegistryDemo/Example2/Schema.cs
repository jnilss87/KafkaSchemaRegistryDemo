using Common;
using Confluent.SchemaRegistry;
using Schemas;
using Xunit.Abstractions;

namespace Example2;

public class Schema(ConfluentCloudFixture fixture, ITestOutputHelper testOutputHelper) : IClassFixture<ConfluentCloudFixture>
{
    [Fact]
    public async Task AttachSchemaToTopic()
    {
        var schemaRegistry = fixture.CreateSchemaRegistryClient();

        // Get the schema for the chat message, there is probably a better way to do this but this is the easiest right now
        var proto = SchemaHelper.GetSchemaChatMessageV1();

        // Construct a schema object, clarify that it is a protobuf schema
        var schema = new Confluent.SchemaRegistry.Schema(proto, SchemaType.Protobuf);

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
            testOutputHelper.WriteLine($"Schema version: {schemaVersion}");
            var schema = await schemaRegistry.GetRegisteredSchemaAsync(Example2Config.Topic + "-value", schemaVersion);
            testOutputHelper.WriteLine(schema.SchemaString);
        }
    }
    
    [Fact]
    public async Task CheckCompatibility()
    {
        var schemaRegistry = fixture.CreateSchemaRegistryClient();
        var nextSchema = SchemaHelper.GetSchemaChatMessageV2();
        var schema = new Confluent.SchemaRegistry.Schema(nextSchema, SchemaType.Protobuf);
        var result = await schemaRegistry.IsCompatibleAsync($"{Example2Config.Topic}-value", schema);
    }
    
    [Fact]
    public async Task DeleteSchema()
    {
        
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
    public void PrintSchemaV4()
    {
        // Print the schema for the chat message
        var schema = SchemaHelper.GetSchemaChatMessageV4();
        testOutputHelper.WriteLine(schema);
    }
}
