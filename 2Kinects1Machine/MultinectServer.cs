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

namespace _2Kinects1Machine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MultinectServer : Microsoft.Xna.Framework.Game
    {
        //remove these two after tech demo
        GraphicsDeviceManager graphics;


        SocketServerClass serv;

        // SpriteBatch spriteBatch;
        //-----------------------------------

        public MultinectServer()
        {
            //remove this after tech demo
            graphics = new GraphicsDeviceManager(this);
            serv = new SocketServerClass();
        }

        /// <summary>
        /// Recieves input from the client processes, 
        /// and then does stuff with it.
        /// </summary>
        protected void pipeFromClient(Process kinect)
        {

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //initialize process variables
            //kinectClient1 = new Process();
            //kinectClient2 = new Process();

            //DEBUG: Rename once Client Class is handled.
            //run the client process
            //kinectClient1.StartInfo.FileName = "pipeClient.exe";
            //kinectClient2.StartInfo.FileName = "pipeClient.exe";

            //Make new threads to support pipes between the servers and clients


            //BELOW HAS  BEEN COMMENTED OUT. UN COMMENT AFTER TECH DEMO.

            /*
            Console.WriteLine("Spawning server");
            ServerClass server = new ServerClass("D:\\git\\2KinectTechDemo\\KinectClient\\bin\\Debug\\KinectClient.exe", 0);
            Thread KinectThread1 = new Thread(new ThreadStart(server.ThreadProc));
            //Thread KinectThread2 = new Thread(new ParameterizedThreadStart(pipeFromClient));

            //Start the pipe thread with the kinect processes
            KinectThread1.Start();
            */



            base.Initialize();
        }

        /// <summary>
   
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //polling socket here. data returned will be a byte array. Figure out what to do with it here. Should just be an int?
            serv.pollSocket();





            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //black is lame. Let's use something longer. LightGoldenrodYellow
            GraphicsDevice.Clear(Color.DarkSlateGray);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
