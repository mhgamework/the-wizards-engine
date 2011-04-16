using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Networking;
using Microsoft.Xna.Framework;
using TreeGenerator.TreeEngine;

namespace TreeGenerator.EngineSynchronisation
{
    public struct TreePacket : INetworkPacket
    {
        public float PosX;
        public float PosY;
        public float PosZ;
        public float Rotation;
        public int Seed;
        public Guid Guid;
        //public TreePacket(Vector3 position,float rotation,int seed, TreeType treeType)
        //{
        //    //posX = position.X;
        //    //posY = position.Y;
        //    //posZ = position.Z;
        //    //this.rotation = rotation;
        //    //this.seed = seed;
        //    //secretCodeName = treeType.GetByteID();
        //}

        public TreePacket(EngineTree tree)
        {
            PosX = tree.Position.X;
            PosY = tree.Position.Y;
            PosZ = tree.Position.Z;
            Rotation = tree.Rotation;
            Seed = tree.Seed;
            Guid = tree.TreeType.Guid;
        }
    }
}
