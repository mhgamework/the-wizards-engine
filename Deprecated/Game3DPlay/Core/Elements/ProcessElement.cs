using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core.Elements
{
    public class ProcessElement : BaseProcessElement
    {
        public ProcessElement(IProcessable nParent)
            : base(nParent)
        {

        }

        public override void OnProcess(object sender, ProcessEventArgs e)
        {
            base.OnProcess(sender, e);
            ((IProcessable)Parent).OnProcess(sender, e);
        }
       
    }
}
