using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.WorldResources;
using NUnit.Framework;
using System.Linq;

namespace MHGameWork.TheWizards.RTSTestCase1.Tests.Resources
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class ResourceGenerationTest
    {
        private WorldResourceGenerationSimulator sim;
        private Tree tree;

        [SetUp]
        public void Setup()
        {
            TestUtilities.SetupTWContext();

            sim = new WorldResourceGenerationSimulator();
            tree = new Tree();
        }


        [Test]
        public void TestGeneratesResources()
        {
            sim.Simulate(100);

            Assert.That(1, Is.EqualTo( TW.Data.Objects.OfType<IItem>().Count()));
            
            sim.Simulate(100);

            Assert.That(1, Is.EqualTo(TW.Data.Objects.OfType<IItem>().Count()));

            TW.Data.Objects.Remove((IModelObject) TW.Data.Objects.OfType<IItem>().First());

            Assert.That(0, Is.EqualTo(TW.Data.Objects.OfType<IItem>().Count()));

            sim.Simulate(100);

            Assert.That(1, Is.EqualTo(TW.Data.Objects.OfType<IItem>().Count()));
        }

    }
}