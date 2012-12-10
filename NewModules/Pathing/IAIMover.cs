using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Pathing
{
    public interface IAIMover
    {
        void SetTarget(Vector3 target);
        void Update(float elapsed);
    }
}
