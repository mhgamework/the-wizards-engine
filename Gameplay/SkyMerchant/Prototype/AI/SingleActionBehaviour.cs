using System;
using MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.AI
{
    public class SingleActionBehaviour : IBehaviourNode
    {
        private readonly Action action;

        public SingleActionBehaviour(Action action)
        {
            this.action = action;
        }

        public bool CanExecute(BehaviourTreeAgent agent)
        {
            return true;
        }

        public NodeResult Execute(BehaviourTreeAgent agent)
        {
            action();
            return NodeResult.Success;
        }
    }
}