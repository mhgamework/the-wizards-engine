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
                TW.Graphics.LineManager3D.AddBox(new BoundingBox(targetedBlock.Position.ToVector3(), targetedBlock.Position.ToVector3() + MathHelper.One), new Color4());

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




            foreach (VoxelTerrain terr in TW.Data.Objects.Where(o => o is VoxelTerrain))
            {
                traverser.NodeSize = 1;
                traverser.GridOffset = new Vector3();

                //TODO: fix multiple terrains 



                var trace = new RayTrace();
                trace.Ray = cameraInfo.GetCenterScreenRay();
                VoxelTerrain terr1 = terr;
                traverser.Traverse(trace, delegate(Point3 arg)
                    {
                        if (!terr1.InGrid(arg)) return true;

                        var voxelBlock = terr1.GetVoxel(arg);
                        if (voxelBlock == null) return false;
                        if (voxelBlock.Filled)
                        {

                            ret = voxelBlock;
                            return true;
                        }
                        last = voxelBlock;
                        return false;
                    });

                break;

            }
            emptyTargetedBlock = last;
            targetedBlock = ret;
        }
    }
}
