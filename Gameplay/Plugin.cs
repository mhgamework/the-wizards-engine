using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.ModelContainer;

namespace MHGameWork.TheWizards
{
    public class Plugin : IGameplayPlugin
    {
        public void Initialize(Engine.TWEngine engine)
        {
            engine.AddSimulator(new Simulators.AssetbrowserSimulator());
            engine.AddSimulator(new Simulators.WorldRenderingSimulator());
        }
    }
}
