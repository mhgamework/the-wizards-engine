using System;
using DirectX11;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.VoxelTerraining;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Generation;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.RTS.Various
{
    [TestFixture]
    [EngineTest]
    public class LightPlacementSimulatorTest
    {
        [Test]
        public void TestSimple()
        {
            var engine = EngineFactory.CreateEngine();

            TW.Data.GetSingleton<CameraInfo>().Mode = CameraInfo.CameraMode.FirstPerson;
            engine.AddSimulator(new FirstPersonCameraSimulator());
            engine.AddSimulator(new LightPlacementSimulator());
            engine.AddSimulator(new TerrainEditorSimulator());
            engine.AddSimulator(new VoxelTerrainSimulator());
            engine.AddSimulator(new PointLightSimulator());
            engine.AddSimulator(new WorldRenderingSimulator());
            engine.Run();
        }
    }
}
