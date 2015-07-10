using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using MHGameWork.TheWizards.Simulation.Spatial;
using MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering.Lod
{
    /// <summary>
    /// Responsible for rendering physical objects using distance based LOD, integrated with the engine 
    /// </summary>
    public class LinebasedLodRenderer
    {
        public float renderPhysicalRange = 50;
        public float renderLineRange = 1000;
        public int wireframeNodeDepth = 3;
        public int physicalNodeDepth = 5;

        private IWorldOctree<IRenderable> worldOctree;

        public LinebasedLodRenderer(IWorldOctree<IRenderable> worldOctree)
        {
            this.worldOctree = worldOctree;
        }

        public List<IRenderable> VisibleRenderables
        {
            get { return visibleRenderables; }
        }

        /// <summary>
        /// Responds and performs all rendering state changes.
        /// </summary>
        public void UpdateRendererState()
        {
            updateVisiblePhysicals();
        }

        #region Line layer
        private Dictionary<ChunkCoordinate, LineManager3DLines> wireframes = new Dictionary<ChunkCoordinate, LineManager3DLines>();

        private LineManager3DLines getChunkLines(ChunkCoordinate chunkCoordinate)
        {
            return wireframes.GetOrCreate(chunkCoordinate, delegate
                {
                    var ret = new LineManager3DLines(TW.Graphics.Device);
                    updateWireframeBox(ret, chunkCoordinate);
                    return ret;
                });
        }

        private void updateWireframeBox(LineManager3DLines ret, ChunkCoordinate chunk)
        {
            ret.ClearAllLines();
            worldOctree.GetWorldObjects(chunk)
                .ForEach(p => ret.AddAABB(p.BoundingBox, Matrix.Identity, new Color4(0, 0, 0)));

        }

        #endregion

        #region Physical layer

        private List<IRenderable> visibleRenderables = new List<IRenderable>();

        private void updateVisiblePhysicals()
        {
            VisibleRenderables.ForEach(i => i.Visible = false);

            VisibleRenderables.Clear();
            VisibleRenderables.AddRange(worldOctree.GetChunksInRange(getCameraPosition(), 0, renderPhysicalRange, physicalNodeDepth).SelectMany(getPhysicals));

            VisibleRenderables.ForEach(i => i.Visible = true);

        }

        private static Vector3 getCameraPosition()
        {
            return TW.Graphics.Camera.ViewInverse.xna().Translation.dx();
        }

        private IEnumerable<IRenderable> getPhysicals(ChunkCoordinate arg)
        {
            return worldOctree.GetWorldObjects(arg);
        }

        #endregion



        public void RenderLines()
        {
            return;
            Vector3 cameraPosition = getCameraPosition();

            var all = worldOctree.GetChunksInRange(cameraPosition, renderPhysicalRange, renderLineRange,
                                                   wireframeNodeDepth);


            var partiallyWireframe = worldOctree.GetChunksInRange(cameraPosition, renderPhysicalRange, renderPhysicalRange, wireframeNodeDepth);

            var fullWireframe = all.Except(partiallyWireframe);

            fullWireframe.Select(getChunkLines).ForEach(l => TW.Graphics.LineManager3D.Render(l, TW.Graphics.Camera));
            partiallyWireframe.SelectMany(getPhysicals).Where(p => !p.Visible).ForEach(l => TW.Graphics.LineManager3D.AddBox(l.BoundingBox, new Color4(0, 0, 0)));

        }
    }
}