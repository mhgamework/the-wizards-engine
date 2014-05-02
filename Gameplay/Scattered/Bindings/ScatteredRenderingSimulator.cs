﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.Core.Internal;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Rendering.Lod;
using MHGameWork.TheWizards.Scattered.GameLogic.Services;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using SlimDX;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.Scattered._Engine;

namespace MHGameWork.TheWizards.Scattered.Bindings
{
    /// <summary>
    /// Supports rendering for entity nodes and Updates all addons.
    /// 
    /// TOODOODOO this is a problem???
    /// </summary>
    public class ScatteredRenderingSimulator : ISimulator
    {
        private readonly Level level;
        private readonly Func<IEnumerable<IIslandAddon>> getAllAddons;
        private LinebasedLodRenderer lodRenderer;
        private readonly WorldRenderingSimulator renderingSimulator;
        private HudService hudService;

        private Textarea debugTextArea = new Textarea();

        public ScatteredRenderingSimulator(Level level, Func<IEnumerable<IIslandAddon>> getAllAddons, LinebasedLodRenderer lodRenderer,WorldRenderingSimulator renderingSimulator)
        {
            this.level = level;
            this.getAllAddons = getAllAddons;
            this.lodRenderer = lodRenderer;
            this.renderingSimulator = renderingSimulator;


            hudService = new HudService(level);


        }

        public void Simulate()
        {
            //renderIslandSpaceManagerBoxes();

            lodRenderer.UpdateRendererState();

            lodRenderer.RenderLines();

            foreach (var a in getAllAddons().ToArray()) // This toarray is a temp bugfix due to the fact that prepareforrendering can create addons :s
            {
                a.PrepareForRendering();
            }

            foreach (var p in level.TextPanelNodes)
            {
                p.UpdateForRendering();
                p.TextRectangle.Update();

            }

            hudService.Simulate();
            renderDebugHud();

            renderingSimulator.Simulate();

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
            var target = level.EntityNodes.Where(e => e.Mesh != null).Raycast(e => TW.Assets.GetBoundingBox(e.Mesh), e => e.Node.Absolute,
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
                    (i.SpaceAllocator.GetType().GetField("reservedSpots", BindingFlags.NonPublic | BindingFlags.Instance).GetValue
                         (i.SpaceAllocator) as List<BoundingBox>)
                        .ForEach(b =>
                            {
                                TW.Graphics.LineManager3D.WorldMatrix = i.Node.Absolute;
                                TW.Graphics.LineManager3D.AddBox(b, new Color4(1, 0, 0));
                            });
                });
        }



    }
}