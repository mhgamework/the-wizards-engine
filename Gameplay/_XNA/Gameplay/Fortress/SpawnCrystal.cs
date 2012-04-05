using System;
using MHGameWork.TheWizards._XNA.Scripting.API;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards._XNA.Gameplay.Fortress
{
    public class SpawnCrystal : IScript, IUpdateHandler
    {
        private float sinceSpawn;
        private IEntityHandle handle;

        private float spawnRate = 2;

        private Seeder seeder = new Seeder(984654);

        public bool SpawnDisabled { get; set; }

        public void Init(IEntityHandle handle)
        {
            this.handle = handle;
            handle.RegisterUpdateHandler();
        }

        public void Destroy()
        {

        }

        public void Update()
        {
            sinceSpawn += handle.Elapsed;

            if (sinceSpawn > spawnRate)
            {
                Spawn();
                sinceSpawn -= spawnRate;
            }

        }

        private void Spawn()
        {
            if (SpawnDisabled) return;
            var ent = handle.CreateEntity();
            ent.Mesh = handle.Mesh;
            ent.Static = false;
            ent.Solid = true;
            var angle = seeder.NextFloat(0, MathHelper.TwoPi);
            var radius = 2f;
            ent.Position = handle.Position + Vector3.UnitX * (float)Math.Sin(angle) * radius +
                           Vector3.UnitZ * (float)Math.Cos(angle) * radius;

        }
    }
}
