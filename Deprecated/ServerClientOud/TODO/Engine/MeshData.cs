using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Engine
{
    public class MeshData
    {
        private ServerClientMainOud engine;

        private IndexBuffer indexBuffer;
        private VertexBuffer vertexBuffer;

        public MeshData(ServerClientMainOud nEngine)
        {
            engine = nEngine;
        }

        
    }
}
