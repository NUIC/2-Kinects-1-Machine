using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;
using System.IO;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Kinect;

namespace _2Kinects1Machine
{
    class TCPServerClass
    {
        public event EventHandler<SkeletonReadyEventArgs> skeletonEvents;

        public void ThreadProc()
        {
            try
            {
                // Get the IP address
                IPAddress ipAddress = IPAddress.Parse("192.168.2.2");

                // Create and start the listener
                TcpListener listener = new TcpListener(ipAddress, 5300);
                listener.Start();

                Console.WriteLine("[Server] Listening for Client Socket");

                // Waiting for a TCP Client
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("[Server] Client Socket Accepted");

                // Get the skeleton data while the client is connected
                while(client.Connected) {

                    BinaryFormatter formatter = new BinaryFormatter();

                    Skeleton[] skeletonData = (Skeleton[]) formatter.Deserialize(client.GetStream());

                    foreach (Skeleton s in skeletonData)
                    {
                        if (s.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            //Console.WriteLine("[Server] Tracking a skeleton with ID: {0}", s.TrackingId);
                            OnSkeletonTracked(new SkeletonReadyEventArgs(s));
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Server: Socket error occurred: {0}", e.Message);
            }
        }

        void OnSkeletonTracked(SkeletonReadyEventArgs e)
        {
            if (skeletonEvents != null)
            {
                skeletonEvents(this, e);
            }
        }
    }
}
