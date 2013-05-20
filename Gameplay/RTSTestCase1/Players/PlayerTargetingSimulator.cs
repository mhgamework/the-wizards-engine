using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;

namespace MHGameWork.TheWizards.RTSTestCase1.Players
{
    public class PlayerTargetingSimulator : ISimulator
    {
        public void Simulate()
        {
            foreach (var pl in TW.Data.Objects.Where(o => o is UserPlayer).Cast<UserPlayer>().ToArray())
            {
                var obj = TW.Data.Get<Engine.WorldRendering.World>().Raycast(pl.GetTargetingRay());
                pl.Targeted = obj.IsHit ? (Entity)obj.Object : null;
            }

        }
    }
}
