using System;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees;
using MHGameWork.TheWizards.RTSTestCase1._Common;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.AI
{
    /// <summary>
    /// NOTE: looks programming wise like a EnemyBehaviourPart instead of a factory!
    /// </summary>
    public class EnemyBehaviourFactory
    {
        #region Injection
        public ISimulationEngine SimulationEngine { get; set; }
        public IWorldLocator WorldLocator { get; set; }
        public TraderPart.IItemFactory ItemFactory { get; set; }
        /// <summary>
        /// WARNING! NOT A SERVICE!! NOT SINGLETON
        /// </summary>
        public EnemyBrain Brain { get; set; }
        public Physical Physical { get; set; }
        public Random Random { get; set; }
        #endregion

        public IBehaviourNode CreateGuardPosition(Func<Vector3> getPosition)
        {
            return new ConcurrentSelector(
                new FindTargetBehaviour(Brain, WorldLocator),
                new Sequence(
                    new SingleActionBehaviour(() => Brain.Destination = getPosition()),
                    new MoveToDestinationBehaviour(Brain, SimulationEngine, Physical)
                )

                );
        }

        public IBehaviourNode CreateChaseTarget()
        {
            return new ConcurrentSelector(
                new SingleActionBehaviour(delegate
                {
                    if (Brain.TargetPlayer == null) return;
                    Brain.Destination = Brain.TargetPlayer.Physical.GetPosition();
                }),

                new MoveToDestinationBehaviour(Brain, SimulationEngine, Physical)
                );
        }

        public IBehaviourNode CreateShootTarget()
        {
            return new ShootBehaviour(Brain, SimulationEngine);
        }

        public IBehaviourNode CreatePickupClosestResource(float range)
        {
            return new PrioritySelector(
                new PickupTargetItemBehaviour(Brain, ItemFactory),
                new Sequence(
                    new FindClosestResourceBehaviour(Brain, WorldLocator, range),
                    new SingleActionBehaviour(() => Brain.Destination = Brain.TargetItem.Physical.GetPosition()),
                    new MoveToDestinationBehaviour(Brain, SimulationEngine, Physical)
                    )
                );
        }

        public IBehaviourNode CreateExplore()
        {
            return new Sequence(
                new SingleActionBehaviour(() => Brain.Destination = new Vector3((float)Random.NextDouble() * 100 - 50, (float)Random.NextDouble() * 100 - 50, (float)Random.NextDouble() * 100 - 50) + Brain.Position),
                new MoveToDestinationBehaviour(Brain, SimulationEngine, Physical)
                );
        }
    }
}