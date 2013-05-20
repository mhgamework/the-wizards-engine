using System;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees
{
    /// <summary>
    /// Represents a node that executes its child behaviour while the while condition is true.
    /// The success condition is evaluated when the while loop exists, and returned.
    /// 
    /// The successcondition thing is something fishy, not sure i need this.
    /// </summary>
    public class While : IBehaviourNode
    {
        private readonly Func<bool> getWhileCondition;
        private readonly Func<bool> getSuccessCondition;
        private readonly IBehaviourNode child;

        public While(Func<bool> getWhileCondition, Func<bool> getSuccessCondition,IBehaviourNode child)
        {
            this.getWhileCondition = getWhileCondition;
            this.getSuccessCondition = getSuccessCondition;
            this.child = child;
        }

        public bool CanExecute(BehaviourTreeAgent agent)
        {
            return getWhileCondition() || (!getWhileCondition() && getSuccessCondition());
        }

        public NodeResult Execute(BehaviourTreeAgent agent)
        {
            if (!getWhileCondition())
                return getSuccessCondition() ? NodeResult.Success : NodeResult.Failed;
             
   
                
            var result = child.Execute(agent);
            if (result == NodeResult.Failed || result == NodeResult.Running) return result;

            return getSuccessCondition() ? NodeResult.Success : NodeResult.Failed;
        }
    }
}