using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Animation
{
   public class Animation
   {
       public List<Track> Tracks = new List<Track>();

       /// <summary>
       /// Shouldnt this always be the last keyframe .time?
       /// </summary>
       public float Length;

       public class Track
       {
           public Joint Joint;
           public List<Keyframe> Frames = new List<Keyframe>();
       }
       public class Keyframe
       {
           public float Time;
           public Matrix Value;
       }
    }
}
