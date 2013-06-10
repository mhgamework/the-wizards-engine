using MHGameWork.TheWizards.RTSTestCase1.Goblins;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Components;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using NSubstitute;
using NUnit.Framework;

namespace MHGameWork.TheWizards.RTSTestCase1.Tests.Items
{
    [TestFixture]
    public class ItemTest
    {
        private IItemStorage storage;
        private IItemStorage storage2;
        private ItemPart item;
        private IItem parent;

        [SetUp]
        public void Setup()
        {
            TestUtilities.SetupTWContext();

            storage = Substitute.For<StorageCrate>();
            storage2 = Substitute.For<StorageCrate>();


            storage.ItemStorage.Capacity = 1;
            storage2.ItemStorage.Capacity = 1;


            item = new ItemPart();
            parent = Substitute.For<IItem>();
            item.Parent = parent;

        }

        [Test]
        public void TestMoveInInventory()
        {
            item.PutInStorage(storage);

            Assert.That(storage.ItemStorage.Items, Contains.Item(parent));
        }

        [Test]
        public void TestMoveBetweenInventories()
        {
            item.PutInStorage(storage);

            item.PutInStorage(storage2);

            Assert.That(storage.ItemStorage.Items, !Contains.Item(parent));
            Assert.That(storage2.ItemStorage.Items, Contains.Item(parent));

        }
    }
}