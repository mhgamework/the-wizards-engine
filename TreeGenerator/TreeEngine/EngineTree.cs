using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TreeGenerator.TreeEngine
{
    public class EngineTree
    {
        public Vector3 Position= Vector3.Zero;
        public Matrix WorldMatrix=Matrix.Identity;
        public ITreeType TreeType;
        public float Rotation = 0;// just added this to make the synchronisation between server client easier
        public int Seed=123;
        public EngineTree(Vector3 pos,float rotation,ITreeType treeType,int seed)
        {
            Position = pos;
            WorldMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, rotation);//when I rotate the tree in the further design there has to be the posibility to rotate against an other vector cause the trees won't be up always
            WorldMatrix *= Matrix.CreateTranslation(pos);
            Rotation = rotation;
            Seed = seed;
            TreeType=treeType;
        }
    }
}
