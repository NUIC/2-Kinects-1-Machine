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
    class KinectClient
    {
        // The client pipestream for sending SkeletonData to the parent
        private static PipeStream clientStream;
        // Store a reference to this process' Kinect sensor
        private static KinectSensor sensor;

        // Smoothing values for the skeleton tracker
        private static TransformSmoothParameters smoothingParameters = new TransformSmoothParameters
        {
            Smoothing = 0.1f,
            Correction = 0.0f,
            Prediction = 0.0f,
            JitterRadius = 1.0f,
            MaxDeviationRadius = 0.5f
        };


        static void Main(string[] args)
        {

#if (DEBUG)
            if (args.Length != 2)
            {
                SetupParentProcess();
            }
#else
            // We are expecting exactly two arguments (both are required)
            if (args.Length != 2)
            {
                throw new Exception("No pipe data passed");
            }
#endif

            try
            {
                Console.WriteLine("[Client] Starting pipe stream");
                clientStream = new AnonymousPipeClientStream(PipeDirection.Out, args[0]);
                
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
                    Console.WriteLine(args[1]);
                    int sensorIndex = GetSensorIndexFromID(args[1]);
                    

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

        /// <summary>
        /// The event handler for the SkeletonFrameReady event.
        /// Grabs the skeleton data, serializes it and sends it over the pipe to the server
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Contains the skeleton frame data</param>
        static void KinectSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Console.WriteLine("[Client] Current Kinect frame: {0}", e.OpenSkeletonFrame().FrameNumber);
            // If the previous frame is still being read, wait for it to finish
            clientStream.WaitForPipeDrain();

            // Grab the skeleton data and save it locally
            Skeleton[] skeletonData = new Skeleton[e.OpenSkeletonFrame().SkeletonArrayLength];
            e.OpenSkeletonFrame().CopySkeletonDataTo(skeletonData);

            // Binary serialize and write the skeleton data over the pipe
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(clientStream, skeletonData);
        }

        /// <summary>
        /// A Debug function that gets called whenever the Kinect's state change. Since a state change 
        /// is usually a bad thing, this event handler will mainly print out diagnostic data.
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Contains data on the sensor</param>
        static void KinectStatusChange(object sender, StatusChangedEventArgs e)
        {
            Console.WriteLine("[Client] Kinect's status changed!");
            Console.WriteLine("[Client] Kinect is running: {0}", e.Sensor.IsRunning);
            Console.WriteLine("[Client] Kinect Skeletal stream is enabled: {0}", e.Sensor.SkeletonStream.IsEnabled);
            Console.WriteLine("[Client] Kinect status: {0}", e.Sensor.Status);
            Console.WriteLine("[Client] Kinect device connection ID: {0}", e.Sensor.DeviceConnectionId);
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
                // Read user input and send that to the client process.
                using (StreamReader sr = new StreamReader(pipeServer))
                {
                    //char temp;
                    //string message = "";
                    Console.WriteLine("[Server] Waiting for skeleton data");

                    while (pipeServer.IsConnected)
                    {
                        BinaryFormatter formatter = new BinaryFormatter();

                        try
                        {
                            skeletonData = (Skeleton[])formatter.Deserialize(pipeServer);
                            Console.WriteLine("[Server] Found a skeleton with ID: {0}", skeletonData[0].TrackingId);
                        }
                        catch (SerializationException e)
                        {
                            Console.WriteLine("[Server] Exception: {0}", e.Message);
                        }
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
