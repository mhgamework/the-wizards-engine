using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Components;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    public class GoblinBehaviourSimulator : ISimulator
    {
        public void Simulate()
        {
            throw new System.NotImplementedException();
        }

        public IBehaviourNode CreateTree()
        {
            var root = new PrioritySelector(GetFollow(), GetMoveSource(), GetMoveTarget());
            return root;
        }

        private IBehaviourNode GetFollow()
        {
            Goblin g = null;
            return RequireOrb(CommandFactory.Get.Follow,
                              getMoveInRange(() => g.Commands.GetOrb(CommandFactory.Get.MoveSource).Physical.GetPosition(),
                                              () => 1));
        }


        private IBehaviourNode GetMoveSource()
        {
            Goblin g = null;
            return RequireOrb(CommandFactory.Get.MoveSource,
                new Sequence(
                    getMoveInRange(() => g.Commands.GetOrb(CommandFactory.Get.MoveSource).Physical.GetPosition(), () => 0.1f),
                    getTakeItemStorage(() => (IItemStorage)g.Commands.GetOrb(CommandFactory.Get.MoveSource).CurrentHolder))
               );
        }

        private IBehaviourNode getTakeItemStorage(Func<IItemStorage> getItemStorage)
        {
            return new TakeItemStorage(getItemStorage);
        }

        private IBehaviourNode getMoveInRange(Func<Vector3> getPosition, Func<float> getRadius)
        {
            return new MoveInRange(getPosition, getRadius);
        }


        private IBehaviourNode GetMoveTarget()
        {
            Goblin g = null;
            return RequireOrb(CommandFactory.Get.MoveSource,
                new Sequence(
                    getMoveInRange(() => g.Commands.GetOrb(CommandFactory.Get.MoveSource).Physical.GetPosition(), () => 0.1f),
                    getDepositItemStorage(() => (IItemStorage)g.Commands.GetOrb(CommandFactory.Get.MoveSource).CurrentHolder)));
        }

        private IBehaviourNode getDepositItemStorage(Func<IItemStorage> getItemStorage)
        {
            return new DepositItemStorage(getItemStorage);
        }

        private IBehaviourNode GetGather()
        {
            Goblin g = null;
            float gatherRange = 10;

            return RequireOrb(CommandFactory.Get.Gather,
                getPickupDrop(
                    getDroppedThing: delegate
                        {
                            var droppedThing = TW.Data.Objects.OfType<DroppedThing>().First(o => Vector3.Distance(o.Physical.GetPosition(), g.Physical.GetPosition()) < gatherRange);
                            return droppedThing;
                        }));
        }

        private IBehaviourNode getPickupDrop(Func<DroppedThing> getDroppedThing)
        {
            Goblin g = null;
            return
                new While(
                    getSuccessCondition: () => g.ItemStorage.Items.Contains(getDroppedThing()),
                    getWhileCondition: () => !g.ItemStorage.IsFull && getDroppedThing().Item.Free,
                    child: new PrioritySelector(
                    getPickupDropAction(getDroppedThing),
                    getMoveInRange(
                        getPosition: () => getDroppedThing().Physical.GetPosition(),
                        getRadius: () => 0.1f)
                    ));
        }

        private IBehaviourNode RequireOrb(GoblinCommandType type, IBehaviourNode child)
        {
            var ret = new ConcurrentSelector(
               new Condition(g => g.Get<Goblin>().Commands.GetOrb(type) != null),
               child);
            return ret;
        }

        private IBehaviourNode getPickupDropAction(Func<DroppedThing> getDroppedThing)
        {
            return new PickupDropAction(getDroppedThing);
        }


        public class MoveInRange : IBehaviourNode
        {
            private readonly Func<Vector3> getGoal;
            private readonly Func<float> getRange;

            public MoveInRange(Func<Vector3> getGoal, Func<float> getRange)
            {
                this.getGoal = getGoal;
                this.getRange = getRange;
            }

            public bool CanExecute(BehaviourTreeAgent agent)
            {
                return true;
            }

            public NodeResult Execute(BehaviourTreeAgent agent)
            {
                Goblin goblin = null;

                var goal = this.getGoal(); //TODO

                goal.Y = 0;
                var toPlayer = goblin.Position - goal;

                goblin.MoveTo(goal);

                if (toPlayer.Length() > getRange())
                    return NodeResult.Running;


                goal = goblin.Position;
                goblin.MoveTo(goal);
                return NodeResult.Success;
            }

        }
        public class TakeItemStorage : IBehaviourNode
        {
            private readonly Func<IItemStorage> getStorage;

            public TakeItemStorage(Func<IItemStorage> getStorage)
            {
                this.getStorage = getStorage;
            }

            public bool CanExecute(BehaviourTreeAgent agent)
            {
                Goblin g = null;
                return !getStorage().ItemStorage.IsEmpty && g.ItemStorage.IsEmpty;
            }
            public NodeResult Execute(BehaviourTreeAgent agent)
            {
                Goblin g;
                throw new NotImplementedException();
                return NodeResult.Success;
            }
        }
        public class DepositItemStorage : IBehaviourNode
        {
            private readonly Func<IItemStorage> getStorage;

            public DepositItemStorage(Func<IItemStorage> getStorage)
            {
                this.getStorage = getStorage;
            }

            public bool CanExecute(BehaviourTreeAgent agent)
            {
                Goblin g = null;

                return !getStorage().ItemStorage.IsFull && !g.ItemStorage.IsEmpty;
            }
            public NodeResult Execute(BehaviourTreeAgent agent)
            {
                Goblin g;
                throw new NotImplementedException();
                return NodeResult.Success;
            }
        }

        public class PickupDropAction : IBehaviourNode
        {
            private readonly Func<DroppedThing> getDroppedThing;

            public PickupDropAction(Func<DroppedThing> getDroppedThing)
            {
                this.getDroppedThing = getDroppedThing;
            }

            public bool CanExecute(BehaviourTreeAgent agent)
            {
                Goblin g = null;

                return Vector3.DistanceSquared(g.Physical.GetPosition(), getDroppedThing().Physical.GetPosition()) <
                       0.01
                       && getDroppedThing().Item.Free;
            }

            public NodeResult Execute(BehaviourTreeAgent agent)
            {
                var t = getDroppedThing();

                //TODO:
                throw new NotImplementedException();
            }
        }

        private static Vector3 getTargetPos(Goblin goblin)
        {
            var orb = goblin.Commands.Orbs.FirstOrDefault(o => o.Type == TW.Data.Get<CommandFactory>().Follow);
            if (orb == null) throw new InvalidOperationException("Cannot perform this behaviour!");

            return orb.Physical.WorldMatrix.xna().Translation.dx();
        }

    }


}