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
             Assert.AreEqual(0,TWMath.nfmod(0,5));
             Assert.AreEqual(0, TWMath.nfmod(5, 5));
             Assert.AreEqual(0, TWMath.nfmod(10, 5));
             Assert.AreEqual(0, TWMath.nfmod(-5, 5));
             Assert.AreEqual(0, TWMath.nfmod(-10, 5));

             Assert.AreEqual(3, TWMath.nfmod(-2, 5));
             Assert.AreEqual(3, TWMath.nfmod(-7, 5));
         }
    }
}