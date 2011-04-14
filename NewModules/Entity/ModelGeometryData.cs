using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Entity
{
    [Obsolete]
    public class ModelGeometryData
    {
        private Microsoft.Xna.Framework.Vector3[] positions;

        public Microsoft.Xna.Framework.Vector3[] Positions
        {
            get { return positions; }
            set { positions = value; }
        }

        private Vector3[] normals;

        public Vector3[] Normals
        {
            get { return normals; }
            set { normals = value; }
        }

        private Vector3[] tangents;

        public Vector3[] Tangents
        {
            get { return tangents; }
            set { tangents = value; }
        }

        private Vector3[] texCoords;

        public Vector3[] TexCoords
        {
            get { return texCoords; }
            set { texCoords = value; }
        }


        public int TriangleCount;
    }
}
