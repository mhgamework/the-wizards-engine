using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered.Core
{
    /// <summary>
    /// Supports rendering for entity nodes and Updates all addons.
    /// 
    /// TOODOODOO this is a problem???
    /// </summary>
    public class ScatteredRenderingSimulator : ISimulator
    {
        private readonly Level level;
        private readonly Func<IEnumerable<EntityNode>> getAllEntityNodes;
        private readonly Func<IEnumerable<IIslandAddon>> getAllAddons;
        private Textarea text;

        public ScatteredRenderingSimulator(Level level, Func<IEnumerable<EntityNode>> getAllEntityNodes, Func<IEnumerable<IIslandAddon>> getAllAddons)
        {
            this.level = level;
            this.getAllEntityNodes = getAllEntityNodes;
            this.getAllAddons = getAllAddons;

            text = new Textarea();
            text.Position = new Vector2(650, 30);
            text.Size = new Vector2(140, 200);

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

            text.Text = level.LocalPlayer.Inventory.Items.Aggregate("Inventory: \n", (acc, el) => acc + el.Name + "\n");

        }
    }
}