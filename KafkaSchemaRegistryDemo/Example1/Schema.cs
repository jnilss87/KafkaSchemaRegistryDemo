using Chat.V1;
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
        var proto = SchemaHelper.GetSchemaChatMessageV1();
        var schema = new Confluent.SchemaRegistry.Schema(proto, SchemaType.Protobuf);

        // POST /subjects/{subject}/versions?normalize=false with the schema as the body
        // This will register the schema on the subject using TopicNameStrategy for topic "test-topic-example1".
        // The -value suffix will make it the value schema
        var schemaRegistryResult = await schemaRegistry.RegisterSchemaAsync("test-topic-example1-value", schema);

        testOutputHelper.WriteLine($"Registered schema with id {schemaRegistryResult}");
    }


    [Fact]
    public void PrintSchema()
    {
        // Print the schema for the chat message
        var schema = SchemaHelper.GetSchemaChatMessageV1();
        testOutputHelper.WriteLine(schema);
    }
}
