﻿using System;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Persistence;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Synchronization;
using SlimDX;

namespace MHGameWork.TheWizards.WorldRendering
{
    /// <summary>
    /// Responsible for representing a independent object in the World. It is not part of any greater system, like the building/TerrainChunk/trees
    /// </summary>
    [Persist]
    [ModelObjectChanged]
    public class Entity : EngineModelObject
    {
        public Entity()
        {
            WorldMatrix = Matrix.Identity;
            Visible = true;
            Static = true;
        }


        public IMesh Mesh { get; set; }
        public Matrix WorldMatrix { get; set; }
        public bool Visible { get; set; }

        /// <summary>
        /// Physics enabled or not
        /// </summary>
        public bool Solid { get; set; }
        /// <summary>
        /// UNTESTED! Kinematic physics property
        /// </summary>
        public bool Kinematic { get; set; }

        /// <summary>
        /// Set to false when this object should be moved by physics
        /// </summary>
        public bool Static { get; set; }

        /// <summary>
        /// This is true when the entity is part of a batched group
        /// </summary>
        public bool Batched { get; set; }

        public override string ToString()
        {
            return string.Format("Mesh: {0}, WorldMatrix: {1}", Mesh, WorldMatrix);
        }
    }
}

