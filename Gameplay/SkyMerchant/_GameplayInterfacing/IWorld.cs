using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing
{
    /// <summary>
    /// Access to manipulating the gameplay world
    /// </summary>
    public interface IWorld
    {
        IWorldObject CreateMeshObject(IMesh loadMesh);
        IWorldObject CreateIsland(int seed);
    }
}