using Chat.V1;
using Google.Protobuf;
namespace Schemas;

public static class SchemaHelper
{
    // read content of file /Protos/V1/ChatMessage.proto
    public static string GetSchemaChatMessageV1()
    {
        // read content of file /Protos/V1/ChatMessage.proto from disk and make sure we use project root as base path
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        return File.ReadAllText(Path.Combine(projectRoot, typeof(SchemaHelper).Namespace!, "Protos", "V1", "Chat.proto"));
    }
}
