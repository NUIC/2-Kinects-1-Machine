using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Microsoft.Kinect;

namespace _2Kinects1Machine
{
    class ServerClass
    {
        private string clientProcess;
        private string kinectId;

        public ServerClass(string client, string kid)
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

            Console.WriteLine("Setting up Kinect with ID: {0}", KinectSensor.KinectSensors[0].UniqueKinectId);

            childProcess.StartInfo.UseShellExecute = false;
            childProcess.Start();

            pipeServer.DisposeLocalCopyOfClientHandle();

            try
            {
                //// Read user input and send that to the client process.
                //using (StreamReader sr = new StreamReader(pipeServer))
                //{
                //    char temp;
                //    string message = "";
                Console.WriteLine("[Server] Ready for Skeleton data");
                BinaryFormatter formatter = new BinaryFormatter();

                Skeleton[] skeletonData;

                while (pipeServer.IsConnected)
                {
                    try
                    {
                        skeletonData = (Skeleton[]) formatter.Deserialize(pipeServer);
                        foreach (Skeleton s in skeletonData)
                        {
                            if (s.TrackingState == SkeletonTrackingState.Tracked)
                            {
                                Console.WriteLine("[Server] Tracking a skeleton with ID: {0}", s.TrackingId);
                            }
                        }
                    }
                    catch (SerializationException e)
                    {
                        Console.WriteLine("[Server] Exception: {0}", e.Message);
                    }
                }
                //}
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
