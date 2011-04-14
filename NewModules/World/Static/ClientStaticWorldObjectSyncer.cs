using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Networking.Client;

namespace MHGameWork.TheWizards.World.Static
{
    public class ClientStaticWorldObjectSyncer
    {
        private readonly IStaticWorldObjectFactory worldObjectFactory;
        private List<IStaticWorldObject> objects = new List<IStaticWorldObject>();
        private IClientPacketTransporter<StaticWorldObjectUpdatePacket> updateTransporter;
        private IClientPacketTransporter<StaticWorldObjectDeletePacket> deleteTransporter;

        public ClientStaticWorldObjectSyncer(IClientPacketManager pm, IStaticWorldObjectFactory worldObjectFactory)
        {
            this.worldObjectFactory = worldObjectFactory;
            var gen =
                new Networking.NetworkPacketFactoryCodeGenerater(TWDir.Cache + "\\ClientStaticWorldObjectSyncer" +
                                                                 (new Random()).Next(0, 100000) + ".dll");

            updateTransporter = pm.CreatePacketTransporter("StaticWorldObjectSyncer_UpdatePacket", gen.GetFactory<StaticWorldObjectUpdatePacket>(), Networking.PacketFlags.TCP);
            deleteTransporter = pm.CreatePacketTransporter("StaticWorldObjectSyncer_DeletePacket", gen.GetFactory<StaticWorldObjectDeletePacket>(), Networking.PacketFlags.TCP);

            gen.BuildFactoriesAssembly();
        }

        public List<IStaticWorldObject> Objects
        {
            get { return objects; }
        }


        public void Update(float elapsed)
        {
            while (updateTransporter.PacketAvailable)
            {
                var p = updateTransporter.Receive();


                var obj = findObject(p.ID);
                if (obj == null)
                    obj = createObject(p.ID);

                worldObjectFactory.ApplyUpdatePacket(obj, p);

            }
            while (deleteTransporter.PacketAvailable)
            {
                var p = updateTransporter.Receive();
                removeObject(p.ID);
            }

        }

        private IStaticWorldObject findObject(int id)
        {
            return Objects.Find(o => o.ID == id);
        }

        private IStaticWorldObject createObject(int id)
        {
            var obj = worldObjectFactory.CreateNew();

            obj.ID = id;
            objects.Add(obj);
            return obj;
        }

        private bool removeObject(int id)
        {
            var obj = findObject(id);
            if (obj == null) return false;

            worldObjectFactory.Delete(obj);

            return Objects.Remove(obj);
        }
    }
}
