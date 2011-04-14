using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Scripting
{
    public interface IStateScript
    {

        void Init();
        void Destroy();

        void Update();
        void Draw();

    }
}
