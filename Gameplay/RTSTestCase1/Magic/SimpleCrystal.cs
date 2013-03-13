using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic
{

    [ModelObjectChanged]
    class SimpleCrystal : EngineModelObject, ICrystal
    {

        public float Capacity { get; set; }     
        public float Energy { get; set; }   
        public Vector3 Position{ get; set; }
        public float GetCapacity()
        {
            return Capacity;    
        }

        public float GetEnergy()
        {
            return Energy;
        }

        public void SetEnergy(float newEnergy)
        {
            Energy = newEnergy;
        }

        public Vector3 GetPosition()
        {
            return Position;
        }
    }
}
