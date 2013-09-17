using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Gameplay;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Development
{
    /// <summary>
    /// 
    /// </summary>
    [EngineTest]
    [TestFixture]
    public class QuestEditorTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();

        [Test]
        public void TestPickupMoveDrop()
        {
            //var item = createTestItem();
            //SpectaterCamera cam = getCamera();
            //var mover = new WorldObjectMover(cam);

            //cam.CameraPosition = new Vector3(2,2,2);
            //cam.CameraDirection = Vector3.UnitX;

            //engine.AddSimulator(new BasicSimulator(delegate
            //    {
            //        if (TW.Graphics.Keyboard.IsKeyPressed())
            //    }));
        }
    }
}