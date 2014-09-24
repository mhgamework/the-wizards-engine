using System.Collections.Generic;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Internal.Rendering
{
    public class CustomVoxelsRenderer
    {
        private HashSet<GameVoxel> visibleVoxels = new HashSet<GameVoxel>();

        private Dictionary<GameVoxel, RenderData> visualizers = new Dictionary<GameVoxel, RenderData>();

        private Model.World world;

        public IEnumerable<IRenderable> VisibleCustomRenderables { get { return visualizers.Values.SelectMany(v => v.Visualizers); } }

        public CustomVoxelsRenderer(Model.World world)
        {
            this.world = world;
        }

        public void updateVoxelCustomRenderers(Point2 offset, Vector3 worldTranslation, Point2 visibleWindowSize)
        {
            var currVisibleVoxels = new HashSet<GameVoxel>();
            for (int x = 0; x < visibleWindowSize.X; x++)
                for (int y = 0; y < visibleWindowSize.Y; y++)
                {
                    var p = new Point2(x, y);
                    var v = world.GetVoxel(p + new Point2(offset));
                    if (v != null)
                        currVisibleVoxels.Add(v);
                }

            var typeChangers = world.ChangedVoxels.Where(v => v.TypeChanged && currVisibleVoxels.Contains(v));


            // Remove old visualizers
            foreach (var removed in visibleVoxels.Except(currVisibleVoxels).ToArray())
                destroyRenderdata(removed);

            // Show new visualizers
            foreach (var added in currVisibleVoxels.Except(visibleVoxels).ToArray())
                createRenderData(added);

            // Rebuild for changed types
            foreach (var removed in typeChangers)
            {
                destroyRenderdata(removed);
                createRenderData(removed);
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
            public GameVoxelType Type;

            public RenderData(IRenderable[] visualizers, GameVoxelType type)
            {
                Visualizers = visualizers;
                Type = type;
            }
        }
    }
}