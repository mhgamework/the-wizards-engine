using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient
{
    public class WereldModel
    {
        private TWWereld wereld;

        public TWWereld Wereld
        {
            get { return wereld; }
            set { wereld = value; }
        }

        private Matrix worldMatrix;

        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }

        private WereldMesh mesh;

        public WereldMesh Mesh
        {
            get { return mesh; }
            set { mesh = value; }
        }


        public WereldModel Clone()
        {
            WereldModel model = new WereldModel();
            model.worldMatrix = worldMatrix;
            model.mesh = mesh;

            return model;
        }

    }
}
