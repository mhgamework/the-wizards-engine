using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Collections;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.Reflection;

namespace MHGameWork.TheWizards.Simulation.Synchronization
{
    /// <summary>
    /// This simulator is responsible for sending local changes to the remotes, and then accepting
    /// the changes from the remotes and applying it to the local model.
    /// 
    /// All changes from simulators that run after this simulator are NOT synced over network
    /// </summary>
    public class NetworkSyncerSimulator : ISimulator
    {
        private StringSerializer stringSerializer = StringSerializer.Create();

        private IServerPacketTransporter<ChangePacket> transporter;

        public NetworkSyncerSimulator(IServerPacketTransporter<ChangePacket> transporter)
        {
            this.transporter = transporter;
        }

        public void Simulate()
        {

            // Send local changes

            int length;
            ModelContainer.ModelContainer.ObjectChange[] array;
            TW.Model.GetEntityChanges(out array, out length);


            // Buffers
            var keys = new List<string>();
            var values = new List<string>();


            for (int i = 0; i < length; i++)
            {
                var change = array[length];

                // Clear buffers
                keys.Clear();
                values.Clear();

                var p = new ChangePacket();

                p.ChangeType = change.ChangeType;
                p.Guid = getObjectGuid(change.ModelObject);
                p.TypeFullName = change.ModelObject.GetType().FullName;

                foreach (var att in ReflectionHelper.GetAllAttributes(change.ModelObject.GetType()))
                {
                    keys.Add(att.Name);
                    IModelObject source = change.ModelObject;
                    values.Add(serializeObject(att.GetData(source)));

                }

                p.Keys = keys.ToArray();
                p.Values = values.ToArray();

                transporter.SendAll(p); // Send!!



            }

            // Accept remote changes


            var remoteChanges = acceptRemoteChanges();
            foreach (var p in remoteChanges)
            {
                IModelObject target;


                switch (p.ChangeType)
                {
                    case ModelContainer.ModelContainer.WorldChangeType.Added:
                        target = (IModelObject)Activator.CreateInstance(resolveTypeName(p.TypeFullName));
                        TW.Model.AddObject(target);
                        break;
                    case ModelContainer.ModelContainer.WorldChangeType.Modified:
                        target = getObjectByGuid(p.Guid);
                        break;
                    case ModelContainer.ModelContainer.WorldChangeType.Removed:
                        target = getObjectByGuid(p.Guid);
                        TW.Model.RemoveObject(target);
                        continue; // stop here for this object

                    default:
                        throw new InvalidOperationException();
                }

                for (int i = 0; i < p.Keys.Length; i++)
                {
                    var key = p.Keys[i];
                    var value = p.Values[i];

                    var att = ReflectionHelper.getAttributeByName(target.GetType(), key);

                    att.SetData(target, deserializeObject(value, att.Type));


                }
            }

        }

        /// <summary>
        /// Resolves a remote type 'FullName' to a local loaded Type (this could require more specific logic later on)
        /// Note: probably INCREDIBLY SLOW
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Type resolveTypeName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).First(o => o.FullName == name);
        }

        private List<ChangePacket> acceptRemoteChanges()
        {
            var ret = new List<ChangePacket>();

            while (transporter.PacketAvailable)
            {
                IClient client;
                ret.Add(transporter.Receive(out client));
            }

            return ret;
        }

        private string serializeObject(object obj)
        {
            var type = obj.GetType();
            if (type is IModelObject)
            {
                return getObjectGuid((IModelObject)obj).ToString();
            }
            if (type is IAsset)
            {
                var asset = (IAsset)obj;
                return asset.Guid.ToString();
            }

            return stringSerializer.Serialize(obj);

        }

        private object deserializeObject(string value, Type type)
        {
            if (type is IModelObject)
            {
                var guid = Guid.Parse(value);
                return getObjectByGuid(guid);
            }
            if (type is IAsset)
            {
                throw new NotImplementedException();
            }


            return stringSerializer.Deserialize(value, type);

        }


        private DictionaryTwoWay<IModelObject, Guid> guidMap = new DictionaryTwoWay<IModelObject, Guid>();

        private Guid getObjectGuid(IModelObject obj)
        {
            if (!guidMap.Contains(obj))
            {
                guidMap.Add(obj, new Guid());
            }
            return guidMap[obj];
        }
        private IModelObject getObjectByGuid(Guid guid)
        {
            if (!guidMap.Contains(guid))
                return null;

            return guidMap[guid];
        }
    }
}
