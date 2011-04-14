using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core.Elements
{
    public class Render2DElement : BaseRender2DElement
    {
		public Render2DElement(IRenderable2D nParent)
            : base(nParent)
        {

        }

        public override void OnBeforeRender(object sender, RenderEventArgs e)
        {
            base.OnBeforeRender(sender, e);
            ((IRenderable2D)Parent).OnBeforeRender(sender, e);
        }
        public override void OnRender(object sender, RenderEventArgs e)
        {
            base.OnRender(sender, e);
            ((IRenderable2D)Parent).OnRender(sender, e);
        }
        public override void OnAfterRender(object sender, RenderEventArgs e)
        {
            base.OnAfterRender(sender, e);
            ((IRenderable2D)Parent).OnAfterRender(sender, e);
        }

        //public delegate void BeforeRenderHandler(object sender, BaseRenderElement.RenderEventArgs e);
        //public event BeforeRenderHandler BeforeRender;
        //public delegate void RenderHandler(object sender, BaseRenderElement.RenderEventArgs e);
        //public event RenderHandler Render;
        //public delegate void AfterRenderHandler(object sender, BaseRenderElement.RenderEventArgs e);
        //public event AfterRenderHandler AfterRender;


    }
}
