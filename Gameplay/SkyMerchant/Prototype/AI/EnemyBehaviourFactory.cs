using System;
using Castle.Core;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using MHGameWork.TheWizards.SkyMerchant._Engine.Windsor;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.AI
{
    /// <summary>
    /// NOTE: looks programming wise like a EnemyBehaviourPart instead of a factory!
    /// </summary>
    public class EnemyBehaviourFactory : IGameObjectComponent
    {
        #region Injection
        public ISimulationEngine SimulationEngine { get; set; }
        public IWorldLocator WorldLocator { get; set; }
        public TraderPart.IItemFactory ItemFactory { get; set; }
        public Random Random { get; set; }
        public EnemyBrain Brain { get; set; }

        public IPositionComponent Physical { get; set; }
        #endregion

        public EnemyBehaviourFactory(EnemyBrain brain,
            ISimulationEngine simulationEngine,
            IWorldLocator worldLocator,
            TraderPart.IItemFactory itemFactory,
            Random random,
            IPositionComponent physical)
        {
            this.Brain = brain;
            SimulationEngine = simulationEngine;
            WorldLocator = worldLocator;
            ItemFactory = itemFactory;
            Random = random;
            Physical = physical;
        }


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