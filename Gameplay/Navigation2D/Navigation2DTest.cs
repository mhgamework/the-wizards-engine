using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Navigation2D
{
    [TestFixture]
    [EngineTest]
    public class Navigation2DTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();
        private Random random = new Random(4);

        [Test]
        public void TestNavigableGrid2DRandom()
        {
            engine.AddSimulator(new NavigableGrid3DEntitySimulator());
            engine.AddSimulator(new NavigableGrid2DVizualizationSimulator());


            engine.AddSimulator(new WorldRenderingSimulator());

            for (int i = 0; i < 20; i++)
            {
                new Engine.WorldRendering.Entity()
                {
                    Solid = true,
                    Mesh = TW.Assets.LoadMesh("Core\\Barrel01"),
                    WorldMatrix = Matrix.Translation((float)random.NextDouble() * 10, 0, (float)random.NextDouble() * 10)
                };
            }

        }

        [Test]
        public void TestNavigableGrid2DSolid()
        {
            engine.AddSimulator(new NavigableGrid3DEntitySimulator());
            engine.AddSimulator(new NavigableGrid2DVizualizationSimulator());


            engine.AddSimulator(new WorldRenderingSimulator());

            new Engine.WorldRendering.Entity()
                {
                    Solid = true,
                    Mesh = TW.Assets.LoadMesh("Core\\Barrel01"),
                    WorldMatrix = Matrix.Translation(2, 0, 2)
                };

            new Engine.WorldRendering.Entity()
            {
                Solid = false,
                Mesh = TW.Assets.LoadMesh("Core\\Barrel01"),
                WorldMatrix = Matrix.Translation(4, 0, 4)
            };
        }

        [Test]
        public void TestNavigableGrid2DMove()
        {
            engine.AddSimulator(new NavigableGrid3DEntitySimulator());
            engine.AddSimulator(new NavigableGrid2DVizualizationSimulator());


            engine.AddSimulator(new WorldRenderingSimulator());

            new Engine.WorldRendering.Entity()
            {
                Solid = true,
                Mesh = TW.Assets.LoadMesh("Core\\Barrel01"),
                WorldMatrix = Matrix.Translation(2, 0, 2)
            };

            new Engine.WorldRendering.Entity()
            {
                Solid = false,
                Mesh = TW.Assets.LoadMesh("Core\\Barrel01"),
                WorldMatrix = Matrix.Translation(4, 0, 4)
            };
        }

        public class MoveBarrelSimulator : ISimulator
        {
            private Data data = TW.Data.GetSingleton<Data>();
            private Vector3 start = new Vector3(4, 0, 1);
            private Vector3 end = new Vector3(4, 0, 9);
            public void Simulate()
            {
                if (data.Entity == null)
                    data.Entity = new Engine.WorldRendering.Entity()
                     {
                         Solid = true,
                         Mesh = TW.Assets.LoadMesh("Core\\Barrel01"),
                         WorldMatrix = Matrix.Translation(start)
                     };

                data.Time += TW.Graphics.Elapsed;


            }

            [ModelObjectChanged]
            public class Data : EngineModelObject
            {
                public Engine.WorldRendering.Entity Entity { get; set; }
                public float Time { get; set; }
                public int Direction { get; set; }
        }
        }
    }
}
