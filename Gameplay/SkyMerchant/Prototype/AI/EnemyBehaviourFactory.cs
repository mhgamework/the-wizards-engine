using System;
using Castle.Core;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant._Engine.Windsor;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.AI
{
    /// <summary>
    /// NOTE: looks programming wise like a EnemyBehaviourPart instead of a factory!
    /// </summary>
    public class EnemyBehaviourFactory
    {
        private EnemyBrain brain;

        #region Injection
        [NonOptional]
        public ISimulationEngine SimulationEngine { get; set; }
        [NonOptional]
        public IWorldLocator WorldLocator { get; set; }
        [NonOptional]
        public TraderPart.IItemFactory ItemFactory { get; set; }
        [NonOptional]
        public Random Random { get; set; }
        /// <summary>
        /// WARNING! NOT A SERVICE!! NOT SINGLETON
        /// </summary>
        [DoNotWire]
        public EnemyBrain Brain
        {
            get { return brain; }
            set { brain = value; }
        }

        public IPositionComponent Physical { get; set; }
        #endregion

        public IBehaviourNode CreateGuardPosition(Func<Vector3> getPosition)
        {
            return new ConcurrentSelector(
                CreateFindTarget(),
                new ConcurrentSelector(
                    new SingleActionBehaviour(() => Brain.Destination = getPosition()),
                    new MoveToDestinationBehaviour(Brain, SimulationEngine, Physical)
                )

                );
        }

        public IBehaviourNode CreateFindTarget()
        {
            return new FindTargetBehaviour(Brain, WorldLocator);
        }

        public IBehaviourNode CreateChaseTarget()
        {
            return new ConcurrentSelector(
                new Condition(ag => Brain.TargetPlayer != null &&
                    Vector3.Distance(Brain.TargetPlayer.Physical.Position, Brain.Position) < Brain.LookDistance),
                new SingleActionBehaviour(delegate
                {
                    if (Brain.TargetPlayer == null) return;
                    Brain.Destination = Brain.TargetPlayer.Physical.Position;
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
                    new SingleActionBehaviour(() => Brain.Destination = Brain.TargetItem.Physical.Position),
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