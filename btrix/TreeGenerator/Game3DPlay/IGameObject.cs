using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public interface IGameObject
    {
        void Render();
        void Process();
    }
}
