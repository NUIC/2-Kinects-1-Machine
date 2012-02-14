using System;
using System.Linq;
using System.Windows;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Kinect;
using System.Drawing.Imaging;

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

        #endregion Singleton

        #region Event Registering / Handling

        public delegate void HandMotion(Hands hand, int x, int y);

        public HandMotion movement;

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


        #endregion Event Registering / Handling

        #region Vars

        readonly KinectSensor sensor = KinectSensor.KinectSensors[0];

        private static Joint rightScaledCursorJoint;
        private static Joint leftScaledCursorJoint;

        #endregion Vars

        #region Initialization

        /// <summary>
        /// Called by the constructor. Sets up the Kinect controls.
        /// </summary>
        private void InitializeMotionDetection()
        {
            InitializeRuntime();

            sensor.SkeletonFrameReady += SkeletonFrameReady;
            sensor.Start();
        }

        protected TransformSmoothParameters smoothingParameters = new TransformSmoothParameters
        {
            Smoothing = 0.1f,
            Correction = 0.0f,
            Prediction = 0.0f,
            JitterRadius = 1.0f,
            MaxDeviationRadius = 0.5f
        };

        private void InitializeRuntime()
        {
            //sensor.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | Microsoft.Research.Kinect.Nui.RuntimeOptions.UseColor | RuntimeOptions.UseSkeletalTracking);

            //sensor.SkeletonEngine.TransformSmooth = true;

            //Use to transform and reduce jitter
            //sensor.SkeletonEngine.SmoothParameters = new TransformSmoothParameters
            //{
            //    Smoothing = 0.7f,
            //    Correction = 0.5f,
            //    Prediction = 0.5f,
            //    JitterRadius = 0.05f,
            //    MaxDeviationRadius = 0.04f
            //};

            //You can adjust the resolution here.
            //sensor.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.ColorYuv);
            sensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            sensor.SkeletonStream.Enable(smoothingParameters);
            Console.WriteLine("Video stream is open");
        }

        #endregion Initialization

        #region Motion Detection

        void SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            //if (e.OpenSkeletonFrame()Skeletons.Count() == 0) return;

            SkeletonFrame skeletonSet = e.OpenSkeletonFrame();
            if (skeletonSet == null) return;

            Skeleton[] skeletonData = new Skeleton[skeletonSet.SkeletonArrayLength];
            skeletonSet.CopySkeletonDataTo(skeletonData);

            if (skeletonData.Length == 0) return;


            Skeleton firstPerson = (from s in skeletonData
                                        where s.TrackingState == SkeletonTrackingState.Tracked
                                        orderby s.TrackingId descending
                                        select s).FirstOrDefault();
            if (firstPerson == null) return;

            JointCollection joints = firstPerson.Joints;

            Joint rightHand = joints[JointType.HandRight];
            Joint leftHand = joints[JointType.HandLeft];


            var joinCursorRightHand = rightHand;
            var joinCursorLeftHand = leftHand;

            float rightPosX = 200 * joinCursorRightHand.Position.X + 400;
            float rightPosY = 200 * joinCursorRightHand.Position.Y + 400;

            float leftPosX = 200 * joinCursorLeftHand.Position.X + 400;
            float leftPosY = 200 * joinCursorLeftHand.Position.Y + 400;


            rightScaledCursorJoint = new Joint
            {
                TrackingState = JointTrackingState.Tracked,
                Position = new SkeletonPoint
                {
                    X = rightPosX,
                    Y = rightPosY,
                    Z = joinCursorRightHand.Position.Z
                }
            };
            leftScaledCursorJoint = new Joint
            {
                TrackingState = JointTrackingState.Tracked,
                Position = new SkeletonPoint
                {
                    X = leftPosX,
                    Y = leftPosY,
                    Z = joinCursorLeftHand.Position.Z
                }
            };

            if (movement != null)
            {
                movement(Hands.Right, (int)rightScaledCursorJoint.Position.X, (int)rightScaledCursorJoint.Position.Y);
                movement(Hands.Left, (int)leftScaledCursorJoint.Position.X, (int)leftScaledCursorJoint.Position.Y);
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
