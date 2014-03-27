using System.Collections.Generic;
using System.Diagnostics.Contracts;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Features.Testing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Scattered._Engine;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered._Tests
{
    /// <summary>
    /// Tests for creating and managing an Island graph
    /// TODO: IDEA: solidify this test by making it automated using screenshots of specific times and positions
    /// </summary>
    [EngineTest]
    [TestFixture]
    [ContractVerification(true)]
    public class TestTextRectangle
    {
        private TWEngine engine = EngineFactory.CreateEngine();
        private List<TextRectangle> rectangles = new List<TextRectangle>();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestBasic()
        {
            var panel = createRectangle();
            panel.Position = new Vector3();
            panel.Radius = 10;
            panel.Normal = Vector3.UnitZ;
            panel.Text = "This is the Text panel test\n The world will now\n ...";

            visualize();

        }



        [Test]
        public void TestTransform()
        {
            var panel = createRectangle();
            panel.Position = new Vector3(10, 0, 0);
            panel.Radius = 3;
            panel.Normal = Vector3.UnitX;
            panel.Text = "X axis";


            panel = createRectangle();
            panel.Position = new Vector3(0, 10, 0);
            panel.Radius = 3;
            panel.Normal = Vector3.UnitY;
            panel.Text = "Y axis";

            panel = createRectangle();
            panel.Position = new Vector3(0, 0, 10);
            panel.Radius = 3;
            panel.Normal = Vector3.UnitZ;
            panel.Text = "Z axis";


            visualize();

        }

        [Test]
        public void TestBillboard()
        {
            var panel = createRectangle();
            panel.Position = new Vector3(0, 3, -5);
            panel.Radius = 5;
            panel.Normal = Vector3.UnitZ;
            panel.IsBillboard = true;
            panel.Text = "This is the Text panel test\n The world will now\n ...";

            visualize();

        }


        private TextRectangle createRectangle()
        {
            var ret = new TextRectangle();
            rectangles.Add(ret);
            return ret;
        }

        private void visualize()
        {
            engine.AddSimulator(new BasicSimulator(() => rectangles.ForEach(r => r.Update())));
            engine.AddSimulator(new WorldRenderingSimulator());
        }
    }
}