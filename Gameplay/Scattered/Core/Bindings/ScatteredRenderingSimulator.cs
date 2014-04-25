using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.Core.Internal;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Rendering.Text;
using MHGameWork.TheWizards.Scattered.Core.Bindings;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using SlimDX;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.Scattered._Engine;

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
        private HudSimulator hudSimulator;

        private Textarea debugTextArea = new Textarea();

        public ScatteredRenderingSimulator(Level level, Func<IEnumerable<EntityNode>> getAllEntityNodes, Func<IEnumerable<IIslandAddon>> getAllAddons)
        {
            this.level = level;
            this.getAllEntityNodes = getAllEntityNodes;
            this.getAllAddons = getAllAddons;


            hudSimulator = new HudSimulator(level);


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

            hudSimulator.Simulate();
            renderDebugHud();

        }

        private void renderDebugHud()
        {
            setupDebugTextArea();

            //debugTextArea.Text = "The text!";
            updateDebugAddonText();
        }

        private void setupDebugTextArea()
        {
            debugTextArea.Position = new Vector2(300, 10);
            debugTextArea.Size = new Vector2(200, 100);
            debugTextArea.Visible = true;
        }

        private void updateDebugAddonText()
        {
            var target = level.EntityNodes.Where(e => e.Entity.Mesh != null).Raycast(e => TW.Assets.GetBoundingBox(e.Entity.Mesh), e => e.Node.Absolute,
                                                   level.LocalPlayer.GetTargetingRay());

            debugTextArea.Text = "No object";

            if (!target.IsHit) return;

            
            var node = ((EntityNode)target.Object).Node;
            while (node.AssociatedObject == null || (!(node.AssociatedObject is IIslandAddon)))
            {
                node = node.Parent;
                if (node == null) break;
            }
            if (node == null) return;
            // Node should now be the addon node

            debugTextArea.Text = node.AssociatedObject.GetType().Name + ":\n";


            if (node.AssociatedObject is IDebugAddon)
            {
                var addon = (IDebugAddon)node.AssociatedObject;
                debugTextArea.Text += addon.GetDebugText();
            }

           
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