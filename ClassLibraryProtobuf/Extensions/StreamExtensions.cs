using System.Buffers;
using Google.Protobuf;

// ReSharper disable once CheckNamespace
namespace System.IO;

public static class StreamExtensions
{
    public static async Task<T> ReadAsync<T>(this Stream stream, CancellationToken cancellationToken = default) where T : IMessage<T>, new()
    {
        var buffer = ArrayPool<byte>.Shared.Rent(2048);
        try
        {
            var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
            return new MessageParser<T>(() => new T()).ParseFrom(buffer, 0, bytesRead);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public static async Task WriteAsync<T>(this Stream stream, T message, CancellationToken cancellationToken = default) where T : IMessage<T>
    {
        var messageBytes = message.ToByteArray();
        await stream.WriteAsync(messageBytes, 0, messageBytes.Length, cancellationToken);
        await stream.FlushAsync(cancellationToken);
    }
}