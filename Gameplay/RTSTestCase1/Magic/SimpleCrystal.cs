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
    public class SimpleCrystal : EngineModelObject, ICrystal, IFieldElement
    {

        public float Capacity { get; set; }     
        public float Energy { get; set; }   
        public Vector3 Position{ get; set; }
        public SimpleCrystal()
        {
            Capacity = 1;
            Energy = 0;
            Active = true;
        }

        float ICrystal.GetCapacity()
        {
            return Capacity;    
        }
        
        float ICrystal.GetEnergy()
        {
            return Energy;
        }

        void ICrystal.SetEnergy(float newEnergy)
        {
            Energy = newEnergy;
        }

        public Vector3 GetPosition()
        {
            return Position;
        }

        public bool IsActive()
        {
            return Active;
        }

        public bool Active { get; set; }

        float IFieldElement.Density
        {   
            get { return Energy;}
        }
    }
}
