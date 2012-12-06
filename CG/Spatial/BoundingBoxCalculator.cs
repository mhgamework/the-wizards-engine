using System;
using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Spatial
{
    /// <summary>
    /// Reponsible for calculating boundingboxes for ISurface objects
    /// </summary>
    public class BoundingBoxCalculator : IBoundingBoxCalculator
    {
        public BoundingBox GetBoundingBox(Triangle surface)
        {
            return BoundingBox.FromPoints(surface.getPositions());
        }
    }
}