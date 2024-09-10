using System.Buffers;
using System.IO.Pipes;
using Google.Protobuf;
using Greet;

// Create a named pipe server
using var pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.InOut);
Console.WriteLine("Waiting for a client to connect...");
pipeServer.WaitForConnection();

while (pipeServer.IsConnected)
{
    // Read a message from the client
    var helloRequest = await pipeServer.ReadAsync<HelloRequest>();
    Console.WriteLine($"Received from client: {helloRequest.Name}");

    // Send a response
    var helloReply = new HelloReply { Message = $"Hello from external process to {helloRequest.Name}" };
    await pipeServer.WriteAsync(helloReply);
}

return;

//async Task<T> ReadAsync<T>(Stream stream) where T : IMessage<T>, new()
//{
//    var buffer = ArrayPool<byte>.Shared.Rent(2048);
//    try
//    {
//        var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

//        return new MessageParser<T>(() => new T()).ParseFrom(buffer, 0, bytesRead);
//        //return HelloRequest.Parser.ParseFrom(buffer, 0, bytesRead);
//    }
//    finally
//    {
//        ArrayPool<byte>.Shared.Return(buffer);
//    }
//}