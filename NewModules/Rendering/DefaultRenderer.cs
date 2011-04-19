using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// WARNING: This is obsolete!!??
    /// 
    /// The DefaultRenderer currently represents the main renderer in The Wizards
    /// DefaultRenderer should be generalized to an abstract IRenderer and then a ForwardRenderer, Deferredrenderer, etc.
    /// This class currently contains a VERY default implementation. It is a Very AL class
    /// </summary>
    public class DefaultRenderer : IXNAObject
    {
        private IXNAGame game;

        private Stack<IDefaultRenderable> uninitializedRenderables;
        /// <summary>
        /// Only contains initialized renderables
        /// </summary>
        private List<IDefaultRenderable> renderables;

        private DefaultModelRenderableShared defaultModelRenderableShared;

        public DefaultRenderer()
        {
            defaultModelRenderableShared = new DefaultModelRenderableShared(this);

            renderables = new List<IDefaultRenderable>();
            uninitializedRenderables = new Stack<IDefaultRenderable>();

        }

        public void Initialize(IXNAGame _game)
        {
            game = _game;

            defaultModelRenderableShared.Initialize(_game);

            for (int i = 0; i < renderables.Count; i++)
            {
                renderables[i].Initialize(_game);
            }

            initializeUninitialized();
        }

        public void Render(IXNAGame _game)
        {
            defaultModelRenderableShared.Render(_game);
            if (_game != game) throw new InvalidOperationException();
            for (int i = 0; i < renderables.Count; i++)
            {
                renderables[i].Render(game);
            }
        }

        public void Update(IXNAGame _game)
        {
            defaultModelRenderableShared.Update(_game);
            if (_game != game) throw new InvalidOperationException();

            initializeUninitialized();

            for (int i = 0; i < renderables.Count; i++)
            {
                renderables[i].Update(game);
            }
        }

        private void initializeUninitialized()
        {
            while (uninitializedRenderables.Count > 0)
            {
                var renderable = uninitializedRenderables.Pop();

                renderable.Initialize(game);
                renderables.Add(renderable);
            }
        }

        public VertexDeclaration GetVertexDeclaration<T>()
        {
            // Buffer the vertexdeclarations + return decl for given type T

            return TangentVertex.CreateVertexDeclaration(game);
        }

        public DefaultRenderElement CreateRenderElement(IDefaultRenderable renderable)
        {
            return renderable.CreateRenderElement();
        }

        public DefaultModelRenderable CreateModelRenderable(TangentVertex[] vertices, short[] indices,IDefaultModelMaterial material)
        {
            DefaultModelRenderable renderable = new DefaultModelRenderable(this, defaultModelRenderableShared, vertices, indices, material);

            addNewRenderable(renderable);
            return renderable;
        }

        private void addNewRenderable(IDefaultRenderable renderable)
        {
            uninitializedRenderables.Push(renderable);
        }
    }
}
