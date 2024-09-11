using System.IO.Pipes;
using GrpcServer;

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