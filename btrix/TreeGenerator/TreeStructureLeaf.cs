using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TreeGenerator.help;

namespace TreeGenerator
{
    public class TreeStructureLeaf
    {
        public LevelAndTexureData LevelTextureData;
        public float Length;
        public float Width;

        public List<Directions> Direction= new List<Directions>();
        public float Position;
        public float DistanceFromTrunk;
        public float AxialSplit;

        public bool BillboardLeaf;

        public bool volumetricLeave;
        public bool Flower;
        public int FaceCountWidth;
        public List<float> BendingWidth;
        public float beningWidthChance;

        //public int Index;
        //public int TextureIndex;
    }
}
