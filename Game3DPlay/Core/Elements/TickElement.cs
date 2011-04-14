using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core.Elements
{
    public class TickElement : BaseTickElement
    {
        public TickElement(ITickable nParent)
            : base(nParent)
        {

        }

        public override void OnTick(object sender, TickEventArgs e)
        {
            base.OnTick(sender, e);
            ((ITickable)Parent).OnTick(sender, e);
        }
       
    }
}
