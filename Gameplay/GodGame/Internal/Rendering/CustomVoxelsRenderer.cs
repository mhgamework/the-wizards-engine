using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using DirectX11;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Internal.Rendering
{
    public class CustomVoxelsRenderer
    {
        private readonly Model.World world;
        private HashSet<GameVoxel> visibleVoxels = new HashSet<GameVoxel>();

        private Dictionary<GameVoxel, RenderData> visualizers = new Dictionary<GameVoxel, RenderData>();


        public IEnumerable<IRenderable> VisibleCustomRenderables { get { return visualizers.Values.SelectMany(v => v.Visualizers); } }


        private HashSet<IVoxel> typeChangedVoxels = new HashSet<IVoxel>();

        public CustomVoxelsRenderer(Model.World world)
        {
            this.world = world;
            world.Created.Subscribe(v => typeChangedVoxels.Add(v));
        }

        public void updateVoxelCustomRenderers(Point2 offset, Vector3 worldTranslation, Point2 visibleWindowSize)
        {
            var currVisibleVoxels = new HashSet<GameVoxel>();
            for (int x = 0; x < visibleWindowSize.X; x++)
                for (int y = 0; y < visibleWindowSize.Y; y++)
                {
                    var p = new Point2(x, y);
                    var v =  world.GetVoxel(p + new Point2(offset));
                    if (v != null)
                        currVisibleVoxels.Add(v);
                }

            var typeChangers = typeChangedVoxels.Where(currVisibleVoxels.Contains).ToArray();
            typeChangedVoxels.Clear();


            // Remove old visualizers
            foreach (var removed in visibleVoxels.Except(currVisibleVoxels).ToArray())
                destroyRenderdata(removed);

            // Show new visualizers
            foreach (var added in currVisibleVoxels.Except(visibleVoxels).ToArray())
                createRenderData(added);

            // Rebuild for changed types
            foreach (var removed in typeChangers)
            {
                destroyRenderdata((GameVoxel)removed);
                createRenderData((GameVoxel)removed);
            }

            // Update visualizers
            foreach (var val in visualizers.Values.SelectMany(s => s.Visualizers))
                val.Update();

            visibleVoxels = currVisibleVoxels;

        }

        private void createRenderData(GameVoxel added)
        {
            var vizs = added.Type.GetCustomVisualizers(added).ToArray();
            if (!vizs.Any()) return;
            foreach (var v in vizs) v.Show();

            visualizers[added] = new RenderData(vizs, added.Type);
        }

        private void destroyRenderdata(GameVoxel removed)
        {
            if (!visualizers.ContainsKey(removed)) return;
            foreach (var viz in visualizers[removed].Visualizers)
                viz.Hide();

            visualizers.Remove(removed);
        }

        private class RenderData
        {
            public IRenderable[] Visualizers;
            public IGameVoxelType Type;

            public RenderData(IRenderable[] visualizers, IGameVoxelType type)
            {
                Visualizers = visualizers;
                Type = type;
            }
        }
    }
}