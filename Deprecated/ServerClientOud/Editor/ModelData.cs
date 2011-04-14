using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public class ModelData
    {
        public Matrix LocalMatrix;

        public VertexBuffer vertexBuffer;
        public IndexBuffer indexBuffer;
        public VertexDeclaration vertexDeclaration;
	
        public ModelData()
        {
        }
    }
}
