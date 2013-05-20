using System.Collections.Generic;

namespace MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees
{
    public interface IBehaviourNode
    {
        bool CanExecute(BehaviourTreeAgent agent);
        NodeResult Execute(BehaviourTreeAgent agent);
    }

    public enum NodeResult
    {
        Success,
        Running,
        Failed,
        Error
    }
}