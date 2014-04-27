using System;
using System.Linq;
using MHGameWork.TheWizards.Scattered.GameLogic.Objects;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered._Engine;
using Matrix = SlimDX.Matrix;
using Vector3 = SlimDX.Vector3;

namespace MHGameWork.TheWizards.Scattered.GameLogic.Services
{
    public class EnemySpawningService
    {
        private Level level;
        private float remainingTimeForSpawn;
        private float spawnTime;
        private Random random;
        public EnemySpawningService(Level level, float spawnTime)
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
            //island.AddAddon(new Enemy(level, island.Node.CreateChild(), new Vector3()).Alter(e => e.Node.Relative = Matrix.Translation(0, 1f, 0)));

        }
    }
}