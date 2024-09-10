using Google.Protobuf;
using Greet;

namespace TestProject;

using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using Xunit;

public class UnitTest1
{
    [Fact]
    public void TestExternalProcessCommunication()
    {
        // Start the external process
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                //FileName = @"C:\Dev\GitHub\SystemIoPipesExample\ExternalProcess\bin\Debug\net8.0\ExternalProcess.exe",
                FileName = @"C:\Dev\GitHub\SystemIoPipesExample\ExternalProcess\bin\Debug\net48\ExternalProcess.exe",
                // WorkingDirectory = @"..\..\..\..\ExternalProcess\bin\Debug\net8.0",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();

        // Set up named pipe client
        using (var pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.InOut))
        {
            pipeClient.Connect(1000); // Timeout after 1 seconds if no connection

            // Send a message to the external process
            var helloRequest = new HelloRequest { Name = "Hello from xUnit!" };
            // string message = "Hello from xUnit!";
            //byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] messageBytes = helloRequest.ToByteArray();
            pipeClient.Write(messageBytes, 0, messageBytes.Length);

            // Read the response
            // byte[] buffer = new byte[256];
            byte[] buffer = new byte[2048];
            int bytesRead = pipeClient.Read(buffer, 0, buffer.Length);
           // string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            // Assert the response is correct
            var helloReply = HelloReply.Parser.ParseFrom(buffer, 0, bytesRead);
            Assert.Equal("Hello from external process", helloReply.Message);
        }

        process.WaitForExit();
    }
}