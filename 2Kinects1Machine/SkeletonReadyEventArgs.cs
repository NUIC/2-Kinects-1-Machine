using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace _2Kinects1Machine
{
    class SkeletonReadyEventArgs : EventArgs
    {

        Skeleton skeleton;

        public SkeletonReadyEventArgs(Skeleton s)
        {
            skeleton = s;
        }

        public Skeleton getSkeleton()
        {
            return skeleton;
        }
    }
}
