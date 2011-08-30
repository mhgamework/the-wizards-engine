﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;


namespace MHGameWork.TheWizards.Particles
{
    public interface IParticleCreater
    {
        
        void GetNewParticleData(out Vector3 position, out Vector3 velocity);

    }                                                                        
}
