using MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.AI
{
    public class ShootBehaviour : IBehaviourNode
    {
        private readonly EnemyBrain brain;
        private readonly ISimulationEngine simulationEngine;

        private float lastShotTime = -1; // NOTE: THIS IS NOT IN THE DATA CLASSES!

        public ShootBehaviour(EnemyBrain brain, ISimulationEngine simulationEngine)
        {
            this.brain = brain;
            this.simulationEngine = simulationEngine;
        }

        public bool CanExecute(BehaviourTreeAgent agent)
        {
            return brain.TargetPlayer != null &&
                   Vector3.Distance(brain.Position, brain.TargetPlayer.Physical.Position) < brain.ShootDistance;
        }

        public NodeResult Execute(BehaviourTreeAgent agent)
        {
            if (simulationEngine.CurrentTime < lastShotTime + brain.ShootInterval) return NodeResult.Running;

            brain.TargetPlayer.ApplyDamage(brain.GunDamage);
            return NodeResult.Success;
        }
    }
}