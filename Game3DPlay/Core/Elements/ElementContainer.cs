using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core.Elements
{
    public class ElementContainer<T> where T : Element
    {
		public ElementContainer()
        {
			_elements = new List<T>();
        }

		private List<T> _elements;

        public T GetElement(int index) { return _elements[index]; }

        public void AddElement(Element IE, IElementContainer container)
        {
            if (IE.Container != null) { throw new Exception(); }
			if (IE.AcceptContainer(container)) { _elements.Add((T)IE); }
        }

		public void RemoveElement(Element IE, IElementContainer container)
        {
            if (IE.Container != container) { throw new Exception("Not in this container"); }
			if (_elements.Remove((T)IE) == false) { throw new Exception("De Element denkt dat hij in deze container zit maar zit er niet in"); }
            IE.AcceptContainer(null);

        }


		public int Count
		{
			get	{ return _elements.Count; }
		}
       
    }
}
