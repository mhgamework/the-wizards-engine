using System;
using System.Linq;
using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.Engine.Features.Testing;
using NUnit.Framework;

namespace MHGameWork.TheWizards
{
    [EngineTest]
    [TestFixture]
    public class TWMathTest
    {
        [Test]
        public void TestNFMod()
        {
            Assert.AreEqual(0, TWMath.nfmod(0, 5));
            Assert.AreEqual(0, TWMath.nfmod(5, 5));
            Assert.AreEqual(0, TWMath.nfmod(10, 5));
            Assert.AreEqual(0, TWMath.nfmod(-5, 5));
            Assert.AreEqual(0, TWMath.nfmod(-10, 5));

            Assert.AreEqual(3, TWMath.nfmod(-2, 5));
            Assert.AreEqual(3, TWMath.nfmod(-7, 5));
        }
        [Test]
        public void TestNFModSpeed()
        {
            var times = 10000000;

            var list = new int[times];
            var rand = new Random(0);
            for (int i = 0; i < times; i++)
            {
                list[i] = rand.Next(-times / 2, times / 2);
            }
            var b = 8;
            var a = 15;
            var modTwice = PerformanceHelper.Measure(() =>
                {
                    for (int i = 0; i < times; i++)
                    {
                        a = list[i];
                        var mod = (a % b + b) % b;
                    }
                });
            Console.WriteLine(modTwice.PrettyPrint());

            var modIf = PerformanceHelper.Measure(() =>
           {
               for (int i = 0; i < times; i++)
               {
                   a = list[i];
                   int r = a % b;
                   r = r < 0 ? r + b : r;
               }
           });
            Console.WriteLine(modIf.PrettyPrint());


            var floor = PerformanceHelper.Measure(() =>
            {
                for (int i = 0; i < times; i++)
                {
                    a = list[i];

                    var mod = (a - b * (int)Math.Floor(a / (double)b));
                }
            });
            Console.WriteLine(floor.PrettyPrint());

            Assert.Less(modIf, modTwice);
            Assert.Less(modIf, floor);
        }
    }
}