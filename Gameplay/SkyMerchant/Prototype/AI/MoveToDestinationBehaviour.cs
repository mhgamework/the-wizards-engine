﻿using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.RTSTestCase1.BehaviourTrees;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.AI
{
    public class MoveToDestinationBehaviour : IBehaviourNode
    {
        private readonly EnemyBrain brain;
        private readonly ISimulationEngine engine;
        private readonly Physical ph;

        /// <summary>
        /// Note: replace physical with a moveto delegate?(Action{Vector3})
        /// </summary>
        /// <param name="brain"></param>
        /// <param name="engine"></param>
        /// <param name="ph"></param>
        public MoveToDestinationBehaviour(EnemyBrain brain, ISimulationEngine engine, Physical ph)
        {
            this.brain = brain;
            this.engine = engine;
            this.ph = ph;
        }

        public bool CanExecute(BehaviourTreeAgent agent)
        {
            //TODO: add pathfinding here (check if obstructed)
            return true;
        }

        public NodeResult Execute(BehaviourTreeAgent agent)
        {
            if (Vector3.Distance(brain.Position, brain.Destination) < 0.01f) return NodeResult.Success;
            ph.MoveTo(brain.Destination, engine.Elapsed);
            return NodeResult.Running;
        }
    }
}