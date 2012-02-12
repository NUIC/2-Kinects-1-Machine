using System;

namespace _2Kinects1Machine
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MultKinectServer game = new MultKinectServer())
            {
                game.Run();
            }
        }
    }
#endif
}