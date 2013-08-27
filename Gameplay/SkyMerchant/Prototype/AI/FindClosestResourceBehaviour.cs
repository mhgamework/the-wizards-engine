using System.Linq;
using MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees;
using MHGameWork.TheWizards.RTSTestCase1._Common;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.AI
{
    public class FindClosestResourceBehaviour : IBehaviourNode
    {
        private readonly EnemyBrain brain;
        private readonly IWorldLocator locator;
        private readonly float range;

        public FindClosestResourceBehaviour(EnemyBrain brain, IWorldLocator locator, float range)
        {
            this.brain = brain;
            this.locator = locator;
            this.range = range;
        }

        public bool CanExecute(BehaviourTreeAgent agent)
        {
            brain.TargetItem = null; //WARNING: probably a bad idea this line is
            return locator.AtPosition(brain.Position, range).OfType<ItemPart>().Any();
        }

        public NodeResult Execute(BehaviourTreeAgent agent)
        {
            var closest = locator.AtPosition(brain.Position, range).OfType<ItemPart>().FirstOrDefault(); // Use sensory information instead of lookup?
            brain.TargetItem = closest;
            return NodeResult.Success;
        }
    }
}