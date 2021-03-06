﻿using System.Collections.Generic;
using System.Drawing;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Networking;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.GodGame.Types.Towns;
using MHGameWork.TheWizards.GodGame._Tests;
using MHGameWork.TheWizards.Scattered.GameLogic.Services;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using MHGameWork.TheWizards.Scattered._Engine;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame
{
    /// <summary>
    /// Responsible for rendering the UI for a local player
    /// TODO: targetingreticle not working
    /// </summary>
    public class UIRenderingService : ISimulator
    {
        private readonly Internal.Model.World world;
        private readonly PlayerState localPlayerState;
        private readonly UserInputProcessingService inputSim;
        private TargetingReticle reticle;
        private Textarea textarea;

        private Textarea debugTextarea;

        public UIRenderingService(Internal.Model.World world, LocalPlayerService localPlayerService, UserInputProcessingService inputSim)
        {
            this.world = world;
            this.localPlayerState = localPlayerService.Player;
            this.inputSim = inputSim;

            reticle = new TargetingReticle();

            textarea = new Textarea();
            textarea.Position = new Vector2(TW.Graphics.Form.Form.ClientSize.Width - 120, 20);
            textarea.Size = new Vector2(100, 50);

            debugTextarea = new Textarea();
            debugTextarea.Position = new Vector2(TW.Graphics.Form.Form.ClientSize.Width - 200, 150);
            debugTextarea.Size = new Vector2(180, 400);

        }

        public void Simulate()
        {
            updateTextarea();
            updateSelectedVoxelVisualizers();
            drawReticle();
            drawSelectedVoxel();
            drawWorldBoundingbox();
            //drawDataValue();
            updateDebugTextarea();
            //drawMagicValue();
        }

        private void updateDebugTextarea()
        {
            debugTextarea.Text = "";

            var target = inputSim.GetTargetedVoxel();
            if (target == null)
            {
                return;
            }
            debugTextarea.Text = target.Type.GetDebugDescription(target);
        }


        private void updateTextarea()
        {
            textarea.Text = localPlayerState.ActiveTool == null ? "NO TOOL" : localPlayerState.ActiveTool.Name;
        }
        private void drawReticle()
        {
            reticle.drawReticle();
        }
        private void drawWorldBoundingbox()
        {
            TW.Graphics.LineManager3D.AddBox(new BoundingBox(new Vector3(0, 0, 0), new Vector3(world.VoxelSize.X * world.WorldSize, 1, world.VoxelSize.Y * world.WorldSize)), new Color4(0, 0, 0));
        }
        private void drawSelectedVoxel()
        {
            var v = inputSim.GetTargetedVoxel();
            if (v == null) return;
            var bb = v.GetBoundingBox();
            bb.Maximum.Y = 1f;

            TW.Graphics.LineManager3D.AddBox(bb, Color.Yellow.dx());
        }

        private TextRectangle dataValueRectangle = new TextRectangle();
        private void drawDataValue()
        {
            var target = inputSim.GetTargetedVoxel();
            /*if (target != null && target.Data.Type is IIndustryBuildingType)
            {
                showDataValueRect(target, ((IIndustryBuildingType)target.Data.Type).GetWorkerConsumer((IVoxelHandle)target).AllocatedWorkersCount); //note: not showing magicval atm!!
                return;
            }*/
            if (target == null || target.Data.DataValue == 0)
            {
                hideDataValueRect();
                return;
            }


            showDataValueRect(target, target.Data.DataValue); //note: not showing magicval atm!!

        }
        private void drawMagicValue()
        {
            var target = inputSim.GetTargetedVoxel();

            if (target == null)//|| target.MagicLevel == 0)
            {
                hideDataValueRect();
                return;
            }

            /*if (target.Data.Type is IIndustryBuildingType)
                showDataValueRect(target, ((IIndustryBuildingType)target.Data.Type).GetWorkerConsumer((IVoxelHandle)target).AllocatedWorkersCount); //note: not showing magicval atm!!
            else*/
            showDataValueRect(target, (int)target.Data.WorkerCount); //note: not showing magicval atm!!

        }

        private void hideDataValueRect()
        {
            dataValueRectangle.Position = new Vector3(0, 10000, 0);

        }
        private void showDataValueRect(GameVoxel target, int value)
        {
            var max = target.GetBoundingBox().Maximum;
            var min = target.GetBoundingBox().Minimum;
            var pos = (max + min) * 0.5f + new Vector3(0, 6.5f, 0);

            dataValueRectangle.Position = pos;
            dataValueRectangle.IsBillboard = true;
            dataValueRectangle.Text = value.ToString();// target.DataValue.ToString();
            dataValueRectangle.Radius = 3;
            dataValueRectangle.Update();
        }

        private List<IRenderable> visualizers = new List<IRenderable>();
        private GameVoxel visualizedVoxel = null;
        private IGameVoxelType visualizedType = null;
        private void updateSelectedVoxelVisualizers()
        {
            var voxel = inputSim.GetTargetedVoxel();
            if (voxel != visualizedVoxel || (voxel != null && voxel.Type != visualizedType))
            {
                foreach (var v in visualizers) v.Hide();
                visualizers.Clear();

                visualizedVoxel = null;
                visualizedType = null;
                if (voxel != null && voxel.Type != null)
                {
                    visualizedVoxel = voxel;
                    visualizedType = voxel.Type;
                    visualizers.AddRange(voxel.Type.GetInfoVisualizers(voxel));
                    foreach (var v in visualizers) v.Show();
                }

            }

            foreach (var v in visualizers) v.Update();
        }
    }
}