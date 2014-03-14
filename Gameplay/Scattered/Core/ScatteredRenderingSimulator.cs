using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Scattered.SceneGraphing;

namespace MHGameWork.TheWizards.Scattered.Core
{
    /// <summary>
    /// Supports rendering for entity nodes and Updates all addons.
    /// 
    /// TOODOODOO this is a problem???
    /// </summary>
    public class ScatteredRenderingSimulator : ISimulator
    {
        private readonly Func<IEnumerable<EntityNode>> getAllEntityNodes;
        private readonly Func<IEnumerable<IIslandAddon>> getAllAddons;

        public ScatteredRenderingSimulator(Func<IEnumerable<EntityNode>> getAllEntityNodes, Func<IEnumerable<IIslandAddon>> getAllAddons )
        {
            this.getAllEntityNodes = getAllEntityNodes;
            this.getAllAddons = getAllAddons;
        }

        public void Simulate()
        {
            foreach (var n in getAllEntityNodes())
            {
                n.UpdateForRendering();
            }
            foreach (var a in getAllAddons())
            {
                a.PrepareForRendering();
            }
        }
    }
}