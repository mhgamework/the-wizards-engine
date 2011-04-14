using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core.Elements
{
    public interface ILoadable : ISpelObject
    {
        void OnLoad(object sender, LoadEventArgs e);
		void OnUnload(object sender, LoadEventArgs e);



    }
}
