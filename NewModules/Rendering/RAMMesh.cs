using System;
using MHGameWork.TheWizards.Physics;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// This class is a RAM (no caching, network or database type implementation) of the IMesh
    /// </summary>
    public class RAMMesh : IMesh
    {
        private MeshCoreData coreData = new MeshCoreData();
        public Guid Guid { get; set; }

        public RAMMesh()
        {
            Guid = Guid.NewGuid();
        }


        public MeshCoreData GetCoreData()
        {
            return coreData;
        }

        private MeshCollisionData collisionData = new MeshCollisionData();
        public MeshCollisionData GetCollisionData()
        {
            return collisionData;
        }
    }
}
