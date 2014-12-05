using System.Xml.Linq;
using Castle.DynamicProxy;
using DirectX11;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using NUnit.Framework;

namespace MHGameWork.TheWizards.GodGame.Types.Towns.Data
{
    [TestFixture]
    public class GenericDatastoreTest
    {
        [Test]
        public void TestSerialize()
        {
            var doc = serialize();
            doc.ToString().Print();
        }

        private static XElement serialize()
        {
            var world = TestWorldBuilder.createTestWorld(10, 10);
            var store = new GenericDatastore(world);

            var list = store.RootRecord.GetList("Towns", r => new Town(null, r));

            var town = new Town(createService(), store.RootRecord.CreateRecord());
            town.SetTownCenter(world.GetVoxel(new Point2(3, 8)));
            list.Add(town);

            town.AddVoxel(world.GetVoxel(new Point2(3, 7)));

            var doc = store.Serialize();
            return doc;
        }

        private static TownCenterService createService()
        {
            return new TownCenterService(null, new GenericDatastoreRecord(new GenericDatastore(null), 15));
        }

        [Test]
        public void TestDeserialize()
        {
            var el = serialize();


            var world = TestWorldBuilder.createTestWorld(10, 10); 
            var store = new GenericDatastore(world);


            store.Deserialize(el);

            var list = store.RootRecord.GetList("Towns", r => new Town(null, r));

            list.Print();
            list[0].TownVoxels.Print();
            list[0].TownCenter.ToString().Print();


        }
    }
}