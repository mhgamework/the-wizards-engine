using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Networking.Packets;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.World.Packets;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.World
{
    public class ServerSyncer
    {
        private float timeSinceTick = 0;
        private float totalTime = 0;

        internal static float tickRate = 1 / 30f;

        private int tickNumber;

        private List<ServerSyncedActor> actors = new List<ServerSyncedActor>();

        private DeltaSnapshotParser deltaParser = new DeltaSnapshotParser();
        private IServerPacketTransporter<DataPacket> deltaSnapshotTransporter;
        private IServerPacketTransporter<TimeUpdatePacket> timeUpdateTransporter;

        private ushort nextUniqueID = 2;

        public ServerSyncer(IServerPacketManager packetManager)
        {
            //TODO: cleanup
            var gen = new NetworkPacketFactoryCodeGenerater(TWDir.Cache.FullName + "\\ServerSyncerGenTemp.dll");
            var timeFactory = gen.GetFactory<TimeUpdatePacket>();
            gen.BuildFactoriesAssembly();

            deltaSnapshotTransporter = packetManager.CreatePacketTransporter("WorldSyncerDeltaSnapshot",
                                                                             new DataPacket.Factory(),
                                                                             Networking.PacketFlags.UDP);
            timeUpdateTransporter = packetManager.CreatePacketTransporter("WorldSyncerTimeUpdate",
                                                                          timeFactory,
                                                                          Networking.PacketFlags.UDP);
        }

        private void tick()
        {
            deltaParser.StartWrite(tickNumber);

            for (int i = 0; i < actors.Count; i++)
            {
                var actor = actors[i];
                actor.Tick();
                var p = new UpdateEntityPacket();
                p.Positie = actor.Positie;
                p.RotatieQuat = actor.RotatieQuat;

                deltaParser.WriteEntityUpdatePacket(actor.ID, p);

            }

            var packet = new DataPacket();
            packet.Data = deltaParser.EndWrite();
            if (packet.Data == null) throw new InvalidOperationException("Cant be null??");
            deltaSnapshotTransporter.SendAll(packet);

            // Currently send time update every tick
            timeUpdateTransporter.SendAll(new TimeUpdatePacket { TickNumber = tickNumber, TotalTime = totalTime });

        }

        public void Update(float elapsed)
        {

            totalTime += elapsed;
            timeSinceTick += elapsed;

            while (timeSinceTick > tickRate)
            {
                timeSinceTick -= tickRate;
                //Do a tick
                tickNumber++;
                tick();
            }

        }

        public ServerSyncedActor CreateActor(Actor actor)
        {
            return CreateActor(new WorldPhysxSyncActor(actor));
        }

        public ServerSyncedActor CreateActor(IWorldSyncActor worldSyncActor)
        {
            var a = new ServerSyncedActor();
            a.Actor = worldSyncActor;
            a.ID = nextUniqueID;
            nextUniqueID++;

            actors.Add(a);

            return a;
        }
    }
}
