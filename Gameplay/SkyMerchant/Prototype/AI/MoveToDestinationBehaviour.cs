using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.AI
{
    public class MoveToDestinationBehaviour : IBehaviourNode
    {
        private readonly EnemyBrain brain;
        private readonly ISimulationEngine engine;
        private readonly IPositionComponent ph;

        /// <summary>
        /// Note: replace physical with a moveto delegate?(Action{Vector3})
        /// </summary>
        /// <param name="brain"></param>
        /// <param name="engine"></param>
        /// <param name="ph"></param>
        public MoveToDestinationBehaviour(EnemyBrain brain, ISimulationEngine engine, IPositionComponent ph)
        {
            this.brain = brain;
            this.engine = engine;
            this.ph = ph;
        }

        public bool CanExecute(BehaviourTreeAgent agent)
        {
            //TODO: add pathfinding here (check if obstructed)
            return true;
        }

        public NodeResult Execute(BehaviourTreeAgent agent)
        {
            if (Vector3.Distance(brain.Position, brain.Destination) < 0.01f) return NodeResult.Success;
            ph.Position = ph.Position + Vector3.Normalize(ph.Position - brain.Destination) * engine.Elapsed * 2;
            return NodeResult.Running;
        }
    }
}