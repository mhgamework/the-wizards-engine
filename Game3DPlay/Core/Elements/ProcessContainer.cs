using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core.Elements
{
    public class ProcessContainer : BaseProcessElement, IElementContainer
    {
        public ProcessContainer(SpelObject nParent)
            : base(nParent)
        {
            _elements = new List<BaseProcessElement>();

            nParent.AddIElementContainer(this);

        }

        private List<BaseProcessElement> _elements;

        public Element GetElement(int index) { return _elements[index]; }

        public void AddElement(Element IE)
        {
            if (IE.Container != null) { throw new Exception(); }
            if (IE.AcceptContainer(this)) { _elements.Add((BaseProcessElement)IE); }
        }
        #region IElementContainer Members

        public bool AcceptsElement(Element IE)
        {
            if (IE is BaseProcessElement) { return true; } else { return false; }

        }

        public void RemoveElement(Element IE)
        {
            if (IE.Container != this) { throw new Exception("Not in this container"); }
            if (_elements.Remove((BaseProcessElement)IE) == false) { throw new Exception("De Element denkt dat hij in deze container zit maar zit er niet in"); }
            IE.AcceptContainer(null);

        }

        public bool TryAdd(Element IE)
        {
            if (AcceptsElement(IE)) { AddElement(IE); return true; } else { return false; }

        }

        #endregion

        public override void OnProcess(object sender, ProcessEventArgs e)
        {
            base.OnProcess(sender, e);
            BaseProcessElement IE;
            for (int i = 0; i < _elements.Count; i++)
            {
                IE = _elements[i];
                if (IE.Enabled) IE.OnProcess(sender, e);
            }
        }
    }
}
