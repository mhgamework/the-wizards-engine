using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.VoxelInfoVisualizers
{
    public class WorkerCountVisualizer : IRenderable
    {
        private readonly IVoxelHandle handle;
        private readonly int maxWorkersNeeded;
        private Entity workerVis;
        private const int maxHeight = 5;
        private Matrix translation;

        public WorkerCountVisualizer(IVoxelHandle handle, int maxWorkersNeeded)
        {
            this.handle = handle;
            this.maxWorkersNeeded = maxWorkersNeeded;
            translation = Matrix.Translation(new Vector3(0, 1, 0) + handle.GetInternalVoxel().GetBoundingBox().Minimum);
        }

        public void Show()
        {
            workerVis = new Entity { Mesh = createMeshWithColor(Color.White), WorldMatrix = translation };
        }

        public void Update()
        {
            if (workerVis == null)
                return;

            var efficiency = (float)handle.Data.WorkerCount / (float)maxWorkersNeeded;

            workerVis.WorldMatrix = Matrix.Scaling(1, efficiency, 1) * translation;

            Color color;
            if (efficiency < 0.5f)
                color = Color.Red;
            else if (efficiency < 1f)
                color = Color.Orange;
            else
                color = Color.Green;

            workerVis.Mesh = createMeshWithColor(color);
        }

        private IMesh createMeshWithColor(Color4 color)
        {
            return UtilityMeshes.CreateBoxColoredSize(color, new Vector3(0.5f, maxHeight, 0.5f));
        }

        public void Hide()
        {
            TW.Data.RemoveObject(workerVis);
        }
    }
}
