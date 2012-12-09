using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Pipeline;

namespace MHGameWork.TheWizards.CG.Texturing
{
    public class SolidGeometrySampler<T> : IGeometrySampler<T>
    {
        private T color;

        public SolidGeometrySampler(T color)
        {
            this.color = color;
        }


        public override T Sample(GeometryInput input)
        {
            return color;
        }

      
    }
}
