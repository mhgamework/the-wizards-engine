using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Client;
using MHGameWork.TheWizards.Networking.Packets;
using MHGameWork.TheWizards.World.Packets;

namespace MHGameWork.TheWizards.World
{
    public class ClientSyncer
    {
        private IClientPacketTransporter<DataPacket> deltaTransport;

        private Queue<byte[]> deltaPackets = new Queue<byte[]>();

        private List<ClientSyncedActor> actors = new List<ClientSyncedActor>();
        private IClientPacketTransporter<TimeUpdatePacket> timeTransport;

        private DeltaSnapshotParser deltaParser = new DeltaSnapshotParser();

        private volatile int tickNumber;
        private volatile float totalTime;
        private float timeSinceTick;

        public ClientSyncer(IClientPacketManager packetManager)
        {
            //TODO: cleanup
            var gen = new NetworkPacketFactoryCodeGenerater(TWDir.Cache.FullName + "\\ClientSyncerGenTemp" + (new Random()).Next(0, 10000) + ".dll");
            var timeFactory = gen.GetFactory<TimeUpdatePacket>();
            gen.BuildFactoriesAssembly();

            deltaTransport = packetManager.CreatePacketTransporter("WorldSyncerDeltaSnapshot", new DataPacket.Factory(),
                                                                   PacketFlags.UDP);

            timeTransport = packetManager.CreatePacketTransporter("WorldSyncerTimeUpdate", timeFactory,
                                                                  PacketFlags.UDP);

            var t = new Thread(receiveDeltaJob) { Name = "ClientSyncerDeltaJob", IsBackground = true };
            t.Start();

            t = new Thread(receiveTimeJob) { Name = "ClientSyncerTimeJob", IsBackground = true };
            t.Start();
        }

        public void Update(float elapsed)
        {
            totalTime += elapsed;
            timeSinceTick += elapsed;
            while (timeSinceTick > ServerSyncer.tickRate)
            {
                timeSinceTick -= ServerSyncer.tickRate;
                //Do a tick
                tickNumber++;
            }

            int totalMiliseconds = (int)(totalTime * 1000);

            for (int i = 0; i < actors.Count; i++)
            {
                var actor = actors[i];
                actor.Process(totalMiliseconds, ServerSyncer.tickRate);
            }

            lock (this)
                while (deltaPackets.Count > 0)
                    processDeltaSnapshotPacket(deltaPackets.Dequeue());


        }

        /// <summary>
        /// WARNING: this is boxing dangerous
        /// </summary>
        /// <param name="p"></param>
        private void processDeltaSnapshotPacket(byte[] data)
        {
            int tick;
            deltaParser.StartRead(data, out tick);

            UpdateEntityPacket p;
            ushort id;

            while (!(p = deltaParser.ReadEntityUpdate(out id)).IsEmpty())
            {
                var a = findEntityByID(id);
                if (a == null)
                {
                    Console.WriteLine(@"ClientSyncerActor not found with ID: " + id);
                    continue;
                }
                a.AddEntityUpdate(tick, p);
            }
        }

        /// <summary>
        /// TODO: optimze
        /// </summary>
        /// <returns></returns>
        private ClientSyncedActor findEntityByID(ushort id)
        {
            if (id == 0)
                throw new InvalidOperationException(
                    "Not allowed to sync entity(ies) with id 0, since this is an invalid id");

            return actors.Find(a => a.ID == id);

        }


        private void receiveTimeJob()
        {
            // This is an old method. You can have this functionality directly now with the transporters.
            for (; ; )
            {
                var p = timeTransport.Receive();
                //Not sure this is thread safe but probably ok
                totalTime = p.TotalTime;
                tickNumber = p.TickNumber;
            }

        }
        private void receiveDeltaJob()
        {
            // This is an old method. You can have this functionality directly now with the transporters.
            for (; ; )
            {
                var p = deltaTransport.Receive();
                lock (deltaPackets)
                    deltaPackets.Enqueue(p.Data);
            }

        }

        public ClientSyncedActor CreateActor(StillDesign.PhysX.Actor actor)
        {
            return CreateActor(new WorldPhysxSyncActor(actor));
        }
        public ClientSyncedActor CreateActor(IWorldSyncActor actor)
        {
            var a = new ClientSyncedActor();
            a.Actor = actor;
            actors.Add(a);
            return a;
        }
    }
}
