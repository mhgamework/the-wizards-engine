using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Math;

namespace MHGameWork.TheWizards.CG.Raytracing
{
    public class SolidShadeCommand : IShadeCommand
    {
        private Color4 color;

        public SolidShadeCommand(Color4 color)
        {
            this.color = color;
        }

        public Color4 CalculateColor()
        {
            return color;
        }
    }
}
