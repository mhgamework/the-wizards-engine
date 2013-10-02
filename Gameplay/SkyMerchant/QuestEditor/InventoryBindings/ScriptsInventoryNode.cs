using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using MHGameWork.TheWizards.SkyMerchant._Engine;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryBindings
{
    /// <summary>
    /// Provides a node that dynamically provides spawners for meshes found in a folder on the filesystem.
    /// </summary>
    public class ScriptsInventoryNode : IInventoryNode
    {
        private readonly string namespc;
        private readonly Func<Type, ScriptToolItem> createScriptToolItem;
        public ReadOnlyCollection<IInventoryNode> Children { get { return new ReadOnlyCollection<IInventoryNode>(GetChildren().ToList()); } }

        public ScriptsInventoryNode(string namespc, Func<Type, ScriptToolItem> createScriptToolItem)
        {
            this.namespc = namespc;
            this.createScriptToolItem = createScriptToolItem;
        }


        public IEnumerable<IInventoryNode> GetChildren()
        {
            foreach (var t in Assembly.GetExecutingAssembly()
                            .GetTypes()
                            .Where(t => t.Namespace == namespc)
                            .Where(t => typeof(IWorldScript).IsAssignableFrom(t)))
            {
                yield return getScriptNode(t);
            }
            foreach (var childNamespace in Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.Namespace != null)
                .Where(t => t.Namespace.StartsWith(namespc + "."))
                .Select(t => t.Namespace.Substring(namespc.Length + 1))
                .Where(n => !n.Contains('.'))
                .Distinct())
            {
                yield return getFolderNode(namespc + "." + childNamespace);
            }
        }

        [Cache]
        private IInventoryNode getScriptNode(Type t)
        {
            return new HotBarItemInventoryNode(createScriptToolItem(t));
        }

        [Cache]
        private IInventoryNode getFolderNode(string space)
        {
            return new ScriptsInventoryNode(space, createScriptToolItem);
        }
    }
}