using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace KinectClient
{
    abstract class KinectClient
    {
        protected KinectSensor sensor;
        protected Skeleton[] skeletonData;

        // 
        protected TransformSmoothParameters smoothingParameters = new TransformSmoothParameters {
            Smoothing = 0.1f,
            Correction = 0.0f,
            Prediction = 0.0f,
            JitterRadius = 1.0f,
            MaxDeviationRadius = 0.5f
        };

        /// <summary>
        /// The event handler for the SkeletonFrameReady event.
        /// Grabs the skeleton data, serializes it and sends it over the pipe to the server
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Contains the skeleton frame data</param>
        protected void KinectSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (skeletonData == null)
            {
                skeletonData = new Skeleton[e.OpenSkeletonFrame().SkeletonArrayLength];
            }

            e.OpenSkeletonFrame().CopySkeletonDataTo(skeletonData);

            this.SendSkeletonData();
        }

        /// <summary>
        /// A Debug function that gets called whenever the Kinect's state change. Since a state change 
        /// is usually a bad thing, this event handler will mainly print out diagnostic data.
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">Contains data on the sensor</param>
        protected void KinectStatusChange(object sender, StatusChangedEventArgs e)
        {
            Console.WriteLine("[Client] Kinect's status changed!");
            Console.WriteLine("[Client] Kinect is running: {0}", e.Sensor.IsRunning);
            Console.WriteLine("[Client] Kinect Skeletal stream is enabled: {0}", e.Sensor.SkeletonStream.IsEnabled);
            Console.WriteLine("[Client] Kinect status: {0}", e.Sensor.Status);
            Console.WriteLine("[Client] Kinect device connection ID: {0}", e.Sensor.DeviceConnectionId);
        }

        protected abstract void SendSkeletonData();
        public abstract void Start();
    }
}
