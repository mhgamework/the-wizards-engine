using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.AI
{
    public class PickupTargetItemBehaviour : IBehaviourNode
    {
        private readonly EnemyBrain brain;
        private readonly TraderPart.IItemFactory itemFactory;

        /// <summary>
        /// Note: replace physical with a moveto delegate?(Action{Vector3})
        /// </summary>
        /// <param name="brain"></param>
        /// <param name="itemFactory"></param>
        public PickupTargetItemBehaviour(EnemyBrain brain, TraderPart.IItemFactory itemFactory)
        {
            this.brain = brain;
            this.itemFactory = itemFactory;
        }

        public bool CanExecute(BehaviourTreeAgent agent)
        {
            return brain.TargetItem != null &&
                   Vector3.Distance(brain.TargetItem.Physical.Position, brain.Position) < 1;
        }

        public NodeResult Execute(BehaviourTreeAgent agent)
        {
            itemFactory.Destroy(brain.TargetItem);
            brain.TargetItem = null;
            return NodeResult.Success;
        }
    }
}