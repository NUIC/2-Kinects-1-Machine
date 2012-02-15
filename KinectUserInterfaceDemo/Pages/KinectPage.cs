#define SPEECH
#define CLAPPING
#define HOVER

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Coding4Fun.Kinect.Wpf.Controls;
using Microsoft.Speech.Recognition;
using Microsoft.Kinect;

namespace KinectMenu
{
    /// <summary>
    /// A generalized Page that is meant to make use of the Kinect's sensors (motion control, speech recognition,...)
    /// Extend this page to enable kinect controls on your Page.
    /// </summary>
    public partial class KinectPage : Page
    {

        #region Vars

        protected static double _handLeft;
        protected static double _handTop;

        protected List<Button> buttons;
        protected Image liveVideo;
        protected static HoverButton kinectLeft;
        protected static HoverButton kinectRight;
        protected static HoverButton kinectLeft1;
        protected static HoverButton kinectRight1;

        private MotionEngine motion;
        //private SpeechEngine speech;

        protected static Button _selectedButton = null;

        protected Boolean inAction = false;

        #endregion Vars

        #region Initialization

        public KinectPage()
        {
            Loaded += new RoutedEventHandler(MainWindow_Loaded);
            Unloaded += new RoutedEventHandler(MainWindow_Unloaded);


        }

        /// <summary>
        /// Override this method.
        /// Allows the kinect page to "know" about any
        /// selectable buttons on the Page.
        /// </summary>
        protected virtual void InitializeButtons() { }

        /// <summary>
        /// Override this method.
        /// Allows the kinect page to update the video image
        /// of the person moving.
        /// </summary>
        protected virtual void InitializeVideoImage() { }

        /// <summary>
        /// Override this method.
        /// Attaches the kinect hand icons to the user's hands.
        /// </summary>
        protected virtual void InitializeKinectHandIcons() { }

        /// <summary>
        /// A callback method for when the page is loaded.
        /// Set's up the buttons and live image for the current page.
        /// Initializes the motion and speech controls as well.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeButtons();
            InitializeVideoImage();
            InitializeKinectHandIcons();

            motion = MotionEngine.getInstance();

            //InitializeMotionControl
            motion.registerHandMotion(OnButtonLocationChanged);
            //motion.registerVideoCapture(this.runtime_VideoFrameReady);

//#if (SPEECH)
//            speech = SpeechEngine.getInstance();

//            //InitializeSpeechRecognition();
//            speech.registerSpeechRecognized(this.SpeechRecognized);
//            speech.registerSpeechRejected(this.SpeechRejected);
//#endif
        }

        #endregion Initialization

        #region Unloading

        /// <summary>
        /// A callback method for when the page is unloaded.
        /// Remove's event handlers from the speech and motion modules.
        /// Reset's all of the referrences.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            buttons = new List<Button>();
            liveVideo = null;
            kinectLeft = null;
            kinectRight = null;

            //SpeechEngine.getInstance().removeSpeechRecognized(this.SpeechRecognized);
            //SpeechEngine.getInstance().removeSpeechRejected(this.SpeechRejected);

            MotionEngine.getInstance().removeHandMotion(KinectPage.OnButtonLocationChanged);
            //MotionEngine.getInstance().removeVideoCapture(this.runtime_VideoFrameReady);
        }

        #endregion Unloading


        #region Motion

        /// <summary>
        /// The main method for handling motion. Checks for clapping or hover events.
        /// </summary>
        /// <param name="movingHand">The hand that is currently moving</param>
        /// <param name="x">The moving hand's current x position</param>
        /// <param name="y">The moving hand's current y position</param>
        public static void OnButtonLocationChanged(Hands movingHand, int x, int y)
        {
            HoverButton hand;//, otherHand;

            if (movingHand == Hands.Right)
            {
                hand = kinectRight;
                //otherHand = kinectLeft;
            }
            else if (movingHand == Hands.Left)
            {
                hand = kinectLeft;
                //otherHand = kinectRight;
            }
            else if (movingHand == Hands.Right1)
            {
                hand = kinectRight1;
            }
            else
            {
                hand = kinectLeft1;
            }

#if (CLAPPING && !HOVER)
            if (!inAction && IsHandsTogetherOnAButton(hand, otherHand, buttons))
            {
                // call the click event of the selected button
                _selectedButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, _selectedButton));
            } else { hand.Release(); inAction = false; }
#endif

//#if (HOVER && !CLAPPING)
            //if (!inAction && IsCursorOverObject(hand, buttons)) { hand.Hovering(); }
            //else { hand.Release(); inAction = false; }
//#endif

#if (CLAPPING && HOVER)
            //if (!inAction && IsHandsTogetherOnAButton(hand, otherHand, buttons))
            //{
            //    // call the click event of the selected button
            //    _selectedButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, _selectedButton));
            //}
            //else if (!inAction && IsCursorOverObject(hand, buttons)) { hand.Hovering(); }
            //else { hand.Release(); inAction = false; }
