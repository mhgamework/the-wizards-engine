using System.Collections.Generic;
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

        public void Update(ProfilingPoint rootPoint)
        {
            var node = getOrCreateNode(rootPoint);
            node.Children.Clear();
            foreach (var child in rootPoint.LastChildren)
            {
                var cNode = getOrCreateNode(child);
                node.Children.Add(cNode);
            }
        }

        private ProfilingNode getOrCreateNode(ProfilingPoint rootPoint)
        {
            if (nodes.ContainsKey(rootPoint)) return nodes[rootPoint];
            var node = new ProfilingNode();
            node.Name = rootPoint.Name;
            nodes.Add(rootPoint,node);
            return node;
        }

        public Control GetView()
        {
            return display;
        }
    }
}
