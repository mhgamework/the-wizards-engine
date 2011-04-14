using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
namespace MHGameWork.Game3DPlay.Core.Elements
{
    public class BaseRender2DElement : BaseRenderElement
    {
        public BaseRender2DElement(ISpelObject nParent)
            : base(nParent)
        {
        }

        private Vector2 _positie;

        public Vector2 Positie
        {
            get { return _positie; }
            set { _positie = value; }
        }

		//public override void OnRender(object sender, RenderEventArgs e)
		//{
		//    base.OnRender(sender, e);
		//    if (Render2D != null) Render2D(sender, e);
		//}

		//public event Render2DEventHandler Render2D;
		//public delegate void Render2DEventHandler(object sender, RenderEventArgs e);

        public override void SendToFront()
        {
            if (Container is Renderer2DContainer)
            {
                ((Renderer2DContainer)Container).SendElementToFront(this);
            }
            else
            {
                base.SendToFront();
            }

        }


    }
}
