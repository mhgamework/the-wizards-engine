using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.Persistence;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.RTS.Tests
{
    [TestFixture]
    [EngineTest]
    public class GoblinsTest
    {
        [Test]
        public void TestAttackPlayer()
        {
            var engine = EngineFactory.CreateEngine();

            TW.Data.Get<Datastore>().LoadFromFile(new FileInfo(TWDir.GameData + "\\Saves\\Lights2.xml"));

            var g = new Goblin() {Position = new Vector3(3, 16, 3)};
            TW.Data.Get<Datastore>().Persist(g);

            var attack = new GoblinAttackPlayerBehaviour();

            engine.AddSimulator(new BasicSimulator(delegate
                {
                    attack.TryAttack(g);
                }));
            TW.Data.Get<CameraInfo>().Mode = CameraInfo.CameraMode.FirstPerson;
            engine.AddSimulator(new FirstPersonCameraSimulator());
            engine.AddSimulator(new GoblinMovementSimulator());
            engine.AddSimulator(new GoblinRendererSimulator());
            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new FirstPersonCameraSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
        }
    }
}
