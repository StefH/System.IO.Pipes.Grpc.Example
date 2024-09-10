using Google.Protobuf;
using Greet;

namespace TestProject;

using System.Buffers;
using System.Diagnostics;
using System.IO.Pipes;
using Xunit;

public class UnitTest1
{
    [Fact]
    public async Task TestExternalProcessCommunication()
    {
        // Start the external process
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = @"C:\Dev\GitHub\SystemIoPipesExample\ExternalProcess\bin\Debug\net48\ExternalProcess.exe",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                //UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();

        // Set up named pipe client
        await using (var pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.InOut))
        {
            await pipeClient.ConnectAsync(5000); // Timeout after 1 seconds if no connection

            // Send a message to the external process
            var helloRequest = new HelloRequest { Name = "xUnit!" };
            // string message = "Hello from xUnit!";
            //byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            var messageBytes = helloRequest.ToByteArray();
            pipeClient.Write(messageBytes, 0, messageBytes.Length);
            pipeClient.Flush();

            // Read the response
            var buffer = ArrayPool<byte>.Shared.Rent(2048);
            var bytesRead = await pipeClient.ReadAsync(buffer, 0, buffer.Length);

            var helloReply = HelloReply.Parser.ParseFrom(buffer, 0, bytesRead);
            Assert.Equal("Hello from external process to xUnit!", helloReply.Message);


            helloRequest = new HelloRequest { Name = "Hello2" };
            messageBytes = helloRequest.ToByteArray();
            await pipeClient.WriteAsync(messageBytes, 0, messageBytes.Length);
            pipeClient.Flush();

            bytesRead = pipeClient.Read(buffer, 0, buffer.Length);
            helloReply = HelloReply.Parser.ParseFrom(buffer, 0, bytesRead);
            Assert.Equal("Hello from external process to Hello2", helloReply.Message);
        }

        await process.WaitForExitAsync();
    }
}