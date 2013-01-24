using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Physics;

namespace MHGameWork.TheWizards.Engine
{
    /// <summary>
    /// Responsible for providing access to Physics API's in the gameplay layer.
    /// </summary>
    public class PhysicsWrapper : PhysicsEngine
    {
        public void ClearAll()
        {
            ResetScene();
        }
    }
}
