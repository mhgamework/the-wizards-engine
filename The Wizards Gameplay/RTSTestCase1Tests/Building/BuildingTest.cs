using System.Linq;
using MHGameWork.TheWizards.RTSTestCase1.Building;
using MHGameWork.TheWizards.RTSTestCase1.Cannons;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1._Common;
using MHGameWork.TheWizards.RTSTestCase1._Tests;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Tests.Building
{
    [TestFixture]
    public class BuildingTest
    {
        private IBuildable buildable;
        private SimpleBuilder builder;

        [SetUp]
        public void Setup()
        {
            TestUtilities.SetupTWContext();
            var locator = new SimpleWorldLocator();
            builder = new SimpleBuilder(locator, new SimpleWorldDestroyer());

            builder.BuildRange = 5;

            buildable = new Cannon();
            buildable.Buildable.ResetBuild();

        }

        [Test]
        public void TestStartAtZeroProgress()
        {
            Assert.That(buildable.Buildable.BuildProgress, Is.EqualTo(0));

        }

        [Test]
        public void TestNoBuildWhenNoResources()
        {
            builder.BuildSingleResource(buildable);
            Assert.That(buildable.Buildable.BuildProgress, Is.EqualTo(0));
        }

        [Test]
        public void TestSingleBuild()
        {
            var r = buildable.Buildable.RequiredResources.First();
            new DroppedThing() { Thing = new Thing() { Type = r } };

            builder.BuildSingleResource(buildable);
            Assert.That(buildable.Buildable.BuildProgress, Is.GreaterThan(0));
        }

        [Test]
        public void TestInvalidResourceType()
        {
            new DroppedThing() { Thing = new Thing() { Type = new ResourceType() } };

            builder.BuildSingleResource(buildable);
            Assert.That(buildable.Buildable.BuildProgress, Is.EqualTo(0));
        }

        [Test]
        public void TestOutOfRange()
        {
            var r = buildable.Buildable.RequiredResources.First();
            var d = new DroppedThing() { Thing = new Thing() { Type = r } };

            d.Physical.WorldMatrix = Matrix.Translation(6, 0, 0);

            builder.BuildSingleResource(buildable);
            Assert.That(buildable.Buildable.BuildProgress, Is.EqualTo(0));
        }

        [Test]
        public void TestComplete()
        {
            foreach (var r in buildable.Buildable.RequiredResources)
                new DroppedThing() { Thing = new Thing() { Type = r } };

            foreach (var r in buildable.Buildable.RequiredResources)
                builder.BuildSingleResource(buildable);

            Assert.That(buildable.Buildable.BuildProgress, Is.EqualTo(1));
        }
    }

  
}