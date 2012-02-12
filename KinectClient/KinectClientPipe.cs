using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;
using Microsoft.Kinect;
using System.Runtime.Serialization.Formatters.Binary;

namespace KinectClient
{
    class KinectClientPipe : KinectClient
    {
        string serverHandle;
        AnonymousPipeClientStream clientStream;

        public KinectClientPipe(string _handle, string _id)
        {
            this.serverHandle = _handle;
            this.kinectId = _id;
        }

        public override void Start()
        {
            try
            {
                Console.WriteLine("[Client] Starting pipe stream");
                clientStream = new AnonymousPipeClientStream(PipeDirection.Out, serverHandle);

                Console.WriteLine("[Client] Stream opened");
                //Console.WriteLine("Transmission mode: {0}", clientStream.TransmissionMode.ToString());

                sensor = null;

                Console.WriteLine("[Client] Found {0} Kinects", KinectSensor.KinectSensors.Count);

                initKinect();
                while (clientStream.IsConnected);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Client] Exception:\n    {0}\nTrace: {1}", e.Message, e.StackTrace);
            }
        }


        protected override void SendSkeletonData()
        {
            // If the previous frame is still being read, wait for it to finish
            clientStream.WaitForPipeDrain();

            // Binary serialize and write the skeleton data over the pipe
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(clientStream, skeletonData);
        }
    }
}
