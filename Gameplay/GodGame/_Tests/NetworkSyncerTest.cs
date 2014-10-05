using System;
using MHGameWork.TheWizards.GodGame.Internal.Data;
using MHGameWork.TheWizards.GodGame.Internal.Networking;
using NUnit.Framework;

namespace MHGameWork.TheWizards.GodGame._Tests
{
    [TestFixture]
    public class NetworkSyncerTest
    {
        [Test]
        public void Test()
        {
            /*var syncerSource = createSyncer();
            var syncerDest = createSyncer();

            var packet = syncerSource.BuildDeltaPacket();

            syncerDest.ApplyDeltaPacket(packet);

            Assert(SourceEqualsDest);*/
        }

        private NetworkDeltaSyncer createSyncer()
        {
            throw new NotImplementedException();
        }
    }

    [TestFixture]
    public class DataStoreTest
    {
        private IDatastore datastore;
        private IObservableDatastore observableDatastore;
        private ISerializableDatastore serializableDatastore;

        private IDataIdentifier id1;
        private IDataIdentifier id2;
        private IDataIdentifier id3;
        [SetUp]
        public void Setup()
        {
            datastore = new StringObjectDatastore();
            observableDatastore = (StringObjectDatastore)datastore;
            serializableDatastore = (StringObjectDatastore)datastore;

            id1 = new StringIdentifier("Key1");
            id2 = new StringIdentifier("Key2");
            id3 = new StringIdentifier("Key3");
        }

        [Test]
        public void TestStore()
        {
            datastore.Store(id1, 5);
            datastore.Store(id2, "Hello");
            Assert.AreEqual(5, datastore.Get<int>(id1));
            Assert.AreEqual("Hello", datastore.Get<string>(id2));
        }

        //TODO: implement the ISerializableDatastore, IObservableDatastore
        // Create a generic ISerializer, which can serialize an object and deserializee
        //  It again in a polymorphic fashion (preserving type). Note that the TWEngine
        //  StringSerializer also supports this (i think)
        //  Next implement the networkdeltasyncer and test it usin gthe datastore
        //  Then link the interfacetodata interceptor to the datastore, which should
        //  enable generic network syncing and data persistence.

    }
}