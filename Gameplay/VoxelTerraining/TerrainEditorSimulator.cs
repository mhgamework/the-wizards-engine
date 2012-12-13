using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.CG.Spatial;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.VoxelTerraining
{
    public class TerrainEditorSimulator : ISimulator
    {
        private CameraInfo cameraInfo;

        private VoxelBlock targetedBlock;
        private VoxelBlock emptyTargetedBlock;

        public TerrainEditorSimulator()
        {
            cameraInfo = TW.Data.GetSingleton<CameraInfo>();
        }

        public void Simulate()
        {
            raycastBlock();

            if (targetedBlock != null)
            {
                var boundingBox = new BoundingBox();
                boundingBox.Minimum = targetedBlock.Position.ToVector3();
                boundingBox.Maximum = targetedBlock.Position.ToVector3() + MathHelper.One;
                boundingBox.Minimum = boundingBox.Minimum * targetedBlock.Terrain.NodeSize + targetedBlock.Terrain.WorldPosition;
                boundingBox.Maximum = boundingBox.Maximum * targetedBlock.Terrain.NodeSize + targetedBlock.Terrain.WorldPosition;
                TW.Graphics.LineManager3D.AddBox(boundingBox, new Color4());
            }

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.F))
                removeBlock();

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.E))
                placeBlock();
        }

        private void placeBlock()
        {
            if (emptyTargetedBlock == null) return;
            emptyTargetedBlock.Filled = true;
        }

        private void removeBlock()
        {
            if (targetedBlock == null) return;

            targetedBlock.Filled = false;
        }

        private void raycastBlock()
        {
            VoxelBlock last = null;
            VoxelBlock ret = null;
            var traverser = new GridTraverser();


            float? closest = null;


            foreach (VoxelTerrain terr in TW.Data.Objects.Where(o => o is VoxelTerrain))
            {
                var trace = new RayTrace();
                trace.Ray = cameraInfo.GetCenterScreenRay();

                float? dist = trace.Ray.xna().Intersects(terr.GetBoundingBox().xna());
                if (!dist.HasValue) continue;
                if (closest.HasValue && closest.Value < dist.Value)
                    continue;

                trace.Start = dist.Value + 0.001f;



                traverser.NodeSize = terr.NodeSize;
                traverser.GridOffset = terr.WorldPosition;

                //TODO: fix multiple terrains 


                var hit = false;


                VoxelTerrain terr1 = terr;
                traverser.Traverse(trace, delegate(Point3 arg)
                    {
                        if (!terr1.InGrid(arg)) return true;

                        var voxelBlock = terr1.GetVoxel(arg);
                        if (voxelBlock == null) return false;
                        if (voxelBlock.Filled)
                        {
                            hit = true;
                            ret = voxelBlock;
                            return true;
                        }
                        last = voxelBlock;
                        return false;
                    });


                if (hit)
                {
                    closest = dist;
                }


            }
            emptyTargetedBlock = last;
            targetedBlock = ret;
        }
    }
}
