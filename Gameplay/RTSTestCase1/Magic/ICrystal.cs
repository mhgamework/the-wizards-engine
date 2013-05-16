using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic
{
    public interface ICrystal
    {
        float GetCapacity(); // Energy level of crystal from 0 tot 1
        float GetEnergy();
        void SetEnergy(float level);
        Vector3 GetPosition();
        bool IsActive();
    }    
}
