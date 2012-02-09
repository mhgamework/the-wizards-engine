using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Deferred;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for storing a mesh and a BuildUnitType.
    /// </summary>
    public class BuildUnit
    {
        public IMesh Mesh;
        public String buildUnitType;

        public BuildUnit(IMesh mesh)
        {
            this.Mesh = mesh;
        }

        public BuildUnit(IMesh mesh, String buildUnitType)
        {
            this.Mesh = mesh;
            this.buildUnitType = buildUnitType;
        }
    }
}
