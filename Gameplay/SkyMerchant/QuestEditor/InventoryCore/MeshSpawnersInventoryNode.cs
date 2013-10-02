﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using MHGameWork.TheWizards.SkyMerchant.Gameplay;
using MHGameWork.TheWizards.SkyMerchant._Engine;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryCore
{
    /// <summary>
    /// Provides a node that dynamically provides spawners for meshes found in a folder on the filesystem.
    /// </summary>
    public class MeshSpawnersInventoryNode : IInventoryNode
    {
        private readonly DirectoryInfo folder;
        private readonly IMeshSpawnerItemFactory factory;
        public ReadOnlyCollection<IInventoryNode> Children { get { return new ReadOnlyCollection<IInventoryNode>(GetChildrenFromDisk().ToList()); } }

        public MeshSpawnersInventoryNode(DirectoryInfo folder, IMeshSpawnerItemFactory factory)
        {
            this.folder = folder;
            this.factory = factory;
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
            var path = file.FullName;
            path = path.Substring(0, path.Length - file.Extension.Length);

            path = path.Replace(TWDir.GameData + "\\", "");

            return new HotBarItemInventoryNode(factory.CreateItem(path));
        }

        [Cache]
        private IInventoryNode getFolderNode(DirectoryInfo childFolder)
        {
            return new MeshSpawnersInventoryNode(childFolder, factory);
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