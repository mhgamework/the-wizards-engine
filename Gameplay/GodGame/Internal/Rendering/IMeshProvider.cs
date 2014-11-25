using Autofac;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.GodGame.Internal.Rendering
{
    /// <summary>
    /// Generates an up-to date mesh for given voxel
    /// </summary>
    public interface IMeshProvider
    {
        IMesh GetMesh(IVoxel gameVoxel);
    }
}