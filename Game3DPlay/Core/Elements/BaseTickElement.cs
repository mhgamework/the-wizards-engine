using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.Game3DPlay.Core.Elements
{
    public class BaseTickElement : Element
    {
		public BaseTickElement(ISpelObject nParent)
            : base(nParent)
        {

        }

        public virtual void OnTick(object sender, TickEventArgs e)
        {

        }


    }
}
