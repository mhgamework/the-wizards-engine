using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core.Elements
{
    public interface ITickable : ISpelObject
    {
        void OnTick(object sender, TickEventArgs e);



    }
}
