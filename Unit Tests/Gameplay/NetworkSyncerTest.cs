using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            clientA = new BasicPacketTransporter<ChangePacket>(p => clientB.QueueReceivedPacket(p));
            clientB = new BasicPacketTransporter<ChangePacket>(p => clientA.QueueReceivedPacket(p));
        }

        [Test]
        public void TestOneWaySyncSimple()
        {
            var syncerA = new NetworkSyncerSimulator(createTransporterA());
            var syncerB = new NetworkSyncerSimulator(createTransporterB());

            var containerA = new ModelContainer.ModelContainer();
            var containerB = new ModelContainer.ModelContainer();

            // Enter scope A, step 1
            TW.Model = containerA;
            containerA.AddObject(new BasicModelObject { Number = 42, Text = "Goblin" });
            syncerA.Simulate();

            // Enter scope B, step 1
            TW.Model = containerB;
            syncerB.Simulate();


            // step 1 verification:
            containerA.Objects[0].Equals(containerB.Objects[0]);


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

        private class BasicModelObject : BaseModelObject
        {
            public int Number;
            public string Text;

            public bool Equals(BasicModelObject other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.Number == Number && Equals(other.Text, Text);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(BasicModelObject)) return false;
                return Equals((BasicModelObject)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Number * 397) ^ (Text != null ? Text.GetHashCode() : 0);
                }
            }
        }




    }
}
