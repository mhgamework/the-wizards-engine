using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.GodGame.Types.Towns
{
    /// <summary>
    /// Adds town border displaying to 
    /// </summary>
    public class TownBordersMeshProvider : IMeshProvider
    {
        private readonly IMeshProvider provider;
        public TownBordersMeshProvider(IMeshProvider provider)
        {
            this.provider = provider;
        }

        public IMesh GetMesh(IVoxel gameVoxel)
        {
            return provider.GetMesh(gameVoxel);
        }
    }
}