using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;
using System.Threading;

namespace KinectClient
{
    class KinectClient
    {
        static void Main(string[] args)
        {
#if (DEBUG)
            if (args.Length == 0)
            {

                
                SetupParentProcess();
            }
#else
            if (args.Length == 0)
            {
                throw new Exception("No pipe data passed");
            }
#endif

            try
            {
                Console.WriteLine("[Client] Starting pipe stream");
                PipeStream clientStream = new AnonymousPipeClientStream(PipeDirection.Out, args[0]);
                
                Console.WriteLine("[Client] Stream opened");
                Console.WriteLine("Transmission mode: {0}", clientStream.TransmissionMode.ToString());

                
                StreamWriter writer = new StreamWriter(clientStream);

                ConsoleKey key;
                while ((key = Console.ReadKey().Key)!= ConsoleKey.Escape)
                {
                    if (key == ConsoleKey.Enter)
                    {
                        writer.Write((char)13);
                        writer.Flush();
                        clientStream.WaitForPipeDrain();
                        Console.WriteLine("[Client] Pipe drained.");
                        Console.WriteLine("\nWrite Something else");
                    }
                    else
                    {
                        string c = key.ToString();
                        writer.Write(c);
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("[Client] Exception:\n    {0}", e.Message);
            }
        }

#if (DEBUG)

        static void SetupParentProcess()
        {
            AnonymousPipeServerStream pipeServer =
                new AnonymousPipeServerStream(PipeDirection.In,
                HandleInheritability.Inheritable);

            Process childProcess = new Process();
            childProcess.StartInfo.FileName = "D:\\git\\2KinectTechDemo\\KinectClient\\bin\\Debug\\KinectClient.exe";
            childProcess.StartInfo.Arguments += " " + pipeServer.GetClientHandleAsString();
            childProcess.StartInfo.Arguments += " " + 0;

            childProcess.StartInfo.UseShellExecute = false;
            childProcess.Start();

            pipeServer.DisposeLocalCopyOfClientHandle();

            try
            {
                // Read user input and send that to the client process.
                using (StreamReader sr = new StreamReader(pipeServer))
                {
                    char temp;
                    string message = "";
                    Console.WriteLine("[Server] Waiting for input");

                    while (pipeServer.IsConnected)
                    {
                        temp = (char) sr.Read();

                        if (temp != (char)13)
                        {
                            message += temp;
                        }
                        else
                        {
                            Console.WriteLine("[Server] Echo: {0} ", message);
                            message = "";
                        }
                    }
                }
            }
            // Catch the IOException that is raised if the pipe is broken
            // or disconnected.
            catch (IOException e)
            {
                Console.WriteLine("[SERVER-Thread1] Error: {0}", e.Message);
            }
        }

#endif
    }
}
