using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.ModelContainer
{
    public interface IGameplayPlugin
    {
        void Initialize(Engine engine);
    }
}
