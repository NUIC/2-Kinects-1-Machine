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
    class SocketServerClass
    {
        //SocketInformation socketInfo;
        //Socket serverSocket;// = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
        //Socket clientSocket;


        public SocketServerClass()
        {
            //Socket socket = new Socket();
            //socketInfo = new SocketInformation();
            //socketInfo.Options = new SocketInformationOptions {
                
            //};
            //ThreadProc();
        }

        public void ThreadProc()
        {
            try
            {
                //Socket serverSocket = new Socket(/*AddressFamily.InterNetwork*/ AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
                IPAddress ipAddress = IPAddress.Parse("192.168.2.2");

                TcpListener listener = new TcpListener(ipAddress, 5300);
                listener.Start();

                // Create ip-endpoint
                //IPEndPoint localEndPoint = new IPEndPoint (ipAddress, 5300);
                //// Bind to local IP Address...
                //Console.WriteLine("[Server] New socket opened on port 5300");
                //serverSocket.Bind( localEndPoint );
				// Start listening...
                Console.WriteLine("[Server] Listening for Client Socket");
                //serverSocket.Listen(1);
                //Socket clientSocket = serverSocket.Accept();
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("[Server] Client Socket Accepted");

                while(client.Connected) {
                //Get a network stream
                    //NetworkStream networkStream = new NetworkStream(clientSocket);
                    BinaryFormatter formatter = new BinaryFormatter();

                    Skeleton[] skeletonData = (Skeleton[]) formatter.Deserialize(client.GetStream());

                    foreach (Skeleton s in skeletonData)
                    {
                        if (s.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            Console.WriteLine("[Server] Tracking a skeleton with ID: {0}", s.TrackingId);
                        }
                    }
                }

            }
            // Catch the Exception that is raised if the socket is broken
            // or disconnected.
            catch (SocketException e)
            {
                Console.WriteLine("Server: Socket error occurred: {0}", e.Message);
            }

            //finally
            //{
            //        serverSocket.Close();
            //}
        }

        //public Skeleton[] pollSocket()
        //{
        //    BinaryFormatter formatter = new BinaryFormatter();
        //    Skeleton[] skeletonData;

        //    NetworkStream n = new NetworkStream(serverSocket);

        //    try
        //    {
        //        skeletonData = (Skeleton[])formatter.Deserialize(n);
        //        Console.WriteLine("[Server] Found a skeleton with ID: {0}", skeletonData[0].TrackingId);
        //        Console.WriteLine("Data Recieved");
        //        return skeletonData;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("[Stream] Exception: {0}", e.Message);
        //    }
        //    return null;
        //}
    }
}
