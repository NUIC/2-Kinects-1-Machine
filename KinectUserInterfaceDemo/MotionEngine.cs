using System;
using System.Linq;
using System.Windows;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Kinect;
using System.Drawing.Imaging;
using System.Threading;
using System.Collections.Generic;
using Coding4Fun.Kinect.Wpf.Controls;
using System.Windows.Controls;

namespace KinectMenu
{
    /// <summary>
    /// The engine that handles motion input from the Kinect
    /// </summary>
    class MotionEngine
    {

        /// <summary>
        /// Handles the Singleton
        /// </summary>
        #region Singleton

        private static MotionEngine _instance;

        public static MotionEngine getInstance()
        {
            if (_instance == null)
            {
                _instance = new MotionEngine();
            }

            MotionEngine i = _instance;

            if (_instance.sensor.SkeletonStream == null)
            {
                _instance.InitializeMotionDetection();
            }

            return _instance;
        }

        private MotionEngine()
        {
            InitializeMotionDetection();
        }

        //public MotionEngine()
        //{
        //    InitializeMotionDetection();
        //}

        #endregion Singleton

        #region Event Registering / Handling

        public delegate void HandMotion(Hands hand, int x, int y);
        public delegate void HandCreated(HoverButton hand);

        public static HandMotion movement;
        public static HandCreated created;

        public void registerHandMotion(HandMotion handler)
        {
            movement += handler;
        }

        public void removeHandMotion(HandMotion handler)
        {
            movement -= handler;
        }

        public void registerVideoCapture(EventHandler<ColorImageFrameReadyEventArgs> handler)
        {
            sensor.ColorFrameReady += handler;
        }

        public void removeVideoCapture(EventHandler<ColorImageFrameReadyEventArgs> handler)
        {
            sensor.ColorFrameReady -= handler;
        }

        public void registerHandCreated(HandCreated handler)
        {
            created += handler;
        }


        #endregion Event Registering / Handling

        #region Vars

        readonly KinectSensor sensor = KinectSensor.KinectSensors[0];

        private static Joint rightScaledCursorJoint;
        private static Joint leftScaledCursorJoint;

        private List<Thread> threads;

        #endregion Vars

        #region Initialization

        /// <summary>
        /// Called by the constructor. Sets up the Kinect controls.
        /// </summary>
        private void InitializeMotionDetection()
        {
            threads = new List<Thread>();
            InitializeRuntime();

            //sensor.SkeletonFrameReady += SkeletonFrameReady;
            //sensor.Start();


        }

        protected TransformSmoothParameters smoothingParameters = new TransformSmoothParameters
        {
            Smoothing = 0.1f,
            Correction = 0.0f,
            Prediction = 0.0f,
            JitterRadius = 1.0f,
            MaxDeviationRadius = 0.5f
        };

        protected static SkeletonPoint fitToScreen(SkeletonPoint skp)
        {
            SkeletonPoint skpResult = new SkeletonPoint { X = skp.X, Y = skp.Y, Z = skp.Z };
            int screenWidth = 1920;
            int screenHeight = 1080;
            skpResult.X *= (screenWidth / -4);
            skpResult.Y *= (screenHeight / -4);
            skpResult.X += screenWidth / 2;
            skpResult.Y += screenHeight / 2;
            return skpResult;
        }

        private void InitializeRuntime()
        {
            //sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            //sensor.SkeletonStream.Enable(smoothingParameters);
            //Console.WriteLine("Video stream is open");

            if (KinectSensor.KinectSensors.Count > 0)
            {
                foreach (KinectSensor sensor in KinectSensor.KinectSensors)
                {
                    if (sensor.Status == KinectStatus.Connected)
                    {
                        ServerClass server = new ServerClass("D:\\git\\2KinectTechDemo\\KinectClient\\bin\\Debug\\KinectClient.exe", sensor.UniqueKinectId);

                        Thread kinectThread = new Thread(new ThreadStart(server.ThreadProc));
                        threads.Add(kinectThread);
                        kinectThread.SetApartmentState(ApartmentState.STA);
                        //skeletonDict[sensor.UniqueKinectId] = new Skeleton[2];
                        kinectThread.Start();
                    }
                }
            }
        }

        #endregion Initialization

        #region Motion Detection

        Dictionary<int, int> handmap = new Dictionary<int, int>();
        int count = 0;

        internal void SkeletonFrameReady(Skeleton firstPerson)
        {
            int index = 0;

            try
            {
                index = handmap[firstPerson.TrackingId];
            }
            catch (Exception e)
            {
                index = count;
                handmap.Add(firstPerson.TrackingId, count++);
            }
            //if (index == 0)
            //{
            //    handmap.Add(firstPerson.TrackingId, count++);
            //}

            JointCollection joints = firstPerson.Joints;

            Joint rightHand = joints[JointType.HandRight];
            Joint leftHand = joints[JointType.HandLeft];

            var joinCursorRightHand = rightHand;
            var joinCursorLeftHand = leftHand;

            joinCursorRightHand.Position = fitToScreen(joinCursorRightHand.Position);
            joinCursorLeftHand.Position = fitToScreen(joinCursorLeftHand.Position);

            rightScaledCursorJoint = new Joint
            {
                TrackingState = JointTrackingState.Tracked,
                Position = new SkeletonPoint
                {
                    X = joinCursorRightHand.Position.X,
                    Y = joinCursorRightHand.Position.Y,
                    Z = joinCursorRightHand.Position.Z
                }
            };
            leftScaledCursorJoint = new Joint
            {
                TrackingState = JointTrackingState.Tracked,
                Position = new SkeletonPoint
                {
                    X = joinCursorLeftHand.Position.X,
                    Y = joinCursorLeftHand.Position.Y,
                    Z = joinCursorLeftHand.Position.Z
                }
            };

            if (movement != null)
            {
                movement(index != 1 ? Hands.Right : Hands.Right1, (int)rightScaledCursorJoint.Position.X, (int)rightScaledCursorJoint.Position.Y);
                movement(index != 1 ? Hands.Left : Hands.Left1, (int)leftScaledCursorJoint.Position.X, (int)leftScaledCursorJoint.Position.Y);
            }
        }

        /// <summary>
        /// Checks to see if the user's hands are together +/- the scale factor.
        /// </summary>
        /// <param name="scale">The scale factor</param>
        /// <returns>Returns true if the hands are toether. False if not.</returns>
        public Boolean HandsTogether(int scale)
        {
            return rightScaledCursorJoint.Position.X >= leftScaledCursorJoint.Position.X - scale
                && rightScaledCursorJoint.Position.X <= leftScaledCursorJoint.Position.X + scale
                && rightScaledCursorJoint.Position.Y >= leftScaledCursorJoint.Position.Y - scale
                && rightScaledCursorJoint.Position.Y <= leftScaledCursorJoint.Position.Y + scale;
        }

        #endregion Motion Detection

    }
}
