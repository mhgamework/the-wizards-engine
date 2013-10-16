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
    [ModelObjectChanged]
    public class ProximityChaseEnemyPart : EngineModelObject
    {
        #region Injection
        public IPositionComponent Physical { get; set; }
        private EnemyBehaviourFactory BehaviourFactory;
        [DoNotWire]
        public EnemyBrain Brain { get; set; }
        #endregion

        private IBehaviourNode behaviourTree;
        private BehaviourTreeAgent agent;

        public ProximityChaseEnemyPart(EnemyBehaviourFactory behaviourFactory, EnemyBrain brain)
        {
            BehaviourFactory = behaviourFactory;
            behaviourTree = CreateBehaviourTree();
            agent = new BehaviourTreeAgent(behaviourTree);
            GunChargedTime = -1;
            brain.LookDistance = 20;
            brain.ShootDistance = 5;
            brain.ShootInterval = 2;
            brain.GunDamage = 20;
            Brain = brain;
        }


        public Vector3 GuardPosition { get; set; }

        /// <summary>
        /// Time when gun is ready to fire.
        /// </summary>
        public float GunChargedTime { get; set; }

        public float GunDamage { get; set; }


        public void SimulateBehaviour()
        {
            var oldPos = Physical.Position;

            Brain.UpdatePercepts(Physical.Position);
            if (behaviourTree.CanExecute(agent))
                behaviourTree.Execute(agent);

            var dir = Vector3.Normalize((Physical.Position - oldPos).ChangeY(0));
            if (dir.Length() > 0.5f)
            {
                Physical.Rotation =Functions.CreateFromLookDir(dir.xna()).dx()
                                    * Quaternion.RotationAxis(Vector3.UnitY, MathHelper.PiOver2);
                Physical.Position = Physical.Position;
            }


        }

        private IBehaviourNode CreateBehaviourTree()
        {
            // Shoot, Chase, Idle
            var ret = new PrioritySelector(
                BehaviourFactory.CreateShootTarget(),
                BehaviourFactory.CreateChaseTarget(),
                BehaviourFactory.CreateGuardPosition(() => GuardPosition));


            return ret;
        }



    }
}