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
    var buffer = ArrayPool<byte>.Shared.Rent(2048);
    var bytesRead = await pipeServer.ReadAsync(buffer, 0, buffer.Length);

    var helloRequest = HelloRequest.Parser.ParseFrom(buffer, 0, bytesRead);
    Console.WriteLine($"Received from client: {helloRequest.Name}");

    // Send a response
    var helloReply = new HelloReply { Message = $"Hello from external process to {helloRequest.Name}" };
    var responseBytes = helloReply.ToByteArray();
    await pipeServer.WriteAsync(responseBytes, 0, responseBytes.Length);
    await pipeServer.FlushAsync();
}