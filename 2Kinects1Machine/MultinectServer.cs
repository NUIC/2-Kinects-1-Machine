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
        // SpriteBatch spriteBatch;
        //-----------------------------------

       Process kinectClient1;
       Process kinectClient2;

        public MultinectServer()
        {
            //remove this after tech demo
            graphics = new GraphicsDeviceManager(this);
        }

        protected void pipeFromClient(Object kinect)
        {
            using (AnonymousPipeServerStream pipeServer =
                new AnonymousPipeServerStream(PipeDirection.In,
                HandleInheritability.Inheritable))
            {
                // Pass the client process a handle to the server.
                kinectClient2.StartInfo.Arguments =
                    pipeServer.GetClientHandleAsString();
                kinectClient2.StartInfo.UseShellExecute = false;
                kinectClient2.Start();

                pipeServer.DisposeLocalCopyOfClientHandle();

                try
                {
                    // Read user input and send that to the client process.
                    using (StreamWriter sw = new StreamWriter(pipeServer))
                    {
                        sw.AutoFlush = true;
                        // Send a 'sync message' and wait for client to receive it.
                        sw.WriteLine("SYNC");
                        pipeServer.WaitForPipeDrain();
                        // Send the console input to the client process.
                        Console.Write("[SERVER] Enter text: ");
                        sw.WriteLine(Console.ReadLine());
                    }
                }
                // Catch the IOException that is raised if the pipe is broken
                // or disconnected.
                catch (IOException e)
                {
                    Console.WriteLine("[SERVER] Error: {0}", e.Message);
                }
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            kinectClient1 = new Process();
            kinectClient2 = new Process();

            //DEBUG: Rename once Client Class is handled.
            kinectClient1.StartInfo.FileName = "pipeClient.exe";
            kinectClient2.StartInfo.FileName = "pipeClient.exe";


            Thread KinectThread1 = new Thread(new ParameterizedThreadStart(pipeFromClient));
            Thread KinectThread2 = new Thread(new ParameterizedThreadStart(pipeFromClient));

            KinectThread1.Start(kinectClient1);
            KinectThread2.Start(kinectClient2);


            base.Initialize();
        }

        /// <summary>
   
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

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
