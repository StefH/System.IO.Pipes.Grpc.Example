using System.IO.Pipes;
using System.Text;
using Google.Protobuf;
using Greet;

// Create a named pipe server
using var pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.InOut);
Console.WriteLine("Waiting for a client to connect...");
pipeServer.WaitForConnection();


// Read a message from the client
byte[] buffer = new byte[2024];
int bytesRead = pipeServer.Read(buffer, 0, buffer.Length);

var helloRequest = HelloRequest.Parser.ParseFrom(buffer, 0, bytesRead);


//string clientMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
Console.WriteLine($"Received from client: {helloRequest.Name}");

// Send a response
//string response = "Hello from external process";
//byte[] responseBytes = Encoding.UTF8.GetBytes(response);
var helloReply = new HelloReply { Message = "Hello from external process" };
byte[] responseBytes = helloReply.ToByteArray();
pipeServer.Write(responseBytes, 0, responseBytes.Length);