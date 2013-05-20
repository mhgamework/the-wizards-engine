using System.Collections.Generic;
using System.Linq;

namespace MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees
{
    /// <summary>
    /// Represents a selector that executes all of its child nodes, but fails if one fails.
    /// </summary>
    public class ConcurrentSelector : IBehaviourNode
    {
        private readonly IBehaviourNode[] children;

        public ConcurrentSelector(params IBehaviourNode[] children)
        {
            this.children = children;
        }

        public bool CanExecute(BehaviourTreeAgent agent)
        {
            return children.All(c => c.CanExecute(agent));
        }

        public NodeResult Execute(BehaviourTreeAgent agent)
        {
            var running = false;
            foreach (var c in children)
            {
                var result = c.Execute(agent);
                if (result == NodeResult.Failed)
                {
                    return NodeResult.Failed;
                }
                if (result == NodeResult.Running)
                {
                    running = true;
                }
            }
            
            return running ? NodeResult.Running : NodeResult.Success;
        }
    }
}