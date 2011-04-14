using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.Game3DPlay.Core.Elements
{
    public interface IProcessable : ISpelObject
    {
        void OnProcess(object sender, ProcessEventArgs e);



    }
}
