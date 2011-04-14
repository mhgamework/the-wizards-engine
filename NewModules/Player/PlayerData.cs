using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Player
{
    /// <summary>
    /// Functionality
    /// 
    /// The Look dir for angle (0,0) is (0,0,-1) = forward. The horizontal angle is around the Y-axis and the vertical around the right axis.
    /// </summary>
    public class PlayerData
    {
        public Vector3 Position;

        public float Health;

        public float LookAngleHorizontal;
        public float LookAngleVertical;

        

        public string Name;

    }
}
