namespace Schemas;

public static class SchemaHelper
{
    public static string GetSchemaChatMessageV1() => GetSchemaChatMessage(1);
    public static string GetSchemaChatMessageV2() => GetSchemaChatMessage(2);
    public static string GetSchemaChatMessageV3() => GetSchemaChatMessage(3);
    public static string GetSchemaChatMessageV3Incompatible() => GetSchemaChatMessage(3, true);
    public static string GetSchemaChatMessageV4() => GetSchemaChatMessage(4);

    public static string GetSchemaChatMessage(int version, bool incompatible = false)
    {
        // read content of file /Protos/V{version}/ChatMessage.proto from disk and make sure we use project root as base path
        var projectRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var incompatibleSuffix = incompatible ? "-incompatible" : "";
        return File.ReadAllText(Path.Combine(projectRoot, typeof(SchemaHelper).Namespace!, "Protos", $"V{version}{incompatibleSuffix}",
            "Chat.proto"));
    }
}
