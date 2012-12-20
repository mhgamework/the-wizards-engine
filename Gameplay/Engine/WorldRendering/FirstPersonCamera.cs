using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using SlimDX;

namespace MHGameWork.TheWizards.Engine.WorldRendering
{
    [ModelObjectChanged]
    public class FirstPersonCamera : EngineModelObject
    {
        public Vector3 Position { get; set; }
        public float JumpHeight { get; set; }
        public float JumpVelocity { get; set; }
        public Vector3 LookDir { get; set; }
    }
}
