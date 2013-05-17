using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldResources
{
    [PersistanceScope]
    public class WorldResourceGenerationSimulator : ISimulator
    {
        public void Simulate()
        {
            var res = getResources();
            foreach (var r in res)
            {
                r.Grow(TW.Graphics.Elapsed);
            }
        }

        private IEnumerable<IWorldResource> getResources()
        {
            return TW.Data.Objects.Where(o => o is IWorldResource).Cast<IWorldResource>().ToArray();
        }
    }
}
