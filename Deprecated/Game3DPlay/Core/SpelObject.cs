using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core
{
    public class SpelObject : IDisposable, ISpelObject
    {
        public SpelObject(ISpelObject nParent)
            : this()
        {
            //Parent = nParent;
            nParent.AddChild(this);
            HoofdObj.RegisterNewSpO(this);

            HoofdObj.CreateStandardElements(this);
        }

        protected SpelObject()
        {
            _children = new List<SpelObject>();
            _elements = new List<Element>();
            _containers = new List<IElementContainer>();
            _enabled = true;

        }
        private ISpelObject _Parent;
        public ISpelObject Parent
        {
            get { return _Parent; }
            private set
            {
				if (_Parent == value) return;
				ValueChangedEventArgs<ISpelObject> e = new ValueChangedEventArgs<ISpelObject>(_Parent, value);
                _Parent = value;
                if (_Parent == null)
                {
                    HoofdObj = null;
                }
                else
                {
                    HoofdObj = value.HoofdObj;
                }
				if (ParentChanged != null) ParentChanged(this, e);
            }
        }
		
		public event ValueChangedEventHandler<ISpelObject> ParentChanged;

		private BaseHoofdObject _hoofdObj;
		public BaseHoofdObject HoofdObj
        {
            get { return _hoofdObj; }
            protected set { _hoofdObj = value; }
        }

        private bool _enabled;
        public bool Enabled
        {
            //get { if (ParentEnabled == false) return false; return _enabled; }
            get { return _enabled; }
            set { _enabled = value; }
        }


        private bool _parentEnabled;
        public bool ParentEnabled
        {
            get { return _parentEnabled; }
            set
            {
                if (ParentEnabled = value) return;
                _parentEnabled = value;

                SetParentEnabledRecursive(value);
            }
        }

        public void SetParentEnabledRecursive(bool nParentEnabled)
        {
            // TODO
        }


        protected List<SpelObject> _children;

        public ISpelObject GetChild(int index)
        {
            return _children[index];
        }

        public virtual void AddChild(SpelObject nSpO)
        {
			if (nSpO.Parent != null) throw new Exception("nSpo zit al in een SpelObject!");
            _children.Add(nSpO);
            nSpO.Parent = this;
        }

		public virtual void RemoveChild(SpelObject nSpO)
		{
			if (nSpO.Parent != this) throw new Exception("Dit SpelObject is niet de parent van nSpo!");
			_children.Remove(nSpO);
			nSpO.Parent = null;
		}

        protected List<Element> _elements;

        public Element GetElement(int index)
        {
            return _elements[index];
        }

        public void AddElement(Element IE)
        {
            if (IE.Parent != null) throw new Exception("IE zit al in een SpelObject!");
            _elements.Add(IE);
            IE.AcceptParent(this);

        }

        public T GetElement<T>() where T : Element
        {
            for (int i = 0; i < _elements.Count; i++)
            {
                if (_elements[i] is T) return (T)_elements[i];

            }
            return null;
        }

        public void LinkElement(Element IE)
        {
            for (int i = 0; i < _containers.Count; i++)
            {
                if (_containers[i] != IE)
                {
                    if (_containers[i].TryAdd(IE)) return;
                }
            }
            if (Parent != null) Parent.LinkElement(IE);
        }

        private List<IElementContainer> _containers;


        public void AddIElementContainer(IElementContainer IEC)
        {
            if (IEC.Parent != this) throw new Exception("Deze IContainer zit met zijn element gedeelte in een ander SpO dan dit.");
            _containers.Add(IEC);


        }

        public void UpdateElementLinks()
        {
            for (int i = 0; i < _elements.Count; i++)
            {
                _elements[i].LinkToContainer();
            }
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].UpdateElementLinks();
            }
        }


        #region IDisposable Members

        public virtual void Dispose()
        {

            if (_children != null)
            {
                for (int i = 0; i < _children.Count; i++)
                {
                    _children[i].Dispose();
                }
                //Children.Clear();
                _children = null;
            }

            if (_elements != null)
            {
                for (int i = 0; i < _elements.Count; i++)
                {
                    _elements[i].Dispose();
                }
                //Elements.Clear();
                _elements = null;
            }
        }

        #endregion


        public virtual void Initialize()
        {
            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Initialize();
            }
            for (int i = 0; i < _elements.Count; i++)
            {
                _elements[i].Initialize();
            }
        }

		///// <summary>
		///// Load your graphics content.  If loadAllContent is true, you should
		///// load content from both ResourceManagementMode pools.  Otherwise, just
		///// load ResourceManagementMode.Manual content.
		///// </summary>
		///// <param name="loadAllContent">Which type of content to load.</param>
		//public virtual void LoadGraphicsContent(bool loadAllContent)
		//{
		//    for (int i = 0; i < _children.Count; i++)
		//    {
		//        _children[i].LoadGraphicsContent(loadAllContent);
		//    }
		//    for (int i = 0; i < _elements.Count; i++)
		//    {
		//        _elements[i].LoadGraphicsContent(loadAllContent);
		//    }
		//}


		///// <summary>
		///// Unload your graphics content.  If unloadAllContent is true, you should
		///// unload content from both ResourceManagementMode pools.  Otherwise, just
		///// unload ResourceManagementMode.Manual content.  Manual content will get
		///// Disposed by the GraphicsDevice during a Reset.
		///// </summary>
		///// <param name="unloadAllContent">Which type of content to unload.</param>
		//public virtual void UnloadGraphicsContent(bool unloadAllContent)
		//{
		//    for (int i = 0; i < _children.Count; i++)
		//    {
		//        _children[i].UnloadGraphicsContent(unloadAllContent);
		//    }
		//    for (int i = 0; i < _elements.Count; i++)
		//    {
		//        _elements[i].UnloadGraphicsContent(unloadAllContent);
		//    }
		//}





    }
}
