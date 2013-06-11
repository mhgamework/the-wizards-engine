using System;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Spawning;
using MHGameWork.TheWizards.RTSTestCase1.Magic;
using NUnit.Framework;
using Rhino.Mocks;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    [TestFixture]
    [EngineTest]

    public class SpawningTest
    {

        [SetUp]
        public void Setup()
        {
        }
        /*
        [Test]
        public void TestWorkingCatch()
        {
            try
            {
                TestWorking();
            }
            catch (Exception ex)
            {
                DI.Get<IErrorLogger>().Log(ex, "Test working :) but error :(");
            }
        }
        
        [Test]
        public void TestWorking()
        {

            IGoblinCreator goblinCreator;
            ICrystal crystal;
            var spawner = CreateSpawner(out goblinCreator, out crystal);
            crystal.Expect(o => o.GetPosition()).Return(new Vector3(0, 0, 0));//stub change
            spawner.Simulate(10,spawner);
            goblinCreator.AssertWasCalled(x => x.CreateGoblin(Arg<Vector3>.Matches(y => true)));//check


        }
        [Test]
        public void TestCrystalToFar()
        {
            IGoblinCreator goblinCreator;
            ICrystal crystal;
            var spawner = CreateSpawner(out goblinCreator, out crystal);
            crystal.Expect(o => o.GetPosition()).Return(new Vector3(-19, 0, -19));//hier wil ik stubben
            spawner.Simulate(15);
            goblinCreator.AssertWasNotCalled(x => x.CreateGoblin(new Vector3(10, 0, 10)));//hier checken

        }

        
        private GoblinSpawner CreateSpawner(out IGoblinCreator goblinCreator, out ICrystal crystal)
        {
            var spawnedGoblin = MockRepository.GenerateMock<IGoblin>();
            goblinCreator = MockRepository.GenerateMock<IGoblinCreator>();
            crystal = MockRepository.GenerateMock<ICrystal>();
            var spawnPosition = new Vector3(10, 0, 10);
            goblinCreator.Stub(o => o.CreateGoblin(spawnPosition)).IgnoreArguments().Return(spawnedGoblin);
            var spawner = new GoblinSpawner(goblinCreator, crystal);
            return spawner;
        }*/
    }
}