using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.World.Static
{
    public class ServerStaticWorldObject
    {
        public bool Change { get; set; }
        public int ID { get; set; }
        private Matrix worldMatrix;
        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set
            {
                if (worldMatrix.Equals(value)) return;
                worldMatrix = value;
                Change = true;
            }
        }

        private IMesh mesh;
        public IMesh Mesh
        {
            get { return mesh; }
            set
            {
                if (mesh == value) return;
                mesh = value;
                Change = true;
            }
        }


        public ServerStaticWorldObject()
        {
        }

    }
}
