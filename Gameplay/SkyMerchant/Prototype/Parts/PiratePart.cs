using System.Linq;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees;
using MHGameWork.TheWizards.RTSTestCase1._Common;
using MHGameWork.TheWizards.SkyMerchant.Prototype.AI;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    public class PiratePart
    {
        #region Injection
        public Physical Physical { get; set; }
        public EnemyBehaviourFactory BehaviourFactory { get; set; }
        #endregion

        private IBehaviourNode behaviourTree;
        private BehaviourTreeAgent agent;

        public PiratePart()
        {
            behaviourTree = CreateBehaviourTree();
            agent = new BehaviourTreeAgent(behaviourTree);
            Brain = new EnemyBrain();
            Brain.LookDistance = 30;
            Brain.ShootDistance = 5;
            Brain.ShootInterval = 2;
            Brain.GunDamage = 20;
        }

        public EnemyBrain Brain { get; set; }

        public void SimulateBehaviour()
        {
            Brain.UpdatePercepts(Physical.GetPosition());
            if (behaviourTree.CanExecute(agent))
                behaviourTree.Execute(agent);
        }

        private IBehaviourNode CreateBehaviourTree()
        {
            // Shoot, Chase, Idle
            var ret = new PrioritySelector(
                BehaviourFactory.CreateShootTarget(),
                BehaviourFactory.CreateChaseTarget(),
                BehaviourFactory.CreatePickupClosestResource(30),
                BehaviourFactory.CreateExplore());

            return ret;
        }



    }


}