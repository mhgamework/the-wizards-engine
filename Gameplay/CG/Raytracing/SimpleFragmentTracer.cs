using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.CG
{
    public class SimpleFragmentTracer : IFragmentTracer
    {
        public FragmentInput TraceFragment(RayTrace rayTrace)
        {
            

            var ret = new FragmentInput();
            //ret.Position = result.CalculateHitPoint(ray);
            ret.Diffuse = new Color4(1, 0, 0);
            ret.Normal = new Vector3(0, 0, 1);

            return ret;
        }
    }
}
