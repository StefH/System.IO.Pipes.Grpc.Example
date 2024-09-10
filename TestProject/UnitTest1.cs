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
            await pipeClient.WriteAsync(helloRequest);

            // Read the response
            var helloReply = await pipeClient.ReadAsync<HelloReply>();
            Assert.Equal("Hello from external process to xUnit!", helloReply.Message);

            helloRequest = new HelloRequest { Name = "Hello2" };
            await pipeClient.WriteAsync(helloRequest);

            helloReply = await pipeClient.ReadAsync<HelloReply>();
            Assert.Equal("Hello from external process to Hello2", helloReply.Message);
        }

        await process.WaitForExitAsync();
    }
}