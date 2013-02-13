using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldResources
{
    [ModelObjectChanged]
    public class Tree : EngineModelObject, IWorldResource
    {
        public float Size { get; set; }
        public Vector3 Position { get; set; }


        public Vector3 GenerationPoint { get { return Position + Vector3.UnitY * 0.3f; } }
        public Vector3 OutputDirection { get { return new Vector3(0, 0, 1); } }
        public Thing TakeResource()
        {
            if (!ResourceAvailable) throw new InvalidOperationException();
            Size -= 1;
            return new Thing() { Type = TW.Data.Get<ResourceFactory>().Wood };
        }

        public bool ResourceAvailable { get { return Size > 1; } }
        public void Grow(float elapsed)
        {
            Size += elapsed;
        }
    }
}
