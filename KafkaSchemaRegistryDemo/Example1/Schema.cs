using Common;
using Confluent.SchemaRegistry;
using Xunit;
using Xunit.Abstractions;
using Schemas;

namespace Example1;

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
        var schemaRegistryResult = await schemaRegistry.RegisterSchemaAsync($"{Example1Config.Topic}-value", schema);

        testOutputHelper.WriteLine($"Registered schema with id {schemaRegistryResult}");
    }

    [Fact]
    public async Task DeleteSchemasSoft()
    {
        await fixture.DeleteSubject(Example1Config.Topic + "-value");
    }

    [Fact]
    public async Task DeleteSchemasPermanent()
    {
        await fixture.DeleteSubject(Example1Config.Topic + "-value", permanent: true);
    }


    [Fact]
    public void PrintSchema()
    {
        // Print the schema for the chat message
        var schema = SchemaHelper.GetSchemaChatMessageV1();
        testOutputHelper.WriteLine(schema);
    }
}
