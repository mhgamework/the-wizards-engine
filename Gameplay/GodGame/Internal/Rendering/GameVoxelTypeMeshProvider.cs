using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.GodGame.Internal.Rendering
{
    public class GameVoxelTypeMeshProvider : IMeshProvider
    {

        public GameVoxelTypeMeshProvider()
        {
        }

        public IMesh GetMesh(IVoxel gameVoxel)
        {

            if (gameVoxel.Data.Type == null) return null;

            var handle = gameVoxel;

            return gameVoxel.Data.Type.GetMesh((IVoxelHandle)handle);

        }
    }
}