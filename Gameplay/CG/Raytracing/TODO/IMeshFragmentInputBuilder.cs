using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Raycasting;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    public interface IMeshFragmentInputBuilder
    {
        GeometryInput CalculateInput(IMesh mesh, Matrix world, MeshRaycastResult raycast);
    }
}
