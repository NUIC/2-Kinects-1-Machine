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
using System.Threading;

namespace _2Kinects1Machine
{
    public delegate void SkeletonTracked(object sender, EventArgs e);

    class ServerClass
    {
        private string clientProcess;
        private string kinectId;

        public event EventHandler<SkeletonReadyEventArgs> skeletonEvents;

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
                             //   Console.WriteLine("[Server] Tracking a skeleton with ID: {0}", s.TrackingId);
                                AssignSkeleton(s);
                            }
                        }
                    }
                    catch (SerializationException e)
                    {
                        Console.WriteLine("[Server] Exception: {0}", e.Message);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("[SERVER-Thread1] Error: {0}", e.Message);
            }
        }

        void AssignSkeleton(Skeleton skeleton)
        {
            Mutex[] gm = { MultKinectServer.playerList };
            Mutex.WaitAll(gm, 40);

            if (MultKinectServer.players.Count == 0)
            {

                MultKinectServer.players.Add(skeleton);
                gm[0].ReleaseMutex();
                return;
            }

            for (int i = 0; i < MultKinectServer.players.Count; i++)
            {
                if (MultKinectServer.players[i].TrackingId.Equals(skeleton))
                {

                    MultKinectServer.players[i] = skeleton;
                    gm[0].ReleaseMutex();
                    return;
                }
            }

            if (MultKinectServer.players.Count < 4)
            {
                MultKinectServer.players.Add(skeleton);
                gm[0].ReleaseMutex();
            }
        }
    }
}
