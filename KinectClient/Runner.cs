using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;
using System.Threading;
using Microsoft.Kinect;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace KinectClient
{
    /// <summary>
    /// 
    /// </summary>
    class Runner
    {
        static void Main(string[] args)
        {

#if (DEBUG)
            if (args.Length != 2)
            {
                //SetupParentProcess();
                KinectClient client = new KinectClientSocket();
                client.Start();
            }
#else
            // We are expecting exactly two arguments (both are required)
            if (args.Length != 2)
            {
                throw new Exception("No pipe data passed");
            }
#endif
            else
                
            {
                KinectClient client = new KinectClientPipe(args[0], args[1]);
                client.Start();
            }

            
        }

#if (DEBUG)
        /// <summary>
        /// This method should be moved into MultKinectServer.cs.
        /// Right now, this is easier for debugging and getting the client right.
        /// </summary>
        static void SetupParentProcess()
        {
            AnonymousPipeServerStream pipeServer =
                new AnonymousPipeServerStream(PipeDirection.In,
                HandleInheritability.Inheritable);

            Process childProcess = new Process();
            childProcess.StartInfo.FileName = "D:\\git\\2KinectTechDemo\\KinectClient\\bin\\Debug\\KinectClient.exe";
            childProcess.StartInfo.Arguments += " " + pipeServer.GetClientHandleAsString();
            childProcess.StartInfo.Arguments += " " + KinectSensor.KinectSensors[0].UniqueKinectId;

            Console.WriteLine("Setting up Kinect with ID: {0}", KinectSensor.KinectSensors[0].UniqueKinectId);

            childProcess.StartInfo.UseShellExecute = false;
            childProcess.Start();

            pipeServer.DisposeLocalCopyOfClientHandle();

            Skeleton[] skeletonData;

            try
            {
                Console.WriteLine("[Server] Waiting for skeleton data");
                BinaryFormatter formatter = new BinaryFormatter();

                while (pipeServer.IsConnected)
                {
                    try
                    {
                        skeletonData = (Skeleton[])formatter.Deserialize(pipeServer);
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
            }
            // Catch the IOException that is raised if the pipe is broken
            // or disconnected.
            catch (IOException e)
            {
                Console.WriteLine("[SERVER-Thread1] Error: {0}", e.Message);
                while (true) ;
            }
            while(true);
        }

#endif
    }
}
