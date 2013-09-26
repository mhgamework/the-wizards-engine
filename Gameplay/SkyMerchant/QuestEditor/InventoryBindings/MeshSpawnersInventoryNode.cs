using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using MHGameWork.TheWizards.SkyMerchant._Engine;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryBindings
{
    /// <summary>
    /// Provides a node that dynamically provides spawners for meshes found in a folder on the filesystem.
    /// </summary>
    public class MeshSpawnersInventoryNode : IInventoryNode
    {
        private readonly DirectoryInfo folder;
        public ReadOnlyCollection<IInventoryNode> Children { get { return new ReadOnlyCollection<IInventoryNode>(GetChildrenFromDisk().ToList()); } }

        public MeshSpawnersInventoryNode(DirectoryInfo folder)
        {
            this.folder = folder;
        }


        public IEnumerable<IInventoryNode> GetChildrenFromDisk()
        {
            foreach (var childFolder in folder.GetDirectories())
            {
                yield return getFolderNode(childFolder);
            }
            foreach (var file in folder.GetFileSystemInfos().Where(isMesh))
            {
                yield return getMeshNode(file);
            }
        }

        [Cache]
        private IInventoryNode getMeshNode(FileSystemInfo file)
        {
            return new HotBarItemInventoryNode(new MeshSpawnerItem(file.FullName));
        }

        [Cache]
        private IInventoryNode getFolderNode(DirectoryInfo childFolder)
        {
            return new MeshSpawnersInventoryNode(childFolder);
        }

        private bool isMesh(FileSystemInfo arg)
        {
            return arg.Extension == ".obj";
        }
    }

    public class HotBarItemInventoryNode : IInventoryNode
    {
        public IHotbarItem Item { get; private set; }

        public HotBarItemInventoryNode(IHotbarItem item)
        {
            this.Item = item;
        }

        public ReadOnlyCollection<IInventoryNode> Children { get { return new ReadOnlyCollection<IInventoryNode>(new List<IInventoryNode>()); } }
    }
}