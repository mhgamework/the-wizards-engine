using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.Entity
{
    /// <summary>
    /// This class is a RAM (no caching, network or database type implementation) of the IMesh
    /// </summary>
    public class RAMMesh : IMesh
    {
        private MeshCoreData coreData = new MeshCoreData();
        public Guid Guid { get; private set; }

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
