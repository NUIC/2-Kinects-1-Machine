using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using Microsoft.Kinect;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace _2Kinects1Machine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MultKinectServer : Microsoft.Xna.Framework.Game
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MessageBox(IntPtr hWnd, String text, String caption, uint type);

        public static Dictionary<string, Skeleton[]> skeletonDict;

        Texture2D blank;

        //remove these two after tech demo
        GraphicsDeviceManager graphics;

        TCPServerClass serv;
        //List<ServerClass> servers;
        List<Thread> threads;

        internal static Mutex playerList = new Mutex(false);
        internal static Skeleton[] players = new Skeleton[6];

        SpriteBatch spriteBatch;

        public MultKinectServer()
        {
            graphics = new GraphicsDeviceManager(this);
            threads = new List<Thread>();
            //players = new List<Skeleton>();
            playerList = new Mutex(false);
            skeletonDict = new Dictionary<string, Skeleton[]>();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            blank = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            blank.SetData(new[] { Color.White });

            base.LoadContent();
        }

        /// <summary>
        /// Recieves input from the client processes, 
        /// and then does stuff with it.
        /// </summary>
        protected void pipeFromClient(Process kinect)
        {

        }

        //void SkeletonDataReady(object sender, SkeletonReadyEventArgs args)
        //{
        //    Mutex[] gm = {playerList};
        //    Mutex.WaitAll(gm, 40);

        //    if (players.Length == 0)
        //    {
        //        players.Add(args.getSkeleton());
        //        gm[0].ReleaseMutex();
        //        return;
        //    }

        //    for (int i = 0; i < players.Count; i++ )
        //    {
        //        if (players[i].TrackingId.Equals(args.getSkeleton()))
        //        {

        //            players[i] = args.getSkeleton();
        //            gm[0].ReleaseMutex();
        //            return;
        //        }
        //    }

        //    if (players.Count < 4)
        //    {
        //        players.Add(args.getSkeleton());
        //        gm[0].ReleaseMutex();
        //    }

        //}

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            if (KinectSensor.KinectSensors.Count > 0)
            {
                foreach (KinectSensor sensor in KinectSensor.KinectSensors)
                {
                    if (sensor.Status == KinectStatus.Connected)
                    {
                        ServerClass server = new ServerClass("D:\\git\\2KinectTechDemo\\KinectClient\\bin\\Debug\\KinectClient.exe", sensor.UniqueKinectId);

                        //server.skeletonEvents += SkeletonDataReady;
                        Thread kinectThread = new Thread(new ThreadStart(server.ThreadProc));
                        threads.Add(kinectThread);
                        skeletonDict[sensor.UniqueKinectId] = new Skeleton[2];
                        kinectThread.Start();
                    }
                }
            }

            //// Open a 
            //TCPServerClass socketServer = new TCPServerClass();
            ////socketServer.skeletonEvents += SkeletonDataReady;
            //Thread socketThread = new Thread(new ThreadStart(socketServer.ThreadProc));
            //threads.Add(socketThread);
            //socketThread.Start();

            base.Initialize();
        }

        //private static System.Object skeletonLock = new Object();
        private static int count = 0;

        //[MethodImplAttribute(MethodImplOptions.Synchronized)]
        internal static void updateSkeleton(Skeleton skeleton)
        {
            lock (players)
            {

                for (int i = 0; i < MultKinectServer.players.Length; i++)
                {
                    if (MultKinectServer.players[i] != null && MultKinectServer.players[i].TrackingId.Equals(skeleton))
                    {
                        Console.WriteLine("[Server] updating skeleton at position {0}", i);
                        MultKinectServer.players[i] = skeleton;
                        return;
                    }
                }

                if (count < 4)
                {
                    //Console.WriteLine("[Server] assigning skeletong to position {0}", count);
                    MultKinectServer.players[count++] = skeleton;//.Add(skeleton);
                }
            }
        }

        internal static void updateSkeletons(Skeleton[] skeletons, string key)
        {
            lock (skeletonDict)
            {
                Console.WriteLine("Updating skeleton");
                skeletonDict[key] = skeletons;
            }
        }

        /// <summary>
   
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        //[MethodImplAttribute(MethodImplOptions.Synchronized)]
        protected override void Draw(GameTime gameTime)
        {
            //black is lame. Let's use something longer. LightGoldenrodYellow
            GraphicsDevice.Clear(Color.DarkSlateGray);

            //Mutex[] gm = { playerList };
            //Mutex.WaitAll(gm, 40);

            lock (players)
            {
                foreach (Skeleton s in players)
                {
                    if (s != null)
                    {
                        Console.WriteLine("Drawing");
                        drawSkeleton(s);
                    }
                }
            }
            //gm[0].ReleaseMutex();


            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        private void addLine(Joint joint, Joint joint_2)
        {

            DrawLine(this.spriteBatch, blank, 3, Color.Yellow, new Vector2(joint.Position.X * (-150) + 400, joint.Position.Y * (-150) + 400), new Vector2(joint_2.Position.X * (-150) + 400, joint_2.Position.Y * (-150) + 400));

        }

        void DrawLine(SpriteBatch batch, Texture2D blank,
              float width, Color color, Vector2 point1, Vector2 point2)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            batch.Draw(blank, point1, null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 0);

        }

        void drawSkeleton(Skeleton skeleton)
        {
            spriteBatch.Begin();

            if (skeleton != null && skeleton.TrackingState == SkeletonTrackingState.Tracked)
            {
                //Console.WriteLine("[Server] Drawing skeletong with id {0}", skeleton.TrackingId);
                Joint headJoint = skeleton.Joints[JointType.Head];
                Joint hipCenter = skeleton.Joints[JointType.HipCenter];

                if (headJoint.TrackingState != JointTrackingState.NotTracked)
                {
                    SkeletonPoint headPosition = headJoint.Position;

                    //HeadPositionPrintline wrecks code efficiency!!!
                    //Console.WriteLine(skeleton.Joints[JointType.Head].Position.Y);

                    // Spine
                    addLine(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter]);
                    addLine(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.Spine]);

                    // Left leg
                    addLine(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.HipCenter]);
                    addLine(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipLeft]);
                    addLine(skeleton.Joints[JointType.HipLeft], skeleton.Joints[JointType.KneeLeft]);
                    addLine(skeleton.Joints[JointType.KneeLeft], skeleton.Joints[JointType.AnkleLeft]);
                    addLine(skeleton.Joints[JointType.AnkleLeft], skeleton.Joints[JointType.FootLeft]);

                    // Right leg
                    addLine(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipRight]);
                    addLine(skeleton.Joints[JointType.HipRight], skeleton.Joints[JointType.KneeRight]);
                    addLine(skeleton.Joints[JointType.KneeRight], skeleton.Joints[JointType.AnkleRight]);
                    addLine(skeleton.Joints[JointType.AnkleRight], skeleton.Joints[JointType.FootRight]);

                    // Left arm
                    addLine(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderLeft]);
                    addLine(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
                    addLine(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
                    addLine(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.HandLeft]);

                    // Right arm
                    addLine(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderRight]);
                    addLine(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);
                    addLine(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
                    addLine(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.HandRight]);

                }


            }
            spriteBatch.End();

        }
    }
}
