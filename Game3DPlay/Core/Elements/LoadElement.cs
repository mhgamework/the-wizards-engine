using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core.Elements
{
	public class LoadElement : BaseLoadElement, IElementContainer
	{
		public LoadElement(ILoadable nParent)
			: base(nParent)
		{
			_elements = new ElementContainer<BaseLoadElement>();

			nParent.AddIElementContainer(this);

		}

		ElementContainer<BaseLoadElement> _elements;

		public Element GetElement(int index) { return _elements.GetElement(index); }
		public void AddElement(Element IE)
		{
			_elements.AddElement(IE, this);
		}
		public void RemoveElement(Element IE)
		{
			_elements.RemoveElement(IE,this);

		}

		public bool AcceptsElement(Element IE)
		{ if (IE is BaseLoadElement) { return true; } else { return false; } }
		public bool TryAdd(Element IE)
		{ if (AcceptsElement(IE)) { AddElement(IE); return true; } else { return false; } }


		public override void OnLoad(object sender, LoadEventArgs e)
		{
			base.OnLoad(sender, e);
			BaseLoadElement IE;
			for (int i = 0; i < _elements.Count; i++)
			{
				IE = _elements.GetElement(i);
				if (IE.Enabled) IE.OnLoad(sender, e);
			}
		}

		public override void OnUnload(object sender, LoadEventArgs e)
		{
			base.OnUnload(sender, e);
			BaseLoadElement IE;
			for (int i = 0; i < _elements.Count; i++)
			{
				IE = _elements.GetElement(i);
				if (IE.Enabled) IE.OnUnload(sender, e);
			}
		}


	}
}
