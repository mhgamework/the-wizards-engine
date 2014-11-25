using System.Drawing;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.Types
{
    /// <summary>
    /// Utility functions for building meshes for voxels
    /// </summary>
    public class VoxelMeshBuilder
    {
        public static IMesh createColoredMesh(Color color1)
        {
            return MeshBuilder.Transform(UtilityMeshes.CreateBoxColored(color1, new Vector3(0.5f, 0.05f, 0.5f)),
                                  Matrix.Translation(0, -0.05f, 0));
        }
    }
}