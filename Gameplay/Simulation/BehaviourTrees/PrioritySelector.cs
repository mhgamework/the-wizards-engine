using System.Collections.Generic;

namespace MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees
{
    /// <summary>
    /// This is a selector. 
    /// It runs until it finds a node that succeeds, starting every time at the first node. (even when a node was running)
    /// 
    /// </summary>
    public class PrioritySelector : IBehaviourNode
    {
        private readonly IBehaviourNode[] children;

        public PrioritySelector(params IBehaviourNode[] children)
        {
            this.children = children;
        }

        public bool CanExecute(BehaviourTreeAgent agent)
        {
            return true;
        }

        public NodeResult Execute(BehaviourTreeAgent agent)
        {
            foreach (var child in children)
            {
                if (!child.CanExecute(agent)) continue;

                bool fail = false;

                var result = child.Execute(agent);
                if (result == NodeResult.Success || result == NodeResult.Running)
                {
                    return result;
                }

                // In case of failure, continue

            }

            return NodeResult.Failed;
        }
    }
}