using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTS
{
    public class EnemyAISimulator : ISimulator
    {
        public void Simulate()
        {
            var goblins = TW.Data.Objects.Where(o => o is Goblin).Cast<Goblin>().ToArray();
            foreach (Goblin g in goblins)
                simulateEnemyAI(g);
        }

        private void simulateEnemyAI(Goblin goblin)
        {
            var cmds = new[] {new AttackPlayer()};
            var c = cmds.OrderBy(o => o.GetTravelDistance(goblin)).First();
            c.Execute(goblin);
        }

     
        private Vector3 getNexusPosition()
        {
            throw new NotImplementedException();
        }

        private interface ICommand
        {
            float GetTravelDistance(Goblin g);
            void Execute(Goblin g);
        }
        private class AttackPlayer :ICommand
        {
            private PlayerRTS player = TW.Data.GetSingleton<PlayerRTS>();

            public float GetTravelDistance(Goblin g)
            {
                return Vector3.Distance(g.Position, player.GetPosition());
            }

            public void Execute(Goblin g)
            {
                if (GetTravelDistance(g) > 0.1f)
                {
                    g.Goal = player.GetPosition();
                    return;
                }
                player.Die();
            }
        }
    }
}
