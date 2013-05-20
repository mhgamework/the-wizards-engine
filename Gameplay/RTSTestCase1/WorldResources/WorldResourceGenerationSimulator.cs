﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1._Engine;
using SlimDX;

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

                generateResource(r);

            }
        }

        private void generateResource(IWorldResource worldResource)
        {
            if (!worldResource.ResourceAvailable) return;
            if (getInSphere(worldResource.GenerationPoint, 0.01f).Any(o => o != worldResource)) return;

            var thing = worldResource.TakeResource();
            var drop = TW.Data.Get<IRTSFactory>().CreateDroppedThing(thing, worldResource.GenerationPoint);

        }

        private IEnumerable<IModelObject> getInSphere(Vector3 centre, float dist)
        {
            var sq = dist * dist;
            return
                TW.Data.Objects.OfType<IPhysical>()
                  .Where(o => Vector3.DistanceSquared(o.Physical.GetPosition(), centre) <= sq);

        }

        private IEnumerable<IWorldResource> getResources()
        {
            return TW.Data.Objects.Where(o => o is IWorldResource).Cast<IWorldResource>().ToArray();
        }
    }
}

