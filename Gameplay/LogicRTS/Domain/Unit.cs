using MHGameWork.TheWizards.LogicRTS.Framework;
using MHGameWork.TheWizards.WorldSimulation.Actions;

namespace MHGameWork.TheWizards.LogicRTS
{
    public class Unit : BaseGameComponent
    {
        public Team Team { get; private set; }
        public float Health { get; private set; }

        public void ApplyDamage(float amount)
        {
            Health -= amount;
            if (Health < 0) Die();
        }

        public void Die()
        {
            Health = 0;
            GameObject.Destroy();
        }

    }

    public class Team
    {
    }
}