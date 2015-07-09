using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace TreeGenerator.LodEngine
{
    public class TWRenderer
    {
        private IXNAGame game;

        public IXNAGame Game
        {
            get { return game; }
        }

        private List<TWRenderElement> elements=new List<TWRenderElement>();

        public TWRenderer(IXNAGame game)
        {
            this.game = game;
        }

        private int refCount;
        public TWRenderElement CreateElement(IRenderable renderData)
        {
            TWRenderElement element = new TWRenderElement(renderData);
            elements.Add(element);

            refCount++;
            return element;
        }

        public void ReleaseElement(TWRenderElement renderELement)
        {
            if (!elements.Remove(renderELement))
                throw new Exception("Debug exception! Element has already been released or is not in the list");
            refCount--;
        }


        public void Render()
        {
            GraphicsDevice device = game.GraphicsDevice;
            RenderState renderState = device.RenderState;

            device.RenderState.PointSpriteEnable = false;


            //device.RenderState.CullMode = CullMode.None;
            device.RenderState.AlphaTestEnable = false;
            device.RenderState.AlphaBlendEnable = false;
            //game.SetCamera(game.SpectaterCamera);
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].RenderData.SetWorldMatrix(elements[i].WorldMatrix);
                elements[i].RenderData.Render(game);
            }
        }

        
    }
}
