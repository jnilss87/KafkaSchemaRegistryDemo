using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Google.Protobuf;

namespace Example1;

public class ByteArraySerializer<T> : IAsyncSerializer<T>
{
    public Task<byte[]> SerializeAsync(T data, SerializationContext context)
    {
        var json = JsonSerializer.Serialize(data);
        var bytes = Encoding.UTF8.GetBytes(json);
        return Task.FromResult(bytes);
    }
}

public class ByteArrayDeserializer<T> : IDeserializer<T>
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        var json = Encoding.UTF8.GetString(data);
        return JsonSerializer.Deserialize<T>(json) ?? throw new InvalidOperationException("Failed to deserialize message");
    }
}
