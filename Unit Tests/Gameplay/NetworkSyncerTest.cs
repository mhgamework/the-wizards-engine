using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.Simulation;
using MHGameWork.TheWizards.Simulation.Synchronization;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Gameplay
{
    [TestFixture]
    public class NetworkSyncerTest
    {
        private BasicPacketTransporter<ChangePacket> clientA;
        private BasicPacketTransporter<ChangePacket> clientB;

        [SetUp]
        public void SetUp()
        {
            clientA = new BasicPacketTransporter<ChangePacket>(
                delegate(ChangePacket p)
                {
                    clientB.QueueReceivedPacket(p);
                    Thread.Sleep(500);// Cheat to make the packet arrive immediately
                });
            clientB = new BasicPacketTransporter<ChangePacket>(

                delegate(ChangePacket p)
                {
                    clientA.QueueReceivedPacket(p);
                    Thread.Sleep(500);// Cheat to make the packet arrive immediately
                });
        }

        [Test]
        public void TestSingleObjectSync()
        {
            var syncerA = new NetworkSyncerSimulator(createTransporterA());
            var syncerB = new NetworkSyncerSimulator(createTransporterB());

            var containerA = new ModelContainer.ModelContainer();
            var containerB = new ModelContainer.ModelContainer();

            // Enter scope A, step 1
            TW.Model = containerA;
            new BasicModelObject { Number = 42, Text = "Goblin" };
            syncerA.Simulate();

            // Enter scope B, step 1
            TW.Model = containerB;
            syncerB.Simulate();

            // step 1 verification:
            Assert.True(((BasicModelObject)containerA.Objects[0]).FieldsEqual(containerB.Objects[0]));

            // step 1 clear
            containerA.ClearDirty();
            containerB.ClearDirty();




            // Enter scope A, step 2
            TW.Model = containerA;
            syncerA.Simulate();

            // Enter scope B, step 2
            TW.Model = containerB;
            ((BasicModelObject)TW.Model.Objects[0]).Text = "Sheep";
            syncerB.Simulate();

            // step 2 verification:
            Assert.False(((BasicModelObject)containerA.Objects[0]).FieldsEqual(containerB.Objects[0]));

            // step 2 clear
            containerA.ClearDirty();
            containerB.ClearDirty();




            // Enter scope A, step 3
            TW.Model = containerA;
            syncerA.Simulate();

            // Enter scope B, step 3
            TW.Model = containerB;
            syncerB.Simulate();

            // step 3 verification:
            Assert.True(((BasicModelObject)containerA.Objects[0]).FieldsEqual(containerB.Objects[0]));

            // step 3 clear
            containerA.ClearDirty();
            containerB.ClearDirty();




            // Enter scope A, step 4
            TW.Model = containerA;
            TW.Model.RemoveObject(TW.Model.Objects[0]);
            syncerA.Simulate();

            // Enter scope B, step 4
            TW.Model = containerB;
            syncerB.Simulate();

            // step 4 verification:
            Assert.AreEqual(0, containerA.Objects.Count);
            Assert.AreEqual(0, containerB.Objects.Count);

            // step 4 clear
            containerA.ClearDirty();
            containerB.ClearDirty();


        }



        private IServerPacketTransporter<ChangePacket> createTransporterA()
        {
            var ret = new ServerPacketTransporterNetworked<ChangePacket>();
            ret.AddClientTransporter(new DummyClient("Client A"), clientA);
            return ret;
        }
        private IServerPacketTransporter<ChangePacket> createTransporterB()
        {
            var ret = new ServerPacketTransporterNetworked<ChangePacket>();
            ret.AddClientTransporter(new DummyClient("Client B"), clientB);
            return ret;
        }

        [ModelObjectChanged]
        private class BasicModelObject : BaseModelObject
        {
            public BasicModelObject()
            {

            }

            public int Number { get; set; }
            public string Text { get; set; }
            public string EmptyProperty { get; set; }

            public bool FieldsEqual(object o)
            {
                var other = (BasicModelObject)o;
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.Number == Number && Equals(other.Text, Text);
            }

        }




    }
}
