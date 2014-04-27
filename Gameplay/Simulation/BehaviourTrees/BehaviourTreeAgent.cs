using System.Collections.Generic;

namespace MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees
{
    /// <summary>
    /// Simulates behaviour for a given behaviour tree. Stores running information
    /// </summary>
    public class BehaviourTreeAgent
    {
        private readonly IBehaviourNode root;
        private IEnumerator<NodeResult> enumerator;

        private Dictionary<IBehaviourNode, IBehaviourNode> nodeStates = new Dictionary<IBehaviourNode, IBehaviourNode>();

        public BehaviourTreeAgent(IBehaviourNode root)
        {
            this.root = root;
        }

        public void Simulate()
        {

            if (!root.CanExecute(this)) return;
            root.Execute(this);



        }

        public void StoreState(IBehaviourNode sequence, IBehaviourNode state)
        {
            nodeStates[sequence] = state;
        }

        public IBehaviourNode GetState(IBehaviourNode sequence)
        {
            if (!nodeStates.ContainsKey(sequence)) return null;
            return nodeStates[sequence];
        }

        public T Get<T>()
        {
            throw new System.NotImplementedException();
        }
    }
}