using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Diagnostics.Tracing;
using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Players;
using NSubstitute;
using NUnit.Framework;

namespace MHGameWork.TheWizards.RTSTestCase1.Tests.Players
{
    [TestFixture]
    public class PlayerInteraction
    {
        private PlayerInteractionPart part;
        private UserPlayer player;
        private IUserTargeter targeter;
        private Cart cart;
        private Cart cart2;
        private UserPlayer player2;

        [SetUp]
        public void Setup()
        {
            TestUtilities.SetupTWContext();

            part = new PlayerInteractionPart();

            player = Substitute.For<UserPlayer>();
            player2 = Substitute.For<UserPlayer>();
            targeter = Substitute.For<IUserTargeter>();

            part.Player = player;
            part.Targeter = targeter;

            cart = Substitute.For<Cart>();
            cart2 = Substitute.For<Cart>();
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
    }
}