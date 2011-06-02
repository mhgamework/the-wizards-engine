using System.Collections.Generic;
using MHGameWork.TheWizards.Networking.Server;

namespace MHGameWork.TheWizards.World.Static
{
    public class ServerStaticWorldObjectSyncer
    {
        private List<ServerStaticWorldObject> objects = new List<ServerStaticWorldObject>();
        private IServerPacketTransporter<StaticWorldObjectUpdatePacket> updateTransporter;
        private IServerPacketTransporter<StaticWorldObjectDeletePacket> deleteTransporter;

        private int nextID = 2;

        public ServerStaticWorldObjectSyncer(IServerPacketManager pm)
        {
            var gen =
                new Networking.NetworkPacketFactoryCodeGenerater(TWDir.GenerateRandomCacheFile("", ".dll"));

            updateTransporter = pm.CreatePacketTransporter("StaticWorldObjectSyncer_UpdatePacket", gen.GetFactory<StaticWorldObjectUpdatePacket>(), Networking.PacketFlags.TCP);
            deleteTransporter = pm.CreatePacketTransporter("StaticWorldObjectSyncer_DeletePacket", gen.GetFactory<StaticWorldObjectDeletePacket>(), Networking.PacketFlags.TCP);

            gen.BuildFactoriesAssembly();
        }


        public ServerStaticWorldObject CreateNew()
        {
            var obj = new ServerStaticWorldObject();
            objects.Add(obj);

            obj.ID = nextID;
            nextID++;
            // We could already send an update here, but since there is no data in the object yet,
            //  it WILL be changed by the user of this method , so a 2nd update will occur, which is now the first update

            obj.Change = true; // Force update

            return obj;
        }

        public void DeleteObject(ServerStaticWorldObject obj)
        {
            if (!objects.Remove(obj)) return;

            var p = new StaticWorldObjectDeletePacket { ID = obj.ID };

            deleteTransporter.SendAll(p);
        }


        public void Update(float elapsed)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                var o = objects[i];
                if (o.Change)
                {
                    var p = new StaticWorldObjectUpdatePacket();
                    p.ID = o.ID;
                    p.MeshGuid = o.Mesh.Guid;
                    p.WorldMatrix = o.WorldMatrix.ToFloatArray();

                    updateTransporter.SendAll(p);

                    o.Change = false;

                }
            }
        }

    }
}
