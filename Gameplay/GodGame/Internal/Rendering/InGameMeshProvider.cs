using System;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Types.Towns;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.GodGame.Internal.Rendering
{
    /// <summary>
    /// The combined Mesh provider used in-game
    /// </summary>
    public class InGameMeshProvider : IMeshProvider
    {
        private IMeshProvider provider;

        public InGameMeshProvider(Func<IMeshProvider, TownBordersMeshProvider> createTownBordersMeshProvider, GameVoxelTypeMeshProvider gameVoxelTypeMeshProvider, Func<IMeshProvider, VisualizeChangesMeshProvider> createVisualizeChangesMeshProvider)
        {
            provider = createVisualizeChangesMeshProvider(createTownBordersMeshProvider(gameVoxelTypeMeshProvider));
        }

        public IMesh GetMesh(IVoxel gameVoxel)
        {
            return provider.GetMesh(gameVoxel);
        }
    }
}