using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;

namespace KinectClient
{
    class KinectClient
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                //throw new Exception("No pipe data passed");
                SetupParentProcess();
            }

            try
            {
                Console.WriteLine("[Client] Starting pipe stream");
                PipeStream clientStream = new AnonymousPipeClientStream(PipeDirection.Out, args[0]);
                
                Console.WriteLine("[Client] Stream opened");
                Console.WriteLine("Transmission mode: {0}", clientStream.TransmissionMode.ToString());

                StreamWriter writer = new StreamWriter(clientStream);
                writer.AutoFlush = true;

                ConsoleKey key;
                while ((key = Console.ReadKey().Key)!= ConsoleKey.Escape)
                {
                    if (key == ConsoleKey.Enter)
                    {
                        //Console.WriteLine("\nSending to server");
                        //writer.Flush();
                        
                        Console.WriteLine("\nWrite Something else");
                    }
                    else
                    {
                        string c = key.ToString();
                        writer.Write(c);
                        writer.Flush();
                        clientStream.WaitForPipeDrain();
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("[Client] Exception:\n    {0}", e.Message);
            }

            //while (true) ;
        }

        private static Byte[] readBuffer = new Byte[1000];

        static void SetupParentProcess()
        {
            AnonymousPipeServerStream pipeServer =
                new AnonymousPipeServerStream(PipeDirection.In,
                HandleInheritability.Inheritable, );

            Process childProcess = new Process();
            childProcess.StartInfo.FileName = "D:\\git\\2KinectTechDemo\\KinectClient\\bin\\Debug\\KinectClient.exe";
            childProcess.StartInfo.Arguments += " " + pipeServer.GetClientHandleAsString();
            childProcess.StartInfo.Arguments += " " + 0;

            childProcess.StartInfo.UseShellExecute = false;
            childProcess.Start();

            pipeServer.DisposeLocalCopyOfClientHandle();

            pipeServer.BeginRead(readBuffer, 0, 1000, finishedRead,  

            try
            {
                // Read user input and send that to the client process.
                using (StreamReader sr = new StreamReader(pipeServer))
                {
                    //sr.AutoFlush = true;
                    // Send a 'sync message' and wait for client to receive it.
                    //sr.WriteLine("SYNC");
                    //pipeServer.WaitForPipeDrain();
                    // Send the console input to the client process.
                    //Console.Write("[SERVER] Enter text: ");
                    //sr.WriteLine(Console.ReadLine());
                    string temp;
                    Console.WriteLine("[Server] Waiting for input");

                    while ((temp = sr.ReadLine()) != "x")
                    {
                        if (temp != null)
                        {

                            Console.WriteLine("[Server] Echo: {0} ", temp);
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

        static void finishedRead()
        {
            Console.WriteLine
        }
    }
}
