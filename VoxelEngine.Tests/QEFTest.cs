using System;
using System.Linq;
using MHGameWork.TheWizards.Engine.Features.Testing;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.DualContouring
{
    [EngineTest]
    [TestFixture]
    public class QEFTest
    {

        [Test]
        public void TestQEF_Simple()
        {
            var normals = new[] { Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ };
            var posses = new[] { Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ };
            var result = QEFCalculator.CalculateCubeQEF(normals, posses, new Vector3(1, 1, 1)).ToArray();
            result.Print();
            CollectionAssert.AreEqual(new float[] { 1, 1, 1 }, result);
        }
        [Test]
        public void TestQEF_UnderDetermined()
        {
            var normals = new[] { Vector3.UnitX, Vector3.UnitY, Vector3.UnitX, Vector3.UnitY };
            var posses = new[] { Vector3.UnitX * 2, Vector3.UnitY * 2, Vector3.UnitX * 2 + Vector3.UnitZ * 2, Vector3.UnitY * 2 + Vector3.UnitZ * 2 };
            var result = QEFCalculator.CalculateCubeQEF(normals, posses, new Vector3(1, 1, 1)).ToArray();
            result.Print();
            CollectionAssert.AreEqual(new float[] { 2, 2, 1 }, result);

        }


        /// <summary>
        /// From a box added to sphere, on the edge between box and sphere, point should be inside the cube specified
        /// </summary>
        [Test]
        public void TestQEF_Complex()
        {
            var p1 = new Vector3(0.4191761f, 1, 0);
            var p2 = new Vector3(0.8585715f, 1, 1);
            var p3 = new Vector3(1, 0.1983433f, 0);
            var p4 = new Vector3(1, 0.5505102f, 1);
            var n1 = new Vector3(-1, 0, 0);
            var n2 = new Vector3(-0.8926786f, -0.25f, -0.375f);
            var n3 = new Vector3(0, -1, 0);
            var n4 = new Vector3(-0.875f, -0.3061862f, -0.375f);

            var qef = QEFCalculator.CalculateCubeQEF(new Vector3[] { n1, n2, n3, n4 }, new Vector3[] { p1, p2, p3, p4 },
                                           new Vector3[] { p1, p2, p3, p4 }.Aggregate((a, b) => a + b) / 4f);
            var v = new Vector3(qef[0], qef[1], qef[2]);
            Console.WriteLine(v);
            //TODO: this should be inside of cube but isnt!
        }



    }
}