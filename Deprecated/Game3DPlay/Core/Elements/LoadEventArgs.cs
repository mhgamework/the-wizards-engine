using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace MHGameWork.Game3DPlay.Core.Elements
{
	public class LoadEventArgs : EventArgs
	{

		public LoadEventArgs(DynamicContentManager nContentManager, bool nAllContent)
			: base()
		{
			ContentManager = nContentManager;
			AllContent = nAllContent;
		}

		private DynamicContentManager _contentManager;

		public DynamicContentManager ContentManager
		{
			get { return _contentManager; }
			set { _contentManager = value; }
		}


		private bool _allContent;

		public bool AllContent
		{
			get { return _allContent; }
			set { _allContent = value; }
		}





	}

}
