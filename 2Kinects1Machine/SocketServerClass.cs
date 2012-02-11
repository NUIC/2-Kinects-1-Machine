using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;
using System.IO;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;

namespace _2Kinects1Machine
{
    class SocketServerClass
    {

        Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 
        Socket clientSocket;

        public SocketServerClass()
        {
   
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

        public byte[] pollSocket()
        {
            byte[] receiveBuffer = new byte[4096];

            Console.WriteLine("Recieving Data");
            clientSocket.Receive(receiveBuffer);
            Console.WriteLine("Data Recieved");
            
            return receiveBuffer;
        }
    }
}
