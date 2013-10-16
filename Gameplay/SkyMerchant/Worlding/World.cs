using System;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;

namespace MHGameWork.TheWizards.SkyMerchant.Worlding
{
    /// <summary>
    /// Implements the IWorld interface for use within SkyMerchant
    /// </summary>
    public class World : IWorld
    {
        private readonly PrototypeObjectsFactory factory;
        private readonly Func<Physical> createPhysical;

        public World(PrototypeObjectsFactory factory, Func<Physical> createPhysical)
        {
            this.factory = factory;
            this.createPhysical = createPhysical;
        }

        public IPositionComponent CreateMeshObject(IMesh loadMesh)
        {
            throw new NotImplementedException();
            var ph = createPhysical();
            ph.Mesh = loadMesh;

            //return new WorldObject(ph);
        }

        public IPositionComponent CreateIsland(int seed)
        {
            throw new NotImplementedException();
            var i = factory.CreateIsland();
            i.Seed = seed;
            //return (IWorldObject)i.Physical;
        }
    }
}