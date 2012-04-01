using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Linq;
using MHGameWork.TheWizards.Common.Networking;
using MHGameWork.TheWizards.Networking;
using NUnit.Framework;
using TCPConnection = MHGameWork.TheWizards.Networking.TCPConnection;
using TCPConnectionListener = MHGameWork.TheWizards.Networking.TCPConnectionListener;
using TCPPacketBuilder = MHGameWork.TheWizards.Networking.TCPPacketBuilder;

namespace MHGameWork.TheWizards.Tests.Networking
{
    [TestFixture]
    public class NetworkingTest
    {
        [Test]
        public void TestTCPConnect()
        {
            bool success = false;
            AutoResetEvent testDone = new AutoResetEvent(false);

            TCPConnectionListener listener = new TCPConnectionListener(5013);

            listener.Listening = true;
            TCPConnection client1 = null;

            listener.ClientConnected +=
                delegate(object sender, TCPConnectionListener.ClientConnectedEventArgs e)
                {
                    Console.WriteLine("Client connected to server!");
                    TCPConnection serverClient1 = new TCPConnection(e.CL);
                    serverClient1.SendPacket(new byte[] { 1, 2, 3, 4 }, TCPPacketBuilder.TCPPacketFlags.None);
                    serverClient1.Receiving = true;


                    serverClient1.PacketRecievedAsync +=
                        delegate(object sender2, BaseConnection.PacketRecievedEventArgs e2)
                        {
                            for (int i = 1; i < 4 + 1; i++)
                            {
                                if (e2.Dgram[i - 1] != i)
                                {
                                    success = false; // optional
                                    testDone.Set();
                                    return;
                                }
                            }
                            Console.WriteLine("Packet received from client!");

                            success = true;
                            testDone.Set();
                        };

                };


            client1 = new TCPConnection();

            client1.ConnectedToServer +=
                delegate(object sender, TCPConnection.ConnectedToServerEventArgs e)
                {
                    Console.WriteLine("Connected successfully!");
                    client1.Receiving = true;


                };

            client1.ConnectError +=
                delegate(object sender, TCPConnection.ConnectErrorEventArgs e)
                {
                    throw e.Ex;
                };

            client1.PacketRecievedAsync +=
                delegate(object sender, BaseConnection.PacketRecievedEventArgs e)
                {
                    for (int i = 1; i < 4 + 1; i++)
                    {
                        if (e.Dgram[i - 1] != i)
                        {
                            success = false; // optional
                            testDone.Set();
                            return;
                        }
                    }
                    Console.WriteLine("Packet received from server!");
                    client1.SendPacket(new byte[] { 1, 2, 3, 4 }, TCPPacketBuilder.TCPPacketFlags.None);

                };




            client1.Connect(System.Net.IPAddress.Parse("127.0.0.1"), 5013);

            //testDone.WaitOne(5000);
            testDone.WaitOne();

            Assert.True(success);


        }
        [Test]
        public void TestGenerateFactoryEmpty()
        {
            NetworkPacketFactoryCodeGenerater gen = new NetworkPacketFactoryCodeGenerater(
                System.Windows.Forms.Application.StartupPath + "\\Test\\Networking\\TestFactoriesEmpty.dll");

            gen.BuildFactoriesAssembly();
        }
        [Test]
        public void TestGenerateFactory()
        {
            NetworkPacketFactoryCodeGenerater gen = new NetworkPacketFactoryCodeGenerater(
                System.Windows.Forms.Application.StartupPath + "\\Test\\Networking\\TestFactoriesGenerated.dll");

            INetworkPacketFactory<DataPacket> factory = gen.GetFactory<DataPacket>();


            gen.BuildFactoriesAssembly();
        }
        [Test]
        public void TestGenerateFactoryFields()
        {
            NetworkPacketFactoryCodeGenerater gen = new NetworkPacketFactoryCodeGenerater(
                System.Windows.Forms.Application.StartupPath + "\\Test\\Networking\\TestFactoriesGeneratedFields.dll");

            INetworkPacketFactory<PacketFieldsTest> factory = gen.GetFactory<PacketFieldsTest>();

            gen.BuildFactoriesAssembly();

            PacketFieldsTest p = new PacketFieldsTest();

            p.Text = "Hello";
            p.Getal = 38;
            p.Boolean = true;
            p.GetalFloat = 456f;
            p.Buffer = new byte[] { 1, 1, 2, 3, 5, 7, 13, 21 };
            p.Guid = Guid.NewGuid();
            p.Enum = ByteEnum.Second;
            p.Array = new int[] { 1, 3, 6, 10, 15, 21 };

            MemoryStream memStrm = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(memStrm);
            BinaryReader br = new BinaryReader(memStrm);
            factory.ToStream(bw, p);

            memStrm.Position = 0;

            PacketFieldsTest pCheck = factory.FromStream(br);
            Assert.AreEqual(pCheck, p);

        }
        [Test]
        public void TestGenerateFactoryProperties()
        {
            NetworkPacketFactoryCodeGenerater gen = new NetworkPacketFactoryCodeGenerater(
                 System.Windows.Forms.Application.StartupPath + "\\Test\\Networking\\TestFactoriesGeneratedProperties.dll");

            INetworkPacketFactory<PacketPropertiesTest> factory = gen.GetFactory<PacketPropertiesTest>();

            gen.BuildFactoriesAssembly();

            PacketPropertiesTest p = new PacketPropertiesTest();

            p.Text = "Hello";
            p.Getal = 38;

            MemoryStream memStrm = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(memStrm);
            BinaryReader br = new BinaryReader(memStrm);
            factory.ToStream(bw, p);

            memStrm.Position = 0;

            PacketPropertiesTest pCheck = factory.FromStream(br);
            Assert.AreEqual(pCheck, p);
        }
        [Test]
        public void TestGenerateFactoryIgnoreAttribute()
        {
            throw new NotImplementedException();

        }

