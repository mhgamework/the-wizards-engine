using System;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.SkyMerchant.GameObjects
{
    /// <summary>
    /// Calls a method once on each provided graph node of a directed graph, so that parents are allways called before their children.
    /// This is an implicit topological sort
    /// </summary>
    public class TopologicalUpdater
    {
        /// <summary>
        /// Assume no circular components!! 
        /// </summary>
        public void UpdateInTopologicalOrder<T>(IEnumerable<T> components, Func<T, T> getParent, Action<T> update) where T : class
        {
            var addedNodes = new HashSet<T>();
            foreach (var n in components) updateNode<T>(n, addedNodes, getParent, update);
        }

        /// <summary>
        /// 
        /// </summary>
        private void updateNode<T>(T node, HashSet<T> updatedNodes, Func<T, T> getParent, Action<T> update) where T : class
        {
            if (updatedNodes.Contains(node)) return;
            var parent = getParent(node);
            if (parent != null) updateNode(parent, updatedNodes, getParent, update); // allways update parent first
            update(node);
            updatedNodes.Add(node);
        }
    }
}