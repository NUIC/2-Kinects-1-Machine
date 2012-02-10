using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;
using System.IO;
using System.Diagnostics;

namespace _2Kinects1Machine
{
    class ServerClass
    {
        private string clientProcess;
        private int kinectId;

        public ServerClass(string client, int kid)
        {
            this.clientProcess = client;
            this.kinectId = kid;
        }

        public void ThreadProc()
        {
            AnonymousPipeServerStream pipeServer =
                    new AnonymousPipeServerStream(PipeDirection.In,
                    HandleInheritability.Inheritable);

            Process childProcess = new Process();
            childProcess.StartInfo.FileName = this.clientProcess;
            childProcess.StartInfo.Arguments += " " + pipeServer.GetClientHandleAsString();
            childProcess.StartInfo.Arguments += " " + kinectId;

            childProcess.StartInfo.UseShellExecute = false;
            childProcess.Start();

            pipeServer.DisposeLocalCopyOfClientHandle();

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

                    while((temp = sr.ReadLine()) != null)
                    {
                        Console.WriteLine("[Server] Echo: {0} ", temp); 
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
    }
}
