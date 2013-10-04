using System;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.Worlding
{
    /// <summary>
    /// Implements the IWorld interface for use within SkyMerchant
    /// </summary>
    public class World : IWorld
    {
        private readonly ObjectsFactory factory;
        private readonly Func<Physical> createPhysical;

        public World(ObjectsFactory factory, Func<Physical> createPhysical)
        {
            this.factory = factory;
            this.createPhysical = createPhysical;
        }

        public IWorldObject CreateMeshObject(IMesh loadMesh)
        {
            var ph = createPhysical();
            ph.Mesh = loadMesh;

            return new WorldObject(ph);
        }

        public IWorldObject CreateIsland(int seed)
        {
            var i = factory.CreateIsland();
            i.Seed = seed;
            return (IWorldObject)i.Physical;
        }
    }
}