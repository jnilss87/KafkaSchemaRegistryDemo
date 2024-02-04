namespace Schemas;

public static class SchemaHelper
{
    // read content of file /Protos/V1/ChatMessage.proto
    public static string GetSchemaChatMessageV1() => GetSchemaChatMessage(1);
    public static string GetSchemaChatMessageV2() => GetSchemaChatMessage(2);
    public static string GetSchemaChatMessageV3() => GetSchemaChatMessage(3);
    public static string GetSchemaChatMessageV4() => GetSchemaChatMessage(4);

    public static string GetSchemaChatMessage(int version)
    {
        // read content of file /Protos/V{version}/ChatMessage.proto from disk and make sure we use project root as base path
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        return File.ReadAllText(Path.Combine(projectRoot, typeof(SchemaHelper).Namespace!, "Protos", $"V{version}", "Chat.proto"));
    }
}
