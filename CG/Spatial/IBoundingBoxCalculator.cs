using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Spatial
{
    /// <summary>
    /// Implements visitor pattern
    /// </summary>
    public interface IBoundingBoxCalculator
    {
        BoundingBox GetBoundingBox(Triangle surface);
    }
}
