using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.Game3DPlay.Core.Elements
{
    public class BaseProcessElement : Element
    {
        public BaseProcessElement(ISpelObject nParent)
            : base(nParent)
        {

        }

        public virtual void OnProcess(object sender, ProcessEventArgs e)
        {

        }


    }
}
