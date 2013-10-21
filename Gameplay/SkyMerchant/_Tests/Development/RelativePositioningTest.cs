using System;
using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.SkyMerchant.GameObjects;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using NSubstitute;
using NUnit.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Development
{
    [Category("RunsAutomated")]
    [TestFixture]
    public class RelativePositioningTest
    {
        private Vector3 posA = new Vector3(1, 0, 0);
        private Quaternion rot90 = Quaternion.RotationAxis(Vector3.UnitY, MathHelper.PiOver2);
        private Quaternion rot270 = Quaternion.RotationAxis(Vector3.UnitY, -MathHelper.PiOver2);

        [Test]
        public void TestNoParent()
        {
            var rel = new RelativePositionComponent(Substitute.For<IPositionComponent>());

            rel.RelativePosition = posA;
            rel.RelativeRotation = rot90;

            Assert.AreEqual(posA, rel.Position);
            Assert.AreEqual(rot90, rel.Rotation);
        }
        [Test]
        public void TestWithParent()
        {
            var parent = Substitute.For<IRelativePositionComponent>();
            parent.Parent = null;
            var rel = new RelativePositionComponent(Substitute.For<IPositionComponent>());
            rel.Parent = parent;

            parent.Position.Returns(new Vector3());
            parent.Rotation.Returns(Quaternion.Identity);


            rel.RelativePosition = posA;
            rel.RelativeRotation = rot90;

            Assert.AreEqual(posA, rel.Position);
            Assert.AreEqual(rot90, rel.Rotation);


            parent.Position.Returns(new Vector3(0, 0, 5));

            Assert.AreEqual(new Vector3(1, 0, 5), rel.Position);
            Assert.AreEqual(rot90, rel.Rotation);

            parent.Rotation.Returns(rot270);

            Assert.True(rel.Position.IsSameAs(new Vector3(0, 0, 6)));
            Assert.True(Quaternion.Identity.IsSameAs(rel.Rotation));


        }

        [Test]
        public void TestDecoratedPositionComponent()
        {
            var pos = Substitute.For<IPositionComponent>();
            var rel = new RelativePositionComponent(pos);

            rel.RelativePosition = posA;
            rel.RelativeRotation = rot90;

            rel.UpdateDecoratedComponent();

            Assert.AreEqual(posA, pos.Position);
            Assert.AreEqual(rot90, pos.Rotation);
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [Test]
        public void TestDetectLoopSelf()
        {
            var comp1 = new RelativePositionComponent(Substitute.For<IPositionComponent>());
            comp1.Parent = comp1;
            var comp2 = new RelativePositionComponent(Substitute.For<IPositionComponent>());
        }
        [ExpectedException(typeof(InvalidOperationException))]
        [Test]
        public void TestDetectLoop1()
        {
            var comp1 = new RelativePositionComponent(Substitute.For<IPositionComponent>());
            var comp2 = new RelativePositionComponent(Substitute.For<IPositionComponent>());
            comp1.Parent = comp2;
            comp2.Parent = comp1;
        }
        [ExpectedException(typeof(InvalidOperationException))]
        [Test]
        public void TestDetectLoop3()
        {
            var comp1 = new RelativePositionComponent(Substitute.For<IPositionComponent>());
            var comp2 = new RelativePositionComponent(Substitute.For<IPositionComponent>());
            var comp3 = new RelativePositionComponent(Substitute.For<IPositionComponent>());
            var comp4 = new RelativePositionComponent(Substitute.For<IPositionComponent>());
            comp1.Parent = comp2;
            comp2.Parent = comp3;
            comp3.Parent = comp4;
            comp4.Parent = comp1;
        }

        
    }
}