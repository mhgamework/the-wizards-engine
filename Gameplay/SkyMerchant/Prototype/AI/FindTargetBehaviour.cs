using System.Linq;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.AI
{
    public class FindTargetBehaviour : IBehaviourNode
    {
        private readonly EnemyBrain brain;
        private readonly IWorldLocator locator;

        public FindTargetBehaviour(EnemyBrain brain, IWorldLocator locator)
        {
            this.brain = brain;
            this.locator = locator;
        }

        public bool CanExecute(BehaviourTreeAgent agent)
        {
            return true;
        }

        public NodeResult Execute(BehaviourTreeAgent agent)
        {
            var closest = locator.AtPosition(brain.Position, brain.LookDistance).OfType<RobotPlayerPart>().FirstOrDefault(); // Use sensory information instead of lookup?
            brain.TargetPlayer = closest;
            return NodeResult.Success;
        }
    }
}