using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Animation
{
    public interface IAnimatable
    {
        void Set(Keyframe prev, Keyframe next, float percent);
    }
}