        [Test]
        public void TestGenerateMultiple()
        {
            NetworkPacketFactoryCodeGenerater gen = new NetworkPacketFactoryCodeGenerater(
             System.Windows.Forms.Application.StartupPath + "\\Test\\Networking\\TestGenerateMultiple.dll");

            INetworkPacketFactory<PacketFieldsTest> factory1 = gen.GetFactory<PacketFieldsTest>();
            INetworkPacketFactory<PacketPropertiesTest> factory2 = gen.GetFactory<PacketPropertiesTest>();

            gen.BuildFactoriesAssembly();

            PacketFieldsTest p1 = new PacketFieldsTest();

            p1.Text = "Hello";
            p1.Getal = 38;

            MemoryStream memStrm; BinaryWriter bw; BinaryReader br;
            memStrm = new MemoryStream();
            bw = new BinaryWriter(memStrm);
            br = new BinaryReader(memStrm);
            factory1.ToStream(bw, p1);

            memStrm.Position = 0;

            PacketFieldsTest pCheck1 = factory1.FromStream(br);
            Assert.AreEqual(pCheck1, p1);


            PacketPropertiesTest p2 = new PacketPropertiesTest();

            p2.Text = "Hello";
            p2.Getal = 38;
            memStrm = new MemoryStream();
            bw = new BinaryWriter(memStrm);
            br = new BinaryReader(memStrm);
            factory2.ToStream(bw, p2);

            memStrm.Position = 0;

            PacketPropertiesTest pCheck2 = factory2.FromStream(br);
            Assert.AreEqual(pCheck2, p2);
        }

        [Test]
        public void TestLoadFromCache()
        {
            throw new NotImplementedException();
        }

        [NetworkPacket]
        public class PacketFieldsTest : INetworkPacket
        {
            public int Getal;
            public string Text;
            public bool Boolean;
            public float GetalFloat;
            public byte[] Buffer;
            public Guid Guid;
            public ByteEnum Enum;
            public int[] Array;





            /// <summary>
            /// Small test
            /// </summary>
            private DateTime date;

            public bool Equals(PacketFieldsTest other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;



                return other.Getal == Getal
                    && Equals(other.Text, Text)
                    && other.Boolean.Equals(Boolean)
                    && other.GetalFloat.Equals(GetalFloat)
                    && ((Buffer == other.Buffer) || Buffer.SequenceEqual(other.Buffer))
                    && Equals(other.Enum, Enum)
                    && ((Array == other.Array) || other.Array.SequenceEqual(Array))
                    && Guid.Equals(other.Guid);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(PacketFieldsTest)) return false;
                return Equals((PacketFieldsTest)obj);
            }

        }

        [NetworkPacket]
        public class PacketPropertiesTest : INetworkPacket
        {
            private int getal;
            private string text;
            private DateTime date;
            private int privateGetal = 5;

            public int PrivateGetal
            {
                [DebuggerStepThrough]
                get { return privateGetal; }
            }

            public int Getal
            {
                [DebuggerStepThrough]
                get { return getal; }
                [DebuggerStepThrough]
                set { getal = value; }
            }

            public string Text
            {
                [DebuggerStepThrough]
                get { return text; }
                [DebuggerStepThrough]
                set { text = value; }
            }

            private DateTime Date
            {
                [DebuggerStepThrough]
                get { return date; }
                [DebuggerStepThrough]
                set { date = value; }
            }



            public bool Equals(PacketPropertiesTest other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return other.getal == getal && Equals(other.text, text);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof(PacketPropertiesTest)) return false;
                return Equals((PacketPropertiesTest)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (getal * 397) ^ (text != null ? text.GetHashCode() : 0);
                }
            }
        }


        //TEMP

        /// <summary>
        /// Used for creating the autogen code
        /// </summary>
        public class TempFactoryFactory : INetworkPacketFactoryFactory
        {
            private Dictionary<Type, INetworkPacketFactory> factoriesMap;

            public TempFactoryFactory()
            {
                factoriesMap = new Dictionary<Type, INetworkPacketFactory>();

                factoriesMap.Add(typeof(DataPacket), new DataPacketFactory());
            }

            public INetworkPacketFactory<T> GetFactory<T>() where T : INetworkPacket
            {
                INetworkPacketFactory ret;
                if (!factoriesMap.TryGetValue(typeof(T), out ret))
                {
                    return null;
                }

                return ret as INetworkPacketFactory<T>;
            }
        }

        public enum ByteEnum : byte
        {
            None,
            First,
            Second,
            Third
        }
    }
}
