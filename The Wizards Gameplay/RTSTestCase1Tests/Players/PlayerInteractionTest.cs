using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Diagnostics.Tracing;
using MHGameWork.TheWizards.RTSTestCase1.Cannons;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.Players;
using MHGameWork.TheWizards.RTSTestCase1._Tests;
using NSubstitute;
using NUnit.Framework;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.RTSTestCase1.Tests.Players
{
    [TestFixture]
    public class PlayerInteractionTest
    {
        private PlayerInteractionPart part;
        private UserPlayer player;
        private IUserTargeter targeter;
        private Cart cart;
        private Cart cart2;
        private UserPlayer player2;
        private DroppedThing item;

        [SetUp]
        public void Setup()
        {
            TestUtilities.SetupTWContext();

            part = new PlayerInteractionPart();
            part.WorldLocator = new SimpleWorldLocator();

            player = Substitute.For<UserPlayer>();
            player2 = Substitute.For<UserPlayer>();
            targeter = Substitute.For<IUserTargeter>();

            part.Player = player;
            part.Targeter = targeter;

            cart = Substitute.For<Cart>();
            cart.ItemStorage.Capacity = 5;
            cart2 = Substitute.For<Cart>();

            item = new DroppedThing();
            
        }

        [Test]
        public void TestTakeCart()
        {
            targeter.Targeted.Returns(cart);
            part.Interact();

            Assert.AreEqual(cart, player.CartHolder.AssignedCart);
        }

        [Test]
        public void TestTakeCartFromOther()
        {
            player2.CartHolder.TakeCart(cart);

            targeter.Targeted.Returns(cart);
            part.Interact();

            Assert.AreEqual(cart, player.CartHolder.AssignedCart);
        }

        [Test]
        public void TestDropCart()
        {
            targeter.Targeted.Returns(cart);
            player.CartHolder.AssignedCart = cart;

            part.Interact();

            Assert.AreEqual(null, player.CartHolder.AssignedCart);
        }

        [Test]
        public void TestDropCartWhenUsingOtherCart()
        {
            targeter.Targeted.Returns(cart2);
            player.CartHolder.AssignedCart = cart;

            part.Interact();

            Assert.AreEqual(null, player.CartHolder.AssignedCart);
        }

        [Test]
        public void TestPickupWhenCartNear()
        {
            targeter.Targeted.Returns(item);
            
            part.Interact();

            Assert.That(player.ItemStorage.IsEmpty);
            Assert.That(cart.ItemStorage.Items.Contains(item));
        }

        [Test]
        public void TestPickupWhenNoCartNear()
        {
            targeter.Targeted.Returns(item);
            cart.Physical.SetPosition(new Vector3(10,0,0));

            part.Interact();

            Assert.That(!player.ItemStorage.IsEmpty);
            Assert.That(!cart.ItemStorage.Items.Contains(item));
        }

        [Test]
        public void TestTakeFromCart()
        {
            targeter.Targeted.Returns(item);
            item.Item.PutInStorage(cart);

            part.Interact();

            Assert.That(!player.ItemStorage.IsEmpty);
            Assert.That(!cart.ItemStorage.Items.Contains(item));
        }

        [Test]
        public void TestBuildCannon()
        {
            targeter.Targeted.Returns(Substitute.For<IPhysical>());
            targeter.TargetPoint.Returns(new Vector3(1, 0, 1));

            part.BuildCannon();

            Assert.That(TW.Data.Objects.OfType<Cannon>().Any(c => c.Position == new Vector3(1, 0, 1)));
            

        }
    }
}