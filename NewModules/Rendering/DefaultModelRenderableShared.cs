using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Rendering
{
    public class DefaultModelRenderableShared : IXNAObject
    {
        private EffectPool effectPool;
        public DefaultRenderer Renderer { get; private set; }
        public DefaultModelShader Shader { get; private set; }
        public VertexDeclaration VertexDeclaration { get; private set; }

        
        
        public DefaultModelRenderableShared(DefaultRenderer renderer)
        {
            Renderer = renderer;
        }

        public void Initialize(IXNAGame _game)
        {
            VertexDeclaration = Renderer.GetVertexDeclaration<TangentVertex>();

            effectPool = new EffectPool();

            Shader = new DefaultModelShader(_game, effectPool);
        }

        public void Render(IXNAGame _game)
        {
            Shader.ViewProjection = _game.Camera.ViewProjection;
            Shader.ViewInverse = _game.Camera.ViewInverse;
        }

        public void Update(IXNAGame _game)
        {
        }

    }
}
