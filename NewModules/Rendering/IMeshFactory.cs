using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Rendering
{
    public interface IMeshFactory
    {
        IMesh GetMesh(Guid guid);
    }
}
