using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine.PhysX;
using MHGameWork.TheWizards.Engine.WorldRendering;

namespace MHGameWork.TheWizards.Engine.Tests.PhysX
{
    public class BarrelSimulatorTest 
    {
        public void Simulate()
        {
        }

        public void Initialize(TWEngine game)
        {
            game.AddSimulator(new BarrelShooterSimulator());
            game.AddSimulator(new PhysXSimulator());
            game.AddSimulator(new WorldRenderingSimulator());
            game.AddSimulator(new PhysXDebugRendererSimulator());
        }
    }
}
