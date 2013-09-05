using System.Linq;
using Castle.Core;
using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.MathExtra;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees;
using MHGameWork.TheWizards.RTSTestCase1._Common;
using MHGameWork.TheWizards.SkyMerchant.Prototype.AI;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    /// <summary>
    /// AI behaviour 
    /// (search,chase, shoot, pickup)
    /// </summary>
    [ModelObjectChanged]
    public class PiratePart : EngineModelObject
    {
        private readonly EnemyBehaviourFactory behaviourFactory;

        #region Injection
        public Physical Physical { get; set; }
        #endregion

        private IBehaviourNode behaviourTree;
        private BehaviourTreeAgent agent;

        public PiratePart(EnemyBehaviourFactory behaviourFactory, EnemyBrain brain)
        {
            this.behaviourFactory = behaviourFactory;
            this.Brain = brain;
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
            var oldPos = Physical.GetPosition();


            Brain.UpdatePercepts(Physical.GetPosition());
            if (behaviourTree.CanExecute(agent))
                behaviourTree.Execute(agent);

            var dir = Vector3.Normalize((Physical.GetPosition() - oldPos).ChangeY(0));
            if (dir.Length() > 0.5f)
                Physical.WorldMatrix = Microsoft.Xna.Framework.Matrix.CreateFromQuaternion(Functions.CreateFromLookDir(dir.xna())).dx()
                                        * Matrix.RotationY(MathHelper.PiOver2)
                                        * Matrix.RotationY(MathHelper.PiOver2)
                                       * Matrix.Translation(Physical.GetPosition());
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