using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Audio;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Simulators;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.RTS
{
    [TestFixture]
    [EngineTest]
    public class GoblinCombatTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [Test]
        public void TestCannon()
        {
            new Cannon() { Angle = MathHelper.Pi, Position = new Vector3(2, 0, 2) };
            new Cannon() { Angle = 0, Position = new Vector3(-2, 0, 2) };
            for (int i = 0; i < 20; i++)
            {
                new Goblin() { Position = new Vector3(0, 0, 7+i) };
            }

            //new SoundEmitter() { Ambient = true, Loop = true, Sound = SoundFactory.Load("RTS\\thunderstorm.wav"), Playing = true};

            engine.AddSimulator(new CannonSimulator());
            engine.AddSimulator(new RTSRendererSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new AudioSimulator());
        }

    }
}
