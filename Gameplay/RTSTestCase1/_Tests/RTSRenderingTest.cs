using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Navigation2D;
using MHGameWork.TheWizards.RTSTestCase1.Rendering;
using MHGameWork.TheWizards.RTSTestCase1.WorldResources;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    [TestFixture]
    [EngineTest]
    public class RTSRenderingTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [SetUp]
        public void Setup()
        {
            TestUtilities.CreateGroundPlane();
        }

        [Test]
        public void TestRenderResources()
        {
            new Tree {Position = new Vector3(3, 0, 3), Size = 0};
            new Tree { Position = new Vector3(6, 0, 3), Size = 5 };
            new Tree { Position = new Vector3(9, 0, 3), Size = 10 };

            new Rock { Position = new Vector3(3, 0, 8), Height = 0 };
            new Rock { Position = new Vector3(3, 0, 15), Height = 5 };
            new Rock { Position = new Vector3(3, 0, 22), Height = 10 };
            engine.AddSimulator(new RTSRendererSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }
    }
}
