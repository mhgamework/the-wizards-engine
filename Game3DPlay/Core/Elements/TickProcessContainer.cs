using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core.Elements
{
    public class TickContainer : BaseTickElement, IElementContainer
    {
        public TickContainer(SpelObject nParent)
            : base(nParent)
        {
            _elements = new List<BaseTickElement>();

            nParent.AddIElementContainer(this);

        }

        private List<BaseTickElement> _elements;

        public Element GetElement(int index) { return _elements[index]; }

        public void AddElement(Element IE)
        {
            if (IE.Container != null) { throw new Exception(); }
            if (IE.AcceptContainer(this)) { _elements.Add((BaseTickElement)IE); }
        }
        #region IElementContainer Members

        public bool AcceptsElement(Element IE)
        {
            if (IE is BaseTickElement) { return true; } else { return false; }

        }

        public void RemoveElement(Element IE)
        {
            if (IE.Container != this) { throw new Exception("Not in this container"); }
            if (_elements.Remove((BaseTickElement)IE) == false) { throw new Exception("De Element denkt dat hij in deze container zit maar zit er niet in"); }
            IE.AcceptContainer(null);

        }

        public bool TryAdd(Element IE)
        {
            if (AcceptsElement(IE)) { AddElement(IE); return true; } else { return false; }

        }

        #endregion

        public override void OnTick(object sender, TickEventArgs e)
        {
            base.OnTick(sender, e);
            BaseTickElement IE;
            for (int i = 0; i < _elements.Count; i++)
            {
                IE = _elements[i];
                if (IE.Enabled) IE.OnTick(sender, e);
            }
        }
    }
}
