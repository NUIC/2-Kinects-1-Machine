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

        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
        Socket clientSocket;

        public SocketServerClass()
        {
            ThreadProc();
        }

        public void ThreadProc()
        {

            

            try
            {
                
				IPEndPoint ipLocal = new IPEndPoint (IPAddress.Any, 4889);
				// Bind to local IP Address...
                Console.WriteLine("New socket opened on port 4889");
				serverSocket.Bind( ipLocal );
				// Start listening...
                Console.WriteLine("Listening for Client Socket");
				serverSocket.Listen (1);
                clientSocket = serverSocket.Accept();
                Console.WriteLine("Client Socket Accepted");

            }
            // Catch the Socket that is raised if the socket is broken
            // or disconnected.
            catch (SocketException e)
            {
                Console.WriteLine("Server: Socket error occurred: {0}", e.Message);
            }

            finally
            {
                
                    serverSocket.Close();
                
            }
        }

        public Skeleton[] pollSocket()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Skeleton[] skeletonData;

            NetworkStream n = new NetworkStream(serverSocket);


            try
            {
                skeletonData = (Skeleton[])formatter.Deserialize(n);
                Console.WriteLine("[Server] Found a skeleton with ID: {0}", skeletonData[0].TrackingId);
                Console.WriteLine("Data Recieved");
                return skeletonData;
            }
            catch (Exception e)
            {
                Console.WriteLine("[Stream] Exception: {0}", e.Message);
            }
            return null;
        }
    }
}
