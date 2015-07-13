using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics.Xna.Graphics
{
    public static class TangentVertexExtensions
    {
        public static VertexDeclaration CreateVertexDeclaration(IXNAGame game)
        {
            return new VertexDeclaration(game.GraphicsDevice, TangentVertex.VertexElements);
        }
    }
}