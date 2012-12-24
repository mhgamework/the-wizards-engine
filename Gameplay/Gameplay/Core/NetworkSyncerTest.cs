using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Gameplay.Core
{
    /// <summary>
    /// TODO: reimplement
    /// </summary>
    [TestFixture]
    public class NetworkSyncerTest
    {
        //private BasicPacketTransporter<ChangePacket> clientA;
        //private BasicPacketTransporter<ChangePacket> clientB;

        //[SetUp]
        //public void SetUp()
        //{
        //    clientA = new BasicPacketTransporter<ChangePacket>(
        //        delegate(ChangePacket p)
        //        {
        //            clientB.QueueReceivedPacket(p);
        //            Thread.Sleep(500);// Cheat to make the packet arrive immediately
        //        });
        //    clientB = new BasicPacketTransporter<ChangePacket>(

        //        delegate(ChangePacket p)
        //        {
        //            clientA.QueueReceivedPacket(p);
        //            Thread.Sleep(500);// Cheat to make the packet arrive immediately
        //        });
        //}

        //[Test]
        //public void TestSingleObjectSync()
        //{
        //    var syncerA = new NetworkSyncerSimulator(createTransporterA());
        //    var syncerB = new NetworkSyncerSimulator(createTransporterB());

        //    var contextA = new TW.Context { Data = new ModelContainer.ModelContainer() };
        //    var contextB = new TW.Context { Data = new ModelContainer.ModelContainer() };
        //    var containerA = contextA.Data;
        //    var containerB = contextB.Data;


        //    // Step 0 is to allow sending dummy packets 2-way
        //    // Enter scope A, step 0
        //    TW.SetContext(contextA);
        //    syncerA.Simulate();

        //    // Enter scope B, step 0
        //    TW.SetContext(contextB);
        //    syncerB.Simulate();

        //    // step 0 verification:
        //    Assert.AreEqual(containerA.Objects.Count, containerB.Objects.Count);

        //    // step 0 clear
        //    containerA.ClearDirty();
        //    containerB.ClearDirty();


        //    // Enter scope A, step 1
        //    TW.SetContext(contextA);
        //    new BasicModelObject { Number = 42, Text = "Goblin" };
        //    syncerA.Simulate();

        //    // Enter scope B, step 1
        //    TW.SetContext(contextB);
        //    syncerB.Simulate();

        //    // step 1 verification:
        //    Assert.AreEqual(containerA.Objects.Count, containerB.Objects.Count);
        //    Assert.True(((BasicModelObject)containerA.Objects[0]).FieldsEqual(containerB.Objects[0]));

        //    // step 1 clear
        //    containerA.ClearDirty();
        //    containerB.ClearDirty();




        //    // Enter scope A, step 2
        //    TW.SetContext(contextA);
        //    syncerA.Simulate();

        //    // Enter scope B, step 2
        //    TW.SetContext(contextB);
        //    ((BasicModelObject)TW.Model.Objects[0]).Text = "Sheep";
        //    syncerB.Simulate();

        //    // step 2 verification:
        //    Assert.False(((BasicModelObject)containerA.Objects[0]).FieldsEqual(containerB.Objects[0]));

        //    // step 2 clear
        //    containerA.ClearDirty();
        //    containerB.ClearDirty();




        //    // Enter scope A, step 3
        //    TW.SetContext(contextA);
        //    syncerA.Simulate();

        //    // Enter scope B, step 3
        //    TW.SetContext(contextB);
        //    syncerB.Simulate();

        //    // step 3 verification:
        //    Assert.True(((BasicModelObject)containerA.Objects[0]).FieldsEqual(containerB.Objects[0]));

        //    // step 3 clear
        //    containerA.ClearDirty();
        //    containerB.ClearDirty();




        //    // Enter scope A, step 4
        //    TW.SetContext(contextA);
        //    TW.Model.RemoveObject(TW.Model.Objects[0]);
        //    syncerA.Simulate();

        //    // Enter scope B, step 4
        //    TW.SetContext(contextB);
        //    syncerB.Simulate();

        //    // step 4 verification:
        //    Assert.AreEqual(0, containerA.Objects.Count);
        //    Assert.AreEqual(0, containerB.Objects.Count);

        //    // step 4 clear
        //    containerA.ClearDirty();
        //    containerB.ClearDirty();


        //}

        ///// <summary>
        ///// Note that, for this to work, the new clients have to send Something, otherwise nothing happens
        ///// </summary>
        //[Test]
        //public void TestLateConnectSendExistingStateOneWay()
        //{
        //    var transporterA = new ServerPacketTransporterNetworked<ChangePacket>();
        //    var transporterB = new ServerPacketTransporterNetworked<ChangePacket>();

        //    var syncerA = new NetworkSyncerSimulator(transporterA);
        //    var syncerB = new NetworkSyncerSimulator(transporterB);

        //    var contextA = new TW.Context { Data = new ModelContainer.ModelContainer() };
        //    var contextB = new TW.Context { Data = new ModelContainer.ModelContainer() };
        //    var containerA = contextA.Data;
        //    var containerB = contextB.Data;

        //    // Enter scope A, step 1
        //    TW.SetContext(contextA);
        //    new BasicModelObject { Number = 42, Text = "Goblin" };
        //    syncerA.Simulate();

        //    // Enter scope B, step 1
        //    // Skip! not started yet

        //    // step 1 verification:

        //    // step 1 clear
        //    containerA.ClearDirty();




        //    // Enter scope A, step 2
        //    TW.SetContext(contextA);
        //    syncerA.Simulate();

        //    // Enter scope B, step 2
        //    // connect!
        //    transporterA.AddClientTransporter(new DummyClient("ClientB"), clientB);
        //    transporterB.AddClientTransporter(new DummyClient("ClientA"), clientA);
        //    TW.SetContext(contextB);
        //    new BasicModelObject { Number = 2, Text = "Sheep" };
        //    syncerB.Simulate(); // Should send dummy packet

        //    // step 2 verification:
        //    Assert.AreNotEqual(containerA.Objects[0], containerB.Objects[0]);

        //    // step 2 clear
        //    containerA.ClearDirty();
        //    containerB.ClearDirty();




        //    // Enter scope A, step 3
        //    TW.SetContext(contextA);
        //    syncerA.Simulate(); // Dummy packet should force a world update to container b

        //    // Enter scope B, step 3
        //    TW.SetContext(contextB);
        //    syncerB.Simulate(); // world update from a should force world update to a from b

        //    // step 3 verification:
        //    Assert.AreNotEqual(containerA.Objects.Count, containerB.Objects.Count);

        //    // step 3 clear
        //    containerA.ClearDirty();
        //    containerB.ClearDirty();



        //    // Enter scope A, step 4
        //    TW.SetContext(contextA);
        //    syncerA.Simulate();

        //    // Enter scope B, step 4
        //    TW.SetContext(contextB);
        //    syncerB.Simulate();

        //    // step 4 verification:
        //    Assert.AreEqual(containerA.Objects.Count, containerB.Objects.Count);
        //    Assert.AreEqual(containerA.Objects[0].ToString(), containerB.Objects[1].ToString());
        //    Assert.AreEqual(containerA.Objects[1].ToString(), containerB.Objects[0].ToString());

        //    // step 4 clear
        //    containerA.ClearDirty();
        //    containerB.ClearDirty();

        //}

        //[Test]
        //public void TestSyncEntity()
        //{
        //    var syncerA = new NetworkSyncerSimulator(createTransporterA());
        //    var syncerB = new NetworkSyncerSimulator(createTransporterB());
        //    var contextA = new TW.Context { Data = new ModelContainer.ModelContainer() };
        //    var contextB = new TW.Context { Data = new ModelContainer.ModelContainer() };
        //    var containerA = contextA.Data;
        //    var containerB = contextB.Data;

        //    // Step 0 is to allow sending dummy packets 2-way
        //    // Enter scope A, step 0
        //    TW.SetContext(contextA);
        //    syncerA.Simulate();

        //    // Enter scope B, step 0
        //    TW.SetContext(contextB);
        //    syncerB.Simulate();
        //    // step 0 verification:
        //    Assert.AreEqual(containerA.Objects.Count, containerB.Objects.Count);

        //    // step 0 clear
        //    containerA.ClearDirty();
        //    containerB.ClearDirty();


        //    // Enter scope A, step 1
        //    TW.SetContext(contextA);
        //    new WorldRendering.Entity { WorldMatrix = Matrix.Translation(2, 0, 3) };
        //    syncerA.Simulate();

        //    // Enter scope B, step 1
        //    TW.SetContext(contextB);
        //    syncerB.Simulate();

        //    // step 1 verification:
        //    Assert.AreEqual(containerA.Objects.Count, containerB.Objects.Count);
        //    Assert.AreEqual(containerA.Objects[0].ToString(), containerB.Objects[0].ToString());

        //    // step 1 clear
        //    containerA.ClearDirty();
        //    containerB.ClearDirty();


        //}

        //[Test]
        //public void TestSyncModelobjectRelation()
        //{
        //    var syncerA = new NetworkSyncerSimulator(createTransporterA());
        //    var syncerB = new NetworkSyncerSimulator(createTransporterB());

        //    var contextA = new TW.Context { Data = new ModelContainer.ModelContainer() };
        //    var contextB = new TW.Context { Data = new ModelContainer.ModelContainer() };
        //    var containerA = contextA.Data;
        //    var containerB = contextB.Data;


        //    // Step 0 is to allow sending dummy packets 2-way
        //    // Enter scope A, step 0
        //    TW.SetContext(contextA);
        //    syncerA.Simulate();

        //    // Enter scope B, step 0
        //    TW.SetContext(contextB);
        //    syncerB.Simulate();
        //    // step 0 verification:
        //    Assert.AreEqual(containerA.Objects.Count, containerB.Objects.Count);

        //    // step 0 clear
        //    containerA.ClearDirty();
        //    containerB.ClearDirty();


        //    // Enter scope A, step 1
        //    TW.SetContext(contextA);
        //    var obj = new ReferenceModelObject();
        //    obj.Remote = new BasicModelObject { Number = 8 };
        //    syncerA.Simulate();

        //    // Enter scope B, step 1
        //    TW.SetContext(contextB);
        //    syncerB.Simulate();

        //    // step 1 verification:
        //    Assert.AreEqual(containerA.Objects.Count, containerB.Objects.Count);
        //    Assert.AreEqual(containerA.Objects[0].ToString(), containerB.Objects[1].ToString());  // Order will be inversed, because of the dependencies between the objects
        //    Assert.AreEqual(containerA.Objects[1].ToString(), containerB.Objects[0].ToString());

        //    // step 1 clear
        //    containerA.ClearDirty();
        //    containerB.ClearDirty();


        //}

        //[Test]
        //public void TestStringSerializerEnum()
        //{
        //    var s = StringSerializer.Create();
        //    var serialized = s.Serialize(FileMode.Create);
        //    var deserialized = s.Deserialize(serialized, typeof(FileMode));
        //    Assert.AreEqual(FileMode.Create, deserialized);
        //}

        //private IServerPacketTransporter<ChangePacket> createTransporterA()
        //{
        //    var ret = new ServerPacketTransporterNetworked<ChangePacket>();
        //    ret.AddClientTransporter(new DummyClient("Client A"), clientA);
        //    return ret;
        //}
        //private IServerPacketTransporter<ChangePacket> createTransporterB()
        //{
        //    var ret = new ServerPacketTransporterNetworked<ChangePacket>();
        //    ret.AddClientTransporter(new DummyClient("Client B"), clientB);
        //    return ret;
        //}

        //[ModelObjectChanged]
        //private class BasicModelObject : SimpleModelObject
        //{
        //    public BasicModelObject()
        //    {

        //    }

        //    public int Number { get; set; }
        //    public string Text { get; set; }
        //    public string EmptyProperty { get; set; }

        //    public bool FieldsEqual(object o)
        //    {
        //        var other = (BasicModelObject)o;
        //        if (ReferenceEquals(null, other)) return false;
        //        if (ReferenceEquals(this, other)) return true;
        //        return other.Number == Number && Equals(other.Text, Text);
        //    }

        //    public override string ToString()
        //    {
        //        return String.Format("Number: {0} Text: {1} EmptyProperty: {2}", Number, Text, EmptyProperty);
        //    }
        //}

        //private class ReferenceModelObject : SimpleModelObject
        //{
        //    public BasicModelObject Remote { get; set; }

        //    public override string ToString()
        //    {
        //        return string.Format("Remote: {0}", Remote);
        //    }
        //}


    }
}
