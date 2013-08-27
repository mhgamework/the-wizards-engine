using System.Collections.Generic;

namespace MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees
{
    /// <summary>
    /// http://aigamedev.com/open/article/sequence/
    /// 
    /// Executes all child nodes in sequence until one fails
    /// Succeeds when all children succeeded, fails when one fails
    /// 
    /// Resumes at the last running node    
    /// 
    /// </summary>
    public class Sequence : IBehaviourNode
    {
        private readonly IBehaviourNode[] children;

        public Sequence(params IBehaviourNode[] children)
        {
            this.children = children;
        }

        public bool CanExecute(BehaviourTreeAgent agent)
        {
            return true;
        }

        public NodeResult Execute(BehaviourTreeAgent agent)
        {
            var state = agent.GetState(this);

            foreach (var c in children)
            {
                if (state != null && c != state) continue;
                state = null;
                if (!c.CanExecute(agent))
                {
                    agent.StoreState(this, null);
                    return NodeResult.Failed;
                }

                var res = c.Execute(agent);
                if (res == NodeResult.Failed)
                {
                    agent.StoreState(this,null);
                    return NodeResult.Failed;
                }
                if (res == NodeResult.Running)
                {
                    agent.StoreState(this,c);
                    return NodeResult.Running;

                }
            }
            return NodeResult.Success;

        }
    }
}