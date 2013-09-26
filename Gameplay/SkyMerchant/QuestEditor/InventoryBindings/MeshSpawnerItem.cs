using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryBindings
{
    /// <summary>
    /// Spawns a mesh set on constructions
    /// </summary>
    public class MeshSpawnerItem : IHotbarItem
    {
        public string MeshPath { get; private set; }

        /// <summary>
        /// TODO: replace meshPath with IMesh, when lazy mesh loading is supported.
        /// </summary>
        /// <param name="meshPath"></param>
        public MeshSpawnerItem(string meshPath)
        {
            this.MeshPath = meshPath;
            Name = "Mesh Spawner";
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
        }
    }
}