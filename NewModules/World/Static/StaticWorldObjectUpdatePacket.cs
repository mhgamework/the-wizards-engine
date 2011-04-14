using System;
using MHGameWork.TheWizards.Networking;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.World.Static
{
    public class StaticWorldObjectUpdatePacket : INetworkPacket
    {
        public int ID;
        public Guid MeshGuid;
        public float[] WorldMatrix;
    }
}
