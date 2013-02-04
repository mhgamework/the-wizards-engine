using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;
using MHGameWork.TheWizards.Profiling;

namespace MHGameWork.TheWizards.Diagnostics.Profiling
{
    public class ProfilerComponent : IDiagnosticsComponent
    {
        private ProfilerDisplay display;

        private Dictionary<ProfilingPoint, ProfilingNode> nodes = new Dictionary<ProfilingPoint, ProfilingNode>();

        public ProfilerComponent()
        {
            display = new ProfilerDisplay();
        }

        /// <summary>
        /// Runs in a different thread!
        /// </summary>
        /// <param name="rootPoint"></param>
        public void Update(ProfilingPoint rootPoint)
        {
            if (Thread.CurrentThread != display.Dispatcher.Thread)
            {
                display.Dispatcher.Invoke(new Action(() => Update(rootPoint)));
                return;
            }


            resetAll();
            updateRecursive(rootPoint);
            if (display.ViewModel.BaseLevel[0] != nodes[rootPoint])
                display.ViewModel.SetRoot(nodes[rootPoint]);


        }

        private void resetAll()
        {
            foreach (var node in nodes.Values)
                node.Duration = 0;
        }

        private void updateRecursive(ProfilingPoint p)
        {
            var node = getOrCreateNode(p);
            node.Children.Clear();
            foreach (var child in p.NonRecursiveChildren)
            {
                var cNode = getOrCreateNode(child);
                cNode.Duration += p.TotalTime;

                if (!node.Children.Contains(cNode))
                    node.Children.Add(cNode);
                updateRecursive(child);
            }
        }

        private ProfilingNode getOrCreateNode(ProfilingPoint rootPoint)
        {
            if (nodes.ContainsKey(rootPoint)) return nodes[rootPoint];
            var node = new ProfilingNode();
            node.Name = rootPoint.Name;
            nodes.Add(rootPoint, node);
            return node;
        }

        public Control GetView()
        {
            return display;
        }
    }
}
