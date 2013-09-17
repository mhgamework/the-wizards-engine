using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1.Magic;
using MHGameWork.TheWizards.RTSTestCase1.Players;

using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins.Spawning
{

    public class GoblinSpawner
    {
        private readonly IGoblinCreator goblinCreator;
        private readonly IWorldLocator locator;
        public GoblinSpawner(IGoblinCreator goblinCreator, IWorldLocator locator)
        {
            this.goblinCreator = goblinCreator;
            this.locator = locator;
        }

        public void Simulate(float elapsedTime, IGoblinSpawnPoint point)
        {
            var totalenergy = locator.AtPosition(point.Position, 5).
                OfType<ICrystal>().Aggregate(0f, (x, y) => x + y.GetEnergy());

            if (totalenergy < 10f)
               return;
            point.SpawnTime-= elapsedTime;
            if ((point.SpawnTime > 0)) return;
            goblinCreator.CreateGoblin(point.Position + new Vector3(point.SpawnTime*30f, 0, 3));
            point.SpawnTime += 10f;
        }
    }
    public interface IGoblinCreator
    {
        IGoblin CreateGoblin(Vector3 position);
    }

    public interface IPlayer
    {
        Vector3 GetPosition();
    }

    public interface IGoblin
    {
    }

    public interface IGoblinSpawnPoint
    {
        float SpawnTime
        {
            get;
            set;
        }
        Vector3 Position { get; set; }
    }
}