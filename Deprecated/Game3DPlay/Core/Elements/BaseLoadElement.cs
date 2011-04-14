using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.Game3DPlay.Core.Elements
{
    public class BaseLoadElement : Element
    {
		public BaseLoadElement(ILoadable nParent)
            : base(nParent)
        {

        }

        public virtual void OnLoad(object sender, LoadEventArgs e)
        {
			((ILoadable)Parent).OnLoad(sender, e);
        }

		public virtual void OnUnload(object sender, LoadEventArgs e)
		{
			((ILoadable)Parent).OnUnload(sender, e);
		}

		public override void Dispose()
		{
			base.Dispose();
			if (Parent.HoofdObj != null)
				if (Parent.HoofdObj.XNAGame._content != null)
					OnUnload(this, new LoadEventArgs(Parent.HoofdObj.XNAGame._content, true));
		}

    }
}
