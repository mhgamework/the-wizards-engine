using System;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Physics;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// This does not belong to the rendering module anymore (due to the CollisionData in meshes)
    /// Should be move to other namespace
    /// </summary>
    public interface IMesh : IAsset
    {
        MeshCoreData GetCoreData();
        MeshCollisionData GetCollisionData();

    }
}
