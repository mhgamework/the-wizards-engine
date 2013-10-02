using System.IO;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.Gameplay.Items
{
    /// <summary>
    /// Spawns a mesh set on constructions
    /// </summary>
    public class MeshSpawnerItem : IHotbarItem
    {
        private readonly IWorld world;
        private WorldPlacerHelper placer;
        public string MeshPath { get; private set; }

        /// <summary>
        /// TODO: replace meshPath with IMesh, when lazy mesh loading is supported.
        /// </summary>
        public MeshSpawnerItem(string meshPath, IWorld world)
        {
            this.world = world;
            this.MeshPath = meshPath;
            Name = "Mesh: " + Path.GetFileNameWithoutExtension(meshPath);

            placer = new WorldPlacerHelper(createNewMesh);

        }

        private IWorldObject createNewMesh()
        {
            var p = world.CreateMeshObject(TW.Assets.LoadMesh(MeshPath));
            return p;
        }

        public string Name { get; private set; }
        public void OnSelected()
        {
            
        }

        public void OnDeselected()
        {
        }

        public void Update()
        {
            placer.Update();
        }
    }
}