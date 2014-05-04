using System;
using System.Linq;
using MHGameWork.TheWizards.Testing;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards._Tests._Manual.Math
{
    /// <summary>
    /// TODO: this test will probably be better when using a stratified uniform sampler to display the distribution.
    /// </summary>
    [TestFixture]
    public class GeometrySamplerTest
    {
        private readonly IRenderingTester test;
        private readonly Seeder seeder;
        private GeometrySampler s;


        public GeometrySamplerTest(IRenderingTester test, Seeder seeder)
        {
            this.test = test;
            this.seeder = seeder;
            s = new GeometrySampler(seeder);
        }


        [Test]
        public void TestCircle()
        {
            test.SetCameraPosition(new Vector3(0, 0, 3), new Vector3());
            displaySamples(1000, () => new Vector3(s.RandomPointOnCircle(), 0));
        }

        [Test]
        public void TestSphere()
        {
            test.SetCameraPosition(new Vector3(0, 0, 3), new Vector3());
            displaySamples(1000, () => s.RandomPointOnSphere());
        }

        private void displaySamples(int numSamples, Func<Vector3> func)
        {
            var points = Enumerable.Range(0, numSamples).Select(_ => func()).ToArray();

            test.ObserveUpdate(() => points.ForEach(p => test.LineManager3D.AddCenteredBox(p, 0.01f, new Color4(1, 1, 0))));
        }
    }
}