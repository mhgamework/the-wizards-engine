using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.World
{
    /// <summary>
    /// Responsible for representing a independent object in the World. It is not part of any greater system, like the building/terrain/trees
    /// </summary>
    public class Entity
    {
        private readonly WorldNoSectors world;

        public Entity(WorldNoSectors world)
        {
            this.world = world;
        }

        public IMesh Mesh { get; set; }
        public Matrix WorldMatrix { get; set; }
        public bool IsDirty { get; set; }
    }
}

