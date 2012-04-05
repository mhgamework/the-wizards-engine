using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Scripting
{
    [Obsolete("Currently obsolete, something similar could be reimplemented later")]
    public interface IStateScript
    {

        void Init();
        void Destroy();

        void Update();
        void Draw();

    }
}
