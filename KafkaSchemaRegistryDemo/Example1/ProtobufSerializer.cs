using System.Text;
using System.Text.Json;
using Google.Protobuf;

namespace Example1;

public static class ProtobufSerializer
{
    public static byte[] SerializeCSharpToByteArray<T>(T source)
    {
        // Create byte array from source (ordinary c# object, no .toByteArray() method available)
        var json = JsonSerializer.Serialize(source);
        var bytes = Encoding.UTF8.GetBytes(json);

        // pack source into Any
        return bytes;
    }

    public static T? DeserializeCSharpFromByteArray<T>(byte[] data)
    {
        // Unpack Any and deserialize to T
        var json = Encoding.UTF8.GetString(data);
        return JsonSerializer.Deserialize<T>(json);
    }

    public static byte[] SerializeMessages<T>(T source)
    {
        if (source is IMessage message)
        {
            return message.ToByteArray();
        }

        throw new ArgumentException("Source type must be a protobuf message");
    }

    public static T DeserializeMessages<T>(byte[] data) where T : IMessage<T>, new()
    {
        return new MessageParser<T>(() => new T()).ParseFrom(data);
    }
}
