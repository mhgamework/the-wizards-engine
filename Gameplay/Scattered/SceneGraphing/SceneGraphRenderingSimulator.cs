using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Scattered.Model;

namespace MHGameWork.TheWizards.Scattered.SceneGraphing
{
    public class SceneGraphRenderingSimulator : ISimulator
    {
        private readonly Func<IEnumerable<EntityNode>> getAllEntityNodes;

        public SceneGraphRenderingSimulator(Func<IEnumerable<EntityNode>> getAllEntityNodes)
        {
            this.getAllEntityNodes = getAllEntityNodes;
        }

        public void Simulate()
        {
            foreach (var n in getAllEntityNodes())
            {
                n.UpdateForRendering();
            }
        }
    }
}