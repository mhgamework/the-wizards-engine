using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.RTSTestCase1.Inputting;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1._Tests
{
    [TestFixture]
    [EngineTest]
    public class InputTest
    {
        private TWEngine engine = EngineFactory.CreateEngine();
        [Test]
        public void TestInput()
        {
            var button = TW.Data.Get<InputFactory>().GetButton("moveForward");

            engine.AddSimulator(new InputSimulator());

            engine.AddSimulator(new BasicSimulator(delegate
                {
                    if (button.Down) TW.Graphics.LineManager3D.AddLine(new Vector3(), new Vector3(1, 1, 1), new Color4(1, 0, 0));
                }));
        }
    }
}
