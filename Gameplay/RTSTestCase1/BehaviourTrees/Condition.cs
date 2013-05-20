using System;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees
{
    /// <summary>
    /// Represents a node that fails when given condition fails, succeeds when the condition succeeds.
    /// </summary>
    public class Condition : IBehaviourNode
    {
        private readonly Func<BehaviourTreeAgent,bool> func;

        public Condition(Func<BehaviourTreeAgent, bool> func)
        {
            this.func = func;
        }

        public bool CanExecute(BehaviourTreeAgent agent)
        {
            return func(agent);
        }

        public NodeResult Execute(BehaviourTreeAgent agent)
        {
            return NodeResult.Success;
        }
    }
}