using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core.Elements
{
    public class LoadElementOud : BaseLoadElement
    {
		public LoadElementOud(ILoadable nParent)
            : base(nParent)
        {

        }

        public override void OnLoad(object sender, LoadEventArgs e)
        {
            base.OnLoad(sender, e);
			((ILoadable)Parent).OnLoad(sender, e);
        }

		public override void OnUnload(object sender, LoadEventArgs e)
		{
			base.OnUnload(sender, e);
			((ILoadable)Parent).OnUnload(sender, e);
		}
       
    }
}