#endif
                //Canvas.SetLeft(hand, x - (hand.ActualWidth / 2));
                //hand.Dispatcher.Invoke(new Action(delegate() { hand.
                object[] param1 = {hand, x - (hand.ActualWidth / 2)};
                hand.Dispatcher.Invoke(
                    new Action<HoverButton, double>(
                        delegate(HoverButton h, double p)
                        {
                            Canvas.SetLeft(h, p);
                        }),
                    param1
                );
                //Canvas.SetTop(hand, y - (hand.ActualHeight / 2));
                object[] param2 = { hand, y - (hand.ActualHeight / 2) };
                hand.Dispatcher.Invoke(
                    new Action<HoverButton, double>(
                        delegate(HoverButton h, double p)
                        {
                            Canvas.SetTop(h, p);
                        }),
                    param2
                );
        }

        /// <summary>
        /// Checks to see if the kinect cursor is over an object.
        /// </summary>
        /// <param name="hand">The Kinect cursor</param>
        /// <param name="buttons">The list of buttons to check against</param>
        /// <returns>Returns true if the cursor is over a button. False if not</returns>
        public static bool IsCursorOverObject(FrameworkElement hand, List<Button> buttons)
        {
            // get the location of the top left of the hand and then use it to find the middle of the hand
            var handTopLeft = new Point(Canvas.GetTop(hand), Canvas.GetLeft(hand));
            _handLeft = handTopLeft.X + (hand.ActualWidth / 2);
            _handTop = handTopLeft.Y + (hand.ActualHeight / 2);

            foreach (Button target in buttons)
            {
                Point targetTopLeft = target.PointToScreen(new Point());
                if (_handTop > targetTopLeft.X
                    && _handTop < targetTopLeft.X + target.ActualWidth
                    && _handLeft > targetTopLeft.Y
                    && _handLeft < targetTopLeft.Y + target.ActualHeight)
                {
                    _selectedButton = target;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks to see if both hands are together in a clapping motion over a button.
        /// </summary>
        /// <param name="hand">The first hand</param>
        /// <param name="otherHand">The second hand</param>
        /// <param name="buttons">The list of buttons to check against</param>
        /// <returns></returns>
        public static bool IsHandsTogetherOnAButton(FrameworkElement hand, FrameworkElement otherHand, List<Button> buttons)
        {
            // get the location of the top left of the hand and then use it to find the middle of the hand
            //var handTopLeft = new Point(Canvas.GetTop(hand), Canvas.GetLeft(hand));
            //_handLeft = handTopLeft.X + (hand.ActualWidth / 2);
            //_handTop = handTopLeft.Y + (hand.ActualHeight / 2);

            //var otherHandTopLeft = new Point(Canvas.GetTop(otherHand), Canvas.GetLeft(otherHand));
            //double _otherHandLeft = otherHandTopLeft.X + (otherHand.ActualWidth / 2);
            //double _otherHandTop = otherHandTopLeft.Y + (otherHand.ActualHeight / 2);

            //var _scaleFactor = 40;
            //if (MotionEngine.getInstance().HandsTogether(_scaleFactor))
            //{
            //    foreach (Button target in buttons)
            //    {
            //        Point targetTopLeft = target.PointToScreen(new Point());
            //        if (_handTop > targetTopLeft.X
            //            && _handTop < targetTopLeft.X + target.ActualWidth
            //            && _handLeft > targetTopLeft.Y
            //            && _handLeft < targetTopLeft.Y + target.ActualHeight)
            //        {
            //            _selectedButton = target;
            //            return true;
            //        }
            //    }
            //}
            return false;
        }

        #endregion Motion

        #region Voice

        /// <summary>
        /// The main method for speech recognition. If an utterance is recognised
        /// by the SpeechEngine, this handler is called with the command. The handler
        /// checks the button names for the matching button and calls its Click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void SpeechRecognized(object sender, SpeechRecognizedEventArgs args)
        {
            Console.WriteLine("Received speech event with command: " + args.Result.Text);

            String command = args.Result.Text.Remove(0,"kinect ".Length);
            command = command.Replace(' ', '_');

            /*
             * Check each button name. If it matches the command,
             * raise an event.
             */
            foreach (Button button in buttons)
            {
                if (button.Name.Equals(command, StringComparison.OrdinalIgnoreCase))
                {
                    button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, button));
                }
            }
        }

        /// <summary>
        /// The main method for speech rejection. Used to notify the user of a rejected speech
        /// utterance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs args)
        {
            Console.WriteLine("Speech rejected with args: " + args.Result.Text);
        }

        void runtime_VideoFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            //pull out the video frame from the eventargs and load it into our image object
            //PlanarImage image = e.ImageFrame.Image;
            ColorImageFrame image = e.OpenColorImageFrame();
            if (image == null) return;
            Byte[] pixels = new Byte[image.PixelDataLength];
            image.CopyPixelDataTo(pixels);
            BitmapSource source = BitmapSource.Create(image.Width, image.Height, 96, 96,
                PixelFormats.Bgr32, null, pixels, image.Width * image.BytesPerPixel);
            liveVideo.Source = source;
        }

        #endregion Voice

    }
}
