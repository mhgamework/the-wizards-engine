using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core
{
	public class Element : IElement, IDisposable
	{
		public Element(ISpelObject nParent)
		{
			nParent.AddElement(this);
			LinkToContainer();

		}

		private ISpelObject _parent;
		public ISpelObject Parent
		{
			get { return _parent; }
			private set { _parent = value; }
		}
		private IElementContainer _container;
		public IElementContainer Container { get { return _container; } }

		public void LinkToContainer()
		{
			if (Container != null)
			{
				Unlink();
			}
			Parent.LinkElement(this);
		}

		public void Unlink()
		{
			if (Container != null)
			{
				Container.RemoveElement(this);
				_container = null;
			}
		}

		public bool AcceptContainer(IElementContainer IEC)
		{
			_container = IEC;
			return true;
		}

		public void AcceptParent(ISpelObject nParent)
		{
			_parent = nParent;
		}

		private bool _enabled;
		public bool Enabled
		{
			get
			{
				return Parent.Enabled;
			}
			set { _enabled = value; }
		}



		public virtual void Initialize()
		{
			LoadGraphicsContent(true);
		}

		/// <summary>
		/// Load your graphics content.  If loadAllContent is true, you should
		/// load content from both ResourceManagementMode pools.  Otherwise, just
		/// load ResourceManagementMode.Manual content.
		/// </summary>
		/// <param name="loadAllContent">Which type of content to load.</param>
		public virtual void LoadGraphicsContent(bool loadAllContent)
		{
		}


		/// <summary>
		/// Unload your graphics content.  If unloadAllContent is true, you should
		/// unload content from both ResourceManagementMode pools.  Otherwise, just
		/// unload ResourceManagementMode.Manual content.  Manual content will get
		/// Disposed by the GraphicsDevice during a Reset.
		/// </summary>
		/// <param name="unloadAllContent">Which type of content to unload.</param>
		public virtual void UnloadGraphicsContent(bool unloadAllContent)
		{
		}





		#region IDisposable Members

		public virtual void Dispose()
		{
			Unlink();

		}

		#endregion






	}
}
