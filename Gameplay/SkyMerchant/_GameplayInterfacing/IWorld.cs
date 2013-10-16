using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;

namespace MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing
{
    /// <summary>
    /// Access to manipulating the gameplay world
    /// </summary>
    public interface IWorld
    {
        IPositionComponent CreateMeshObject(IMesh loadMesh);
        IPositionComponent CreateIsland(int seed);
    }
}