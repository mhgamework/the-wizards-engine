using System;
using System.Linq;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using MHGameWork.TheWizards.Scattered._Engine;
using Microsoft.Xna.Framework;
using SlimDX;
using Matrix = SlimDX.Matrix;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class EnemySpawningSimulator : ISimulator
    {
        private Level level;
        private float remainingTimeForSpawn;
        private float spawnTime;
        private Random random;
        public EnemySpawningSimulator(Level level, float spawnTime)
        {
            this.level = level;
            this.spawnTime = spawnTime;
            remainingTimeForSpawn = spawnTime;
            random = new Random(0);
        }

        public void Simulate()
        {
            return; // No spawning :(
            remainingTimeForSpawn -= TW.Graphics.Elapsed;
            if (remainingTimeForSpawn > 0)
                return;
            remainingTimeForSpawn += spawnTime;
            var islands = level.Islands;
            var islandNumber = random.Next(islands.Count());
            if (islandNumber < 0)
                return;
            var island = islands.Skip(islandNumber).Take(1).First();
            if (island.Addons.Any(a => a is Enemy))
                return;
            island.AddAddon(new Enemy(level,island.Node.CreateChild()).Alter(e=>e.Node.Relative = Matrix.Translation(0,1f,0)));
            
        }
    }
}