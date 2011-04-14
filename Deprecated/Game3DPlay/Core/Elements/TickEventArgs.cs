using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.Game3DPlay.Core.Elements
{
    public class TickEventArgs : ProcessEventArgs 
    {

		public TickEventArgs(BaseHoofdObject nHoofdObj)
            : base(nHoofdObj)
        {

        }

    }

}
