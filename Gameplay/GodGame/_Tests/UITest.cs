using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.GodGame.UI;
using NUnit.Framework;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    [EngineTest]
    [TestFixture]
    public class UITest
    {
        [Test]
        public void TestValueControl()
        {
            var engine = EngineFactory.CreateEngine();
            var control = new ValueControl();

            engine.AddSimulator(new BasicSimulator(() =>
                {
                    if (TW.Graphics.Mouse.LeftMouseJustPressed)
                        control.TryLeftClick(TW.Data.Get<CameraInfo>().GetCenterScreenRay());
                    else if (TW.Graphics.Mouse.RightMouseJustPressed)
                        control.TryRightClick(TW.Data.Get<CameraInfo>().GetCenterScreenRay());

                    control.GetRenderable().Update();
                }));
            engine.AddSimulator(new WorldRenderingSimulator());

            control.GetRenderable().Show();
        }
         
    }
}