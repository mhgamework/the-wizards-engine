using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.NavMeshing
{
    public class NavMesh
    {
        private readonly NavMeshNode rootNode;

        private List<NavMeshNode> visited = new List<NavMeshNode>();
        private Stack<NavMeshNode> stack = new Stack<NavMeshNode>();
        public IEnumerable<NavMeshNode> Nodes
        {
            get
            {
                visited.Clear();
                stack.Push(rootNode);
                while (stack.Count>0)
                {
                    var curr = stack.Pop();
                    yield return curr;
                    foreach (var node in curr.Edges.Select(e=>e.LeftNode).Concat(curr.Edges.Select(e=>e.RightNode)))
                    {
                        if (node == null)
                            continue;
                        if (visited.Contains(node))
                            continue;
                        visited.Add(node);
                        stack.Push(node);
                    }
                }
            }
        }

        public NavMesh(NavMeshNode rootNode)
        {
            this.rootNode = rootNode;
        }
    }
}
