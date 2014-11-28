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
            var world = new Internal.Model.World(10, 10, (w, p) => new GameVoxel(w, p, new ProxyGenerator()));
            var store = new GenericDatastore(world);

            var list = store.RootRecord.GetList("Towns", r => new Town(null, r));

            var town = new Town(null, store.RootRecord.CreateRecord());
            list.Add(town);

            town.TownVoxels.Add(world.GetVoxel(new Point2(3, 8)));
            town.TownVoxels.Add(world.GetVoxel(new Point2(4, 1)));

            var doc = store.Serialize();
            return doc;
        }

        [Test]
        public void TestDeserialize()
        {
            var el = serialize();


            var world = new Internal.Model.World(10, 10, (w, p) => new GameVoxel(w, p, new ProxyGenerator()));
            var store = new GenericDatastore(world);


            store.Deserialize(el);

            var list = store.RootRecord.GetList("Towns", r => new Town(null, r));

            list.Print();


        }
    }
}