using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.WorldRendering
{
    [ModelObjectChanged]
    public class PointLight : EngineModelObject
    {
        public float Intensity { get; set; }

        public PointLight()
        {
            Enabled = true;
            Size = 10;
            Intensity = 1;
        }

        public Vector3 Position { get; set; }
        public float Size { get; set; }
        public bool Enabled { get; set; }

    }
}
