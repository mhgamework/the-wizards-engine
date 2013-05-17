using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldResources
{
    [PersistanceScope]
    public class WorldResourceGenerationWithPipesSimulator : ISimulator
    {
        public void Simulate()
        {
            var res = getResources();
            foreach (var r in res)
            {
                r.Grow(TW.Graphics.Elapsed);

                var pipe = getPipe(r);
                pipe.Update();
                if (!pipe.IsFree) return;
                if (!r.ResourceAvailable) continue;
                var t = r.TakeResource();
                pipe.SpawnItem(t);
                
            }
        }

        private DroppedThingOutputPipe getPipe(IWorldResource worldResource)
        {
            return worldResource.get<DroppedThingOutputPipe>() ?? createPipe(worldResource);
        }

        private static DroppedThingOutputPipe createPipe(IWorldResource worldResource)
        {
            var ret = new DroppedThingOutputPipe(worldResource.GenerationPoint, worldResource.OutputDirection);
            worldResource.set(ret);
            return ret;
        }

        private IEnumerable<IWorldResource> getResources()
        {
            return TW.Data.Objects.Where(o => o is IWorldResource).Cast<IWorldResource>().ToArray();
        }
    }
}
