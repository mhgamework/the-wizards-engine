using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;

namespace MHGameWork.TheWizards.WorldSimulation.Actions
{
    public class AtHome:IAction
    {
        private readonly Creature creature;
        private readonly Building build;
        private readonly float neededTime;
        private float totalElapsed;

        public AtHome(Creature creature,Building build,float timeAtHome)
        {
            this.creature = creature;
            this.build = build;
            this.neededTime = timeAtHome;
            build.Home = true;
        }

        public void Apply(float elapsed, Creature creature)
        {
            totalElapsed += elapsed;
            //totalElapsed = MathHelper.Clamp(totalElapsed, 0, neededTime);
        }

        public void End()
        {
            

        }

        public bool Fullfilled()
        {
            if (totalElapsed >= neededTime)
                return true;
            return false;
        }

        public void ForcedEnd()
        {
            build.Home = false;
        }
    }
}
