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
        string kinectId;
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

                /**
                 * Setting up the Kinect
                 **/
                if (KinectSensor.KinectSensors.Count > 0)
                {
                    KinectSensorCollection sensors = KinectSensor.KinectSensors;
                    Console.WriteLine(kinectId);
                    int sensorIndex = GetSensorIndexFromID(kinectId);


                    sensor = KinectSensor.KinectSensors[sensorIndex];
                    sensor.SkeletonStream.Enable(smoothingParameters);
                    sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(KinectSkeletonFrameReady);
                    KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectStatusChange);
                    sensor.Start();
                    while (clientStream.IsConnected) ;
                }
                else
                {
                    //throw new Exception("No Kinect sensors found");
                    Console.WriteLine("No Kinect sensors found");
                    while (true) ;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("[Client] Exception:\n    {0}\nTrace: {1}", e.Message, e.StackTrace);
            }
        }

        /// <summary>
        /// Checks the inputted KinectID and returns its index in the KinectSensorCollection
        /// </summary>
        /// <param name="uniqueId">The unique Kinect Id inputed by the parent process</param>
        /// <returns>The index into the KinectSensorCollection associated with the unique Id</returns>
        static int GetSensorIndexFromID(string uniqueId)
        {
            for (int i = 0; i < KinectSensor.KinectSensors.Count; i++)
            {
                // Check to see if the Kinect is connected to the system
                // Check to see if the Kinect is not already running
                // Check to see if the UniqueId matches
                // If all are true return
                if (KinectSensor.KinectSensors[i].Status == KinectStatus.Connected
                    && !KinectSensor.KinectSensors[i].IsRunning
                    && KinectSensor.KinectSensors[i].UniqueKinectId.Equals(uniqueId))
                {
                    return i;
                }
            }

            throw new Exception("Invalid unique ID");
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
