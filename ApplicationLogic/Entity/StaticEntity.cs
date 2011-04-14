using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.World.Static;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Entity
{
    public class StaticEntity
    {
        public Matrix WorldMatrix;
        public IMesh Mesh;
        private ServerStaticWorldObject staticWorldObject;

        public StaticEntity()
        {
        }


        public void Init(ServerStaticWorldObjectSyncer syncer)
        {
            staticWorldObject = syncer.CreateNew();
            staticWorldObject.Mesh = Mesh;
            staticWorldObject.WorldMatrix = WorldMatrix;

            
        }
    }
}
