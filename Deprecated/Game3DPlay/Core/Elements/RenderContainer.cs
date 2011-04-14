using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core.Elements
{
    public class RenderContainer : BaseRenderElement, IElementContainer
    {
        public RenderContainer(SpelObject nParent)
            : base(nParent)
        {
            _elements = new List<BaseRenderElement>();
            nParent.AddIElementContainer(this);
        }

        private List<BaseRenderElement> _elements;

        public BaseRenderElement GetElement(int index) { return _elements[index]; }

        public void AddElement(Element IE)
        {
            if (IE.Container != null) { throw new Exception(); }
            if (IE.AcceptContainer(this)) { _elements.Add((BaseRenderElement)IE); }
        }
        #region IElementContainer Members

        public bool AcceptsElement(Element IE)
        {
            if (IE is BaseRenderElement) { return true; } else { return false; }

        }

        public void RemoveElement(Element IE)
        {
            if (IE.Container != this) { throw new Exception("Not in this container"); }
            if (_elements.Remove((BaseRenderElement)IE) == false) { throw new Exception("De Element denkt dat hij in deze container zit maar zit er niet in"); }
            IE.AcceptContainer(null);

        }

        public bool TryAdd(Element IE)
        {
            if (AcceptsElement(IE)) { AddElement(IE); return true; } else { return false; }

        }

        #endregion


        public override void OnBeforeRender(object sender, RenderEventArgs e)
        {
            base.OnBeforeRender(sender, e);
            BaseRenderElement IE;
            for (int i = _elements.Count - 1; i >= 0; i--)
            {
                IE = _elements[i];
                if (IE.Enabled) IE.OnBeforeRender(sender, e);
            }
        }
        public override void OnRender(object sender, RenderEventArgs e)
        {
            base.OnRender(sender, e);
            BaseRenderElement IE;
            for (int i = _elements.Count - 1; i >= 0; i--)
            {
                IE = _elements[i];
                if (IE.Enabled) IE.OnRender(sender, e);
            }
        }
        public override void OnAfterRender(object sender, RenderEventArgs e)
        {
            base.OnAfterRender(sender, e);
            BaseRenderElement IE;
            for (int i = _elements.Count - 1; i >= 0; i--)
            {
                IE = _elements[i];
                if (IE.Enabled) IE.OnAfterRender(sender, e);
            }
        }
    }
}
