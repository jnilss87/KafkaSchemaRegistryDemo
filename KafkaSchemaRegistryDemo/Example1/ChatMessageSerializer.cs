using System.IO.Compression;
using Apex.Serialization;
using Confluent.Kafka;
using Example1.Models;
using Xunit;

namespace Example1;

/// <summary>
/// This is a custom serializer for the ChatMessage class using the Apex.Serialization library
/// to illustrate that you can use any serialization library with Confluent.Kafka.
/// </summary>
public class ChatMessageSerializer : IAsyncSerializer<ChatMessage>
{
    public Task<byte[]> SerializeAsync(ChatMessage data, SerializationContext context)
    {
        var binarySerializer = Binary.Create(new Settings().MarkSerializable(typeof(ChatMessage)).MarkSerializable(typeof(User)));
        using var memoryStream = new MemoryStream();
        binarySerializer.Write(data, memoryStream);
        // gzip compress the serialized data
        return Task.FromResult(Compress(memoryStream.ToArray()));
    }

    public static byte[] Compress(byte[] input)
    {
        using var result = new MemoryStream();
        var lengthBytes = BitConverter.GetBytes(input.Length);
        result.Write(lengthBytes, 0, 4);

        using (var compressionStream = new GZipStream(result, CompressionMode.Compress))
        {
            compressionStream.Write(input, 0, input.Length);
            compressionStream.Flush();
        }

        return result.ToArray();
    }
}

/// <summary>
/// This is a custom deserializer for the ChatMessage class using the Apex.Serialization library
/// </summary>
public class ChatMessageDeserializer : IDeserializer<ChatMessage>
{
    public ChatMessage Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        var binarySerializer = Binary.Create(new Settings().MarkSerializable(typeof(ChatMessage)).MarkSerializable(typeof(User)));
        using var memoryStream = new MemoryStream(Decompress(data.ToArray()));
        return binarySerializer.Read<ChatMessage>(memoryStream);
    }

    public static byte[] Decompress(byte[] input)
    {
        using var source = new MemoryStream(input);
        var lengthBytes = new byte[4];
        _ = source.Read(lengthBytes, 0, 4);

        var length = BitConverter.ToInt32(lengthBytes, 0);
        using var decompressionStream = new GZipStream(source, CompressionMode.Decompress);
        var result = new byte[length];
        _ = decompressionStream.Read(result, 0, length);
        return result;
    }
}

public class ByteArraySerializerTests
{
    [Fact]
    public async Task CanSerializeAndDeserialize()
    {
        var serializer = new ChatMessageSerializer();
        var deserializer = new ChatMessageDeserializer();
        var message = new ChatMessage
        {
            User = new User { Id = "123", Name = "Alice" },
            Content = "Hello, World!",
            Timestamp = DateTime.UtcNow
        };

        var serialized = await serializer.SerializeAsync(message, new SerializationContext(MessageComponentType.Value, "topic"));
        var deserialized = deserializer.Deserialize(serialized, false, new SerializationContext(MessageComponentType.Value, "topic"));

        Assert.Equal(message.User.Id, deserialized.User.Id);
        Assert.Equal(message.User.Name, deserialized.User.Name);
        Assert.Equal(message.Content, deserialized.Content);
        Assert.Equal(message.Timestamp, deserialized.Timestamp);
    }
}
