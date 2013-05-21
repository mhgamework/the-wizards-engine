using System;
using System.Linq;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.MathExtra;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.RTS
{
    public class GoblinSimulator : ISimulator
    {
        public void Simulate()
        {
            var elapsedTime = TW.Graphics.Elapsed;
            SimulateEvilSpawners();
            SimulateSpawner(elapsedTime);
            SimulateGoblins(elapsedTime);
        }

        private void SimulateEvilSpawners()
        {
            if (TW.Data.Objects.Where(o => o is Goblin).Count() > 10) return;
            TW.Data.EnsureAttachment<EvilGoblinSpawner, EvilGoblinSpawnerData>(e => new EvilGoblinSpawnerData());
            var copy = TW.Data.Objects.Where(o => o is EvilGoblinSpawner).Cast<EvilGoblinSpawner>().ToArray();
            foreach (var spawner in copy)
            {
                if (TW.Graphics.TotalRunTime > spawner.get<EvilGoblinSpawnerData>().NextSpawn)
                {
                    new Goblin() { Position = spawner.Position };
                    spawner.get<EvilGoblinSpawnerData>().NextSpawn = TW.Graphics.TotalRunTime + 4;
                }
            }
        }

        private Random a = new Random();

        public void SimulateGoblins(float elapsedTime)
        {

            IModelObject[] goblins = TW.Data.Objects.Where(gob => gob is Goblin).ToArray();
            if (goblins.Length > 30)
                goblins = goblins.Skip(goblins.Length - 30).ToArray();
            //getNextFriend(goblins);

            foreach (var goblin in TW.Data.GetChangedObjects<Goblin>())
            {
                if (goblin.get<GoblinMover>() == null)
                    goblin.set(new GoblinMover(goblin));
                if (goblin.get<Engine.WorldRendering.Entity>() == null)
                    goblin.set(new Engine.WorldRendering.Entity());
                if (goblin.get<GoblinRenderData>() == null)
                    goblin.set(new GoblinRenderData { LookDirection = new Vector3(0, 0, 1) });


                //setBasicEntities(goblin);
                var ent = goblin.get<Engine.WorldRendering.Entity>();
                fixRendering(goblin, ent);
            }

            updateAllGoblins(goblins);
        }
        private void updateAllGoblins(IModelObject[] goblins)
        {
            var attack = new GoblinAttackPlayerBehaviour();

            foreach (Goblin goblin in goblins)
            {
                goblin.get<GoblinMover>().Update();
                attack.TryAttack(goblin);
            }
        }
        private static void fixRendering(Goblin goblin, Engine.WorldRendering.Entity ent)
        {
            var renderData = goblin.get<GoblinRenderData>();
            if ((renderData.LastPosition - goblin.Position).Length() > 0.001f)
            {
                renderData.LookDirection = goblin.Position - renderData.LastPosition;
            }
            renderData.LastPosition = goblin.Position;
            var quat = Functions.CreateFromLookDir(-Vector3.Normalize(renderData.LookDirection).xna());
            ent.WorldMatrix = Microsoft.Xna.Framework.Matrix.CreateFromQuaternion(quat).dx() * Matrix.Scaling(0.01f, 0.01f, 0.01f) *
                              Matrix.Translation(goblin.Position);

            renderData.LastPosition = goblin.Position;
            ent.Mesh = MeshFactory.Load("Goblin\\GoblinLowRes");
            ent.Solid = true;
            ent.Static = false;
        }

        private static void setBasicEntities(Goblin goblin)
        {

            if (goblin.BestFriend == null)
                goblin.BestFriend = goblin;

            goblin.get<GoblinMover>().MoveTo(goblin.BestFriend.Position);

        }

        private void getNextFriend(IModelObject[] goblins)
        {
            foreach (Goblin goblin in goblins.Cast<Goblin>().Where(goblin => a.Next(200) == 0))
            {
                if (a.Next(3) == 0)
                    goblin.Position = new Vector3(a.Next(40), a.Next(40), a.Next(40));
                goblin.BestFriend = (Goblin)goblins[a.Next(goblins.Length)];
            }
        }

        public void SimulateSpawner(float elapsedTime)
        {
            foreach (GoblinSpawner spawner in TW.Data.Objects.Where(o => o is GoblinSpawner).ToArray())
            {
                spawner.remainingSpawnTime -= elapsedTime;
                if (!(spawner.remainingSpawnTime < 0)) continue;
                spawner.remainingSpawnTime += 2;
                var a = new Random();
                var spawnedGoblin = new Goblin() { Position = spawner.Position };
            }
            foreach (var spawner in TW.Data.GetChangedObjects<GoblinSpawner>())
            {
                if (spawner.get<Engine.WorldRendering.Entity>() == null)
                    spawner.set(new Engine.WorldRendering.Entity());

                var ent = spawner.get<Engine.WorldRendering.Entity>();

                ent.WorldMatrix = Matrix.Scaling(0.01f, 0.01f, 0.01f) * Matrix.Translation(spawner.Position);
                ent.Mesh = MeshFactory.Load("Goblin\\GoblinLowRes");
                ent.Solid = true;
                ent.Static = false;
            }
        }
        private class GoblinRenderData
        {
            public Vector3 LastPosition { get; set; }
            public Vector3 LookDirection { get; set; }
        }
        private class EvilGoblinSpawnerData : IModelObjectAddon<EvilGoblinSpawner>
        {
            public float NextSpawn = 0;
            public void Dispose()
            {

            }
        }
    }
}