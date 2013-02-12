using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldResources
{
    [ModelObjectChanged]
    public class Rock : EngineModelObject
    {
        public float Height { get; set; }
        public Vector3 Position { get; set; }
    }
}
