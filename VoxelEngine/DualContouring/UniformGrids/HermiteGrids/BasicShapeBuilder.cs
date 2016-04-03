
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    /// <summary>
    /// Constructs basic shapes
    /// </summary>
    public class BasicShapeBuilder
    {
        /// <summary>
        /// Returns a hermite grid of size+1 with a cube of 'size' voxels centered in the middle
        /// </summary>
        public HermiteDataGrid CreateCube(int size)
        {
            return HermiteDataGrid.FromIntersectableGeometry(size + 1, size + 1, Matrix.Scaling(new Vector3(0.5f * size)) * Matrix.Translation(new Vector3(0.5f + size / 2f)),
                                                            new IntersectableCube());
        }
        /// <summary>
        /// Returns a hermite grid of size+1 with a sphere of 'size' voxels centered in the middle
        /// </summary>
        public HermiteDataGrid CreateSphere(int size)
        {
            return HermiteDataGrid.FromIntersectableGeometry(size + 1, size + 1, Matrix.Scaling(new Vector3(0.5f * size)) * Matrix.Translation(new Vector3(0.5f + size / 2f)),
                                                            new IntersectableSphere());
        }
    }
}