using System;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class ScatteredPlayer
    {
        public ScatteredPlayer(Level level, SceneGraphNode createChild)
        {
        }

        public Vector3 Position { get; set; }

        /// <summary>
        /// Contains the island the player is currently flying
        /// </summary>
        public Island FlyingIsland { get; set; }
    }
}