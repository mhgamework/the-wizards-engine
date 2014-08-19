using System.Collections.Generic;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.Types;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Internal
{
    public class CustomVoxelsRenderer
    {
        private HashSet<GameVoxel> visibleVoxels = new HashSet<GameVoxel>();

        private Dictionary<GameVoxel, RenderData> visualizers = new Dictionary<GameVoxel, RenderData>();

        private World world;

        public IEnumerable<IRenderable> VisibleCustomRenderables { get { return visualizers.Values.SelectMany(v => v.Visualizers); } }

        public CustomVoxelsRenderer(World world)
        {
            this.world = world;
        }

        public void updateVoxelCustomRenderers(Point2 offset, Vector3 worldTranslation, Array2D<Entity> entities)
        {
            var currVisibleVoxels = new HashSet<GameVoxel>();
            entities.ForEach((e, p) =>
                {
                    var v = world.GetVoxel(p + new Point2(offset));
                    if (v != null)
                        currVisibleVoxels.Add(v);
                    /*e.WorldMatrix = Matrix.Scaling(new Vector3(world.VoxelSize.X)) *
                                Matrix.Translation(world.GetBoundingBox(p).GetCenter() + worldTranslation);
                e.Visible = e.Mesh != null;*/
                });


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
            var vizs = added.Type.GetCustomVisualizers(new IVoxelHandle(world, added)).ToArray();
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