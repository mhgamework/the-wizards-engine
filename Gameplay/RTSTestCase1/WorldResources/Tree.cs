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
    public class Tree : EngineModelObject, IWorldResource
    {
        public float Size { get; set; }
        public Vector3 Position { get; set; }


        public Vector3 GenerationPoint { get; private set; }
        public Vector3 OutputPoint { get; private set; }
        public void TakeResource()
        {
            
        }

        public bool ResourceAvailable { get; private set; }
    }
}
