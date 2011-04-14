using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core.Elements
{
    public class BaseRenderElement : Element
    {
        public BaseRenderElement(ISpelObject nParent)
            : base(nParent)
        {
            _visible = false;
        }

        private bool _visible;
        public bool Visible { get { return _visible; } set { _visible = value; } }

        public virtual void OnBeforeRender(object sender, RenderEventArgs e) { }
        public virtual void OnRender(object sender, RenderEventArgs e) { }
        public virtual void OnAfterRender(object sender, RenderEventArgs e) { }

        public virtual void SendToFront()
        {
            /*If TypeOf Me.IContainer Is RenderContainer Then
                CType(Me.IContainer, RenderContainer).SendElementToFront(Me)
            Else
                Stop
            End If*/
        }
        
    }
}
