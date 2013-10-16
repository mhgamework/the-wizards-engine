using System.Linq;
using Castle.Core;
using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.MathExtra;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees;

using MHGameWork.TheWizards.SkyMerchant.Prototype.AI;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    /// <summary>
    /// AI behaviour 
    /// (search,chase, shoot, pickup)
    /// </summary>
    [ModelObjectChanged]
    public class PiratePart : EngineModelObject,IGameObjectComponent
    {
        private readonly EnemyBehaviourFactory behaviourFactory;
        public IPositionComponent Physical { get; set; }
        private IBehaviourNode behaviourTree;
        private BehaviourTreeAgent agent;

        public PiratePart(EnemyBehaviourFactory behaviourFactory, EnemyBrain brain, IPositionComponent Physical)
        {
            this.behaviourFactory = behaviourFactory;
            this.Brain = brain;
            this.Physical = Physical;
            behaviourTree = CreateBehaviourTree();
            agent = new BehaviourTreeAgent(behaviourTree);
            Brain.LookDistance = 20;
            Brain.ShootDistance = 5;
            Brain.ShootInterval = 2;
            Brain.GunDamage = 20;
        }

        [DoNotWire]
        public EnemyBrain Brain { get; set; }

        public void SimulateBehaviour()
        {
            var oldPos = Physical.Position;


            Brain.UpdatePercepts(Physical.Position);
            if (behaviourTree.CanExecute(agent))
                behaviourTree.Execute(agent);

            var dir = Vector3.Normalize((Physical.Position - oldPos).ChangeY(0));
            if (!(dir.Length() > 0.5f)) return;
            Physical.Rotation = Functions.CreateFromLookDir(dir.xna()).dx()
                                   * Quaternion.RotationAxis(Vector3.UnitY, MathHelper.PiOver2)
                                    * Quaternion.RotationAxis(Vector3.UnitY, MathHelper.PiOver2);
        }

        private IBehaviourNode CreateBehaviourTree()
        {
            // Shoot, Chase, Idle
            var ret = new ConcurrentSelector(behaviourFactory.CreateFindTarget(),
                new PrioritySelector(
                    behaviourFactory.CreateShootTarget(),
                    behaviourFactory.CreateChaseTarget(),
                    behaviourFactory.CreatePickupClosestResource(30),
                    behaviourFactory.CreateExplore()));

            return ret;
        }



    }


}