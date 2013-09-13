using System.Collections.Generic;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine.WorldRendering;
using System.Linq;
using Castle.Core.Internal;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.SkyMerchant.DataStructures;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Lod
{
    /// <summary>
    /// Responsible for rendering physical objects using distance based LOD, integrated with the engine 
    /// </summary>
    public class PhysicalLodRenderer
    {
        private float renderPhysicalRange = 50;
        private float renderLineRange = 1000;
        private int wireframeNodeDepth = 3;
        private int physicalNodeDepth = 5;

        private IWorldOctree worldOctree;

        public PhysicalLodRenderer(IWorldOctree worldOctree)
        {
            this.worldOctree = worldOctree;
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
                .ForEach(p => ret.AddAABB(p.GetBoundingBox(), Matrix.Identity, new Color4(0, 0, 0)));

        }

        #endregion

        #region Physical layer

        private List<Physical> visiblePhysicals = new List<Physical>();

        private void updateVisiblePhysicals()
        {
            visiblePhysicals.ForEach(i => i.Visible = false);

            visiblePhysicals.Clear();
            visiblePhysicals.AddRange(worldOctree.GetChunksInRange(getCameraPosition(), 0, renderPhysicalRange, physicalNodeDepth).SelectMany(getPhysicals));

            visiblePhysicals.ForEach(i => i.Visible = true);

        }

        private static Vector3 getCameraPosition()
        {
            return TW.Graphics.Camera.ViewInverse.xna().Translation.dx();
        }

        private IEnumerable<Physical> getPhysicals(ChunkCoordinate arg)
        {
            return worldOctree.GetWorldObjects(arg);
        }

        #endregion



        public void RenderLines()
        {
            Vector3 cameraPosition = getCameraPosition();

            var all = worldOctree.GetChunksInRange(cameraPosition, renderPhysicalRange, renderLineRange,
                                                   wireframeNodeDepth);


            var partiallyWireframe = worldOctree.GetChunksInRange(cameraPosition, renderPhysicalRange, renderPhysicalRange, wireframeNodeDepth);

            var fullWireframe = all.Except(partiallyWireframe);

            fullWireframe.Select(getChunkLines).ForEach(l => TW.Graphics.LineManager3D.Render(l, TW.Graphics.Camera));
            partiallyWireframe.SelectMany(getPhysicals).Where(p => !p.Visible).ForEach(l => TW.Graphics.LineManager3D.AddBox(l.GetBoundingBox(), new Color4(0, 0, 0)));

        }
    }
}