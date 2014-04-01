using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.Core.Internal;
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
            //renderIslandSpaceManagerBoxes();
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

            foreach (var p in level.TextPanelNodes)
            {
                p.UpdateForRendering();
                p.TextRectangle.Update();

            }


            text.Text = level.LocalPlayer.Inventory.Items.GroupBy(i => i).Aggregate("Inventory: \n", (acc, el) => acc + el.Count() + " " + el.First().Name + "\n");

        }

        private void renderIslandSpaceManagerBoxes()
        {
            level.Islands.ForEach(i =>
                {
                    (i.SpaceManager.GetType().GetField("reservedSpots", BindingFlags.NonPublic | BindingFlags.Instance).GetValue
                         (i.SpaceManager) as List<BoundingBox>)
                        .ForEach(b =>
                            {
                                TW.Graphics.LineManager3D.WorldMatrix = i.Node.Absolute;
                                TW.Graphics.LineManager3D.AddBox(b, new Color4(1, 0, 0));
                            });
                });
        }
    }
}