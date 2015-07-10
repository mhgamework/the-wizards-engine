using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics.Xna.Graphics
{
    public class SkinnedMesh
    {

        private SkinnedShader shader;
        public SkinnedShader Shader { get { return shader; } }
        
        public Primitives Primitives;

        public void SetWorldMatrix( Matrix worldMatrix )
        {

            //shader.World = worldMatrix;
        }

        public SkinnedMesh( SkinnedModel model )
        {
            shader = model.BaseShader.Clone();

        }

        public void Render()
        {
            shader.RenderPrimitiveSinglePass( Primitives, SaveStateMode.None );
        }

    }
}
