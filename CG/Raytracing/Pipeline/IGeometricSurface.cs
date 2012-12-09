﻿using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Raytracing.Pipeline
{
    public interface IGeometricSurface
    {
        BoundingBox CalculateBoundingBox();
        void Intersects(ref RayTrace trace, ref TraceResult result);
    }
}
