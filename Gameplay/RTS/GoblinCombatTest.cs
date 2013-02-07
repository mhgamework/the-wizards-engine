using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Audio;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTS.Commands;
using MHGameWork.TheWizards.Simulators;
using NUnit.Framework;
using SlimDX;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.RTS
{
    [TestFixture]
    [EngineTest]
    public class GoblinCombatTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [Test]
        public void TestRocketCannon()
        {
            new Cannon() { Angle = MathHelper.Pi, Position = new Vector3(2, 0, 2) };
            for (int i = 0; i < 10; i++)
            {
                new Goblin() { Position = new Vector3(i, 0, 1) };
                
            }
            for (int i = 0; i < 8; i++)
            {
                var daThing = new Thing() { Type = TW.Data.GetSingleton<RTSData>().RocketResourceType };
                new DroppedThing() { Thing = daThing, InitialPosition = new Vector3(i, 0, 2.5f) };
                
            }
            
            
            engine.AddSimulator(new CannonSimulator());
            engine.AddSimulator(new RTSRendererSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new AudioSimulator());

        }


        [Test]
        public void TestCannon()
        {
            new Cannon() { Angle = MathHelper.Pi, Position = new Vector3(2, 0, 2) };
            new Cannon() { Angle = 0, Position = new Vector3(-2, 0, 2) };
            for (int i = 0; i < 20; i++)
            {
                new Goblin() { Position = new Vector3(0, 0, 7 + i) };
            }

            //new SoundEmitter() { Ambient = true, Loop = true, Sound = SoundFactory.Load("RTS\\thunderstorm.wav"), Playing = true};

            engine.AddSimulator(new CannonSimulator());
            engine.AddSimulator(new RTSRendererSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new AudioSimulator());
        }

        [Test]
        public void TestEnemyAI()
        {
            //new Cannon() { Angle = MathHelper.Pi, Position = new Vector3(2, 0, 2) };
            //new Cannon() { Angle = 0, Position = new Vector3(-2, 0, 2) };
            for (int i = 0; i < 1; i++)
            {
                new Goblin() { Position = new Vector3(5, 0, 7 + i) };
            }

            //new SoundEmitter() { Ambient = true, Loop = true, Sound = SoundFactory.Load("RTS\\thunderstorm.wav"), Playing = true};

            TW.Data.GetSingleton<NavigableGrid2DData>().Size = 10;
            TW.Data.GetSingleton<NavigableGrid2DData>().NodeSize = 0.5f;


            engine.AddSimulator(new BIGBarrelPlacerSimulator());
            engine.AddSimulator(new NavigableGrid3DEntitySimulator());
            engine.AddSimulator(new NavigableGrid2DVizualizationSimulator());
            engine.AddSimulator(new EnemyAISimulator());
            engine.AddSimulator(new GoblinSimpleCrowdControlSimulator());
            engine.AddSimulator(new GoblinMovementSimulator());
            engine.AddSimulator(new RTSRendererSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }


        /// <summary>
        /// TODO: design flaw ==> dont make simulators reusable, put the reusable featuers in a seperate class!!
        /// </summary>
        [PersistanceScope]
        public class BIGBarrelPlacerSimulator : ISimulator
        {
            public void Simulate()
            {
                var ray = TW.Data.GetSingleton<CameraInfo>().GetCenterScreenRay();

                if (TW.Graphics.Keyboard.IsKeyDown(Key.F))
                {
                    var plane = new Plane(new Vector3(0, 1, 0), 0);
                    var dist = ray.xna().Intersects(plane.xna());
                    if (dist.HasValue)
                        new Engine.WorldRendering.Entity
                        {
                            WorldMatrix = Matrix.Scaling(5, 5, 5) * Matrix.Translation(ray.Position + ray.Direction * dist.Value),
                            Solid = true,
                            Mesh = TW.Assets.LoadMesh("Core\\Barrel01")
                        };
                }
                if (TW.Graphics.Keyboard.IsKeyDown(Key.G))
                {
                    var result = TW.Data.GetSingleton<Engine.WorldRendering.World>().Raycast(ray);
                    if (result.IsHit) TW.Data.RemoveObject((Engine.WorldRendering.Entity)result.Object);
                }
            }
        }
    }
}
