using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.TileEngine
{
    public class TileData
    {
        public IMesh Mesh;
        public Vector3 Dimensions;
        public TileFaceType[] FaceTypes = new TileFaceType[6];
        public bool[] FaceWinding = new bool[6];

        public TileFaceType GetFaceType(TileFace face)
        {
            return FaceTypes[(int)face-1];
        }

        public void SetFaceType(TileFace face, TileFaceType value)
        {
            FaceTypes[(int)face-1] = value;
        }

        public bool GetWinding(TileFace face)
        {
            return FaceWinding[(int)face - 1];
        }

        public void SetWinding(TileFace face, bool value)
        {
            FaceWinding[(int)face - 1] = value;
        }
    }
}
