using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Scripting.API
{
    public interface IScript
    {
        void Init(IEntityHandle handle);
        void Destroy();
    }
}
