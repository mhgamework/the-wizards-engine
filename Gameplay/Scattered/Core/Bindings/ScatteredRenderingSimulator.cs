using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using SlimDX;
using System.Linq;
using DirectX11;

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

                var dist = Vector3.Distance(n.Node.Absolute.GetTranslation(),
                                            TW.Graphics.Camera.ViewInverse.GetTranslation());

                n.Entity.Visible = dist < 500;

                if (!n.Entity.Visible)
                {
                    var pos = n.Node.Absolute.GetTranslation();
                    TW.Graphics.LineManager3D.AddLine(pos, pos + Vector3.UnitY * 20, new Color4(0, 0, 1));

                }


            }
            foreach (var a in getAllAddons().ToArray()) // This toarray is a temp bugfix due to the fact that prepareforrendering can create addons :s
            {
                a.PrepareForRendering();
            }

            text.Text = level.LocalPlayer.Inventory.Items.Aggregate("Inventory: \n", (acc, el) => acc + el.Name + "\n");

        }
    }
}