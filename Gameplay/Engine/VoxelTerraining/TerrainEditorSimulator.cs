using DirectX11;
using MHGameWork.TheWizards.Engine.WorldRendering;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.Engine.VoxelTerraining
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
                boundingBox.Minimum = targetedBlock.RelativePosition.ToVector3();
                boundingBox.Maximum = targetedBlock.RelativePosition.ToVector3() + MathHelper.One;
                boundingBox.Minimum = boundingBox.Minimum * targetedBlock.TerrainChunk.NodeSize + targetedBlock.TerrainChunk.WorldPosition;
                boundingBox.Maximum = boundingBox.Maximum * targetedBlock.TerrainChunk.NodeSize + targetedBlock.TerrainChunk.WorldPosition;
                TW.Graphics.LineManager3D.AddBox(boundingBox, new Color4());
            }

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.F))
                removeBlock();

            if (TW.Graphics.Keyboard.IsKeyPressed(Key.E))
                placeBlock();
        }

        private void raycastBlock()
        {
            targetedBlock= TW.Data.GetSingleton<VoxelTerrain>().Raycast(cameraInfo.GetCenterScreenRay(), out emptyTargetedBlock);
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


    }
}
