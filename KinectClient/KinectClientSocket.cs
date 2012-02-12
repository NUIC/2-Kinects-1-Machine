using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Kinect;

namespace KinectClient
{
    class KinectClientSocket : KinectClient
    {
        public readonly Int32 PORT = 5300;
        public readonly String IP = "192.168.2.2";

        BinaryFormatter binaryFormatter;
        TcpClient client;
        NetworkStream stream;


        public KinectClientSocket()
        {
            binaryFormatter = new BinaryFormatter();
            kinectId = KinectSensor.KinectSensors[0].UniqueKinectId;
        }

        /**
         * Sets up the socket and then the kinect.
         */
        public override void Start()
        {

            try
            {
                client = new TcpClient(IP, PORT);
                // get the stream for reading/writing.
                Console.WriteLine("[Client] Starting socket stream");
                stream = client.GetStream();

                Console.WriteLine("[Client] Stream opened");

                sensor = null;

                Console.WriteLine("[Client] Found {0} Kinects", KinectSensor.KinectSensors.Count);

                initKinect();
                while (client.Connected) ;
            }
            catch (Exception e)
            {
                Console.WriteLine("[Client] Exception:\n    {0}\nTrace: {1}", e.Message, e.StackTrace);
            }
            finally
            {

            }
        }

        /**
         * Called each time when a new skeleton[] is ready.
         */
        protected override void SendSkeletonData()
        {
            try
            {
                Console.WriteLine("[Client] Sending Data");
                binaryFormatter.Serialize(stream, skeletonData);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                while (true) ;
            }
            
        }


    }
}
