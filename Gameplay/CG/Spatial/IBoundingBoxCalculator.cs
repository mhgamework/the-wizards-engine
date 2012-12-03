using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

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
