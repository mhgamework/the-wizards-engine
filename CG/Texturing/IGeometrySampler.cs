using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;

namespace MHGameWork.TheWizards.CG.Texturing
{
    public abstract class IGeometrySampler<T>
    {
        public abstract T Sample(TraceResult input);

         public static implicit operator IGeometrySampler<T>(T val)
         {
             return new SolidGeometrySampler<T>(val);
         }
    }
}
