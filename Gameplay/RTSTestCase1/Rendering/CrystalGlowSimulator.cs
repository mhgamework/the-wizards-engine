using System;
using System.Linq;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Magic;

namespace MHGameWork.TheWizards.RTSTestCase1.Rendering
{
    public class CrystalGlowSimulator : ISimulator
    {
        Random rand = new Random();
        public void Simulate()
        {
            var crystals = TW.Data.Objects.Where(o => o is SimpleCrystal).Cast<SimpleCrystal>();
            foreach (var crystal in crystals)
            {
                var renderData = crystal.get<CrystalRenderData>();
                var load = crystal.Energy / crystal.Capacity;
                if(rand.NextDouble()>0.5f)
                    renderData.glowPart += 4*(1 + 5*load ) * TW.Graphics.Elapsed;
                if (renderData.glowPart > 2 * Math.PI)
                    renderData.glowPart = renderData.glowPart - (2 * (float)Math.PI);
                //renderData.PointLight.Intensity =0.3f * (1 + (float)Math.Sin(renderData.glowPart));
            }
        }
    }
}