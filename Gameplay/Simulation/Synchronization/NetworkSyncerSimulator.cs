using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Collections;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.ModelContainer.Synchronization;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.Reflection;

namespace MHGameWork.TheWizards.Simulation.Synchronization
{
    /// <summary>
    /// This simulator is responsible for sending local changes to the remotes, and then accepting
    /// the changes from the remotes and applying it to the local model.
    /// 
    /// All changes from simulators that run after this simulator are NOT synced over network
    /// 
    /// The syncer sends broadcasts a dummy change to all the remotes, as a cheat to notify them of our existance
    /// </summary>
    public class NetworkSyncerSimulator : ISimulator
    {
        private const string nullString = "[[[[NULL]]]]";
        private StringSerializer stringSerializer = StringSerializer.Create();

        private IServerPacketTransporter<ChangePacket> transporter;

        public NetworkSyncerSimulator(IServerPacketTransporter<ChangePacket> transporter)
        {
            this.transporter = transporter;
            transporter.EnableReceiveMode();
        }

        bool firstSimulate = true;

        public void Simulate()
        {
            if (firstSimulate)
            {
                // Broadcast dummy change to the remotes to notify them of our existance
                transporter.SendAll(new ChangePacket { ChangeType = ModelContainer.ModelContainer.WorldChangeType.None });
                firstSimulate = false;
            }

            // Send local changes
            sendLocalChanges();

            // Accept remote changes
            acceptRemoteChanges();
        }

        private void acceptRemoteChanges()
        {
            while (transporter.PacketAvailable)
            {
                IClient client;
                var p = transporter.Receive(out client);

                // Check if we need to send a full model state
                if (isNewClient(client))
                {
                    foreach (var obj in getSyncedObjects())
                    {
                        transporter.SendTo(client,
                                           createChangePacket(obj, ModelContainer.ModelContainer.WorldChangeType.Added));
                    }
                    markClientNotNew(client);
                }

                // Relay changes to the previously synced clients (except the origin of the packet!!)! TODO: bounce problem??

                foreach (var cl in clients)
                {
                    if (cl == client) continue;

                    transporter.SendTo(cl, p);
                }

                // Integrate change in our model

                applyChangePacket(p);
            }
        }

        private void applyChangePacket(ChangePacket p)
        {
            IModelObject target;
            switch (p.ChangeType)
            {
                case ModelContainer.ModelContainer.WorldChangeType.Added:
                    target = (IModelObject)Activator.CreateInstance(resolveTypeName(p.TypeFullName));
                    setObjectGuid(target, p.Guid);
                    if (target.Container == null)
                    {
                        throw new InvalidOperationException("Modelobject did not add itself to a container!! => " +
                                                            p.TypeFullName);
                    }
                    //TW.Model.AddObject(target);   This is called automatically by the modelobject's constructor
                    break;
                case ModelContainer.ModelContainer.WorldChangeType.Modified:
                    target = getObjectByGuid(p.Guid);
                    break;
                case ModelContainer.ModelContainer.WorldChangeType.Removed:
                    target = getObjectByGuid(p.Guid);
                    TW.Model.RemoveObject(target);
                    return;
                case ModelContainer.ModelContainer.WorldChangeType.None:
                    // do nothing! This should be a dummy packet to notify us of a remote's existance
                    return;
                default:
                    throw new InvalidOperationException();
            }

            for (int i = 0; i < p.Keys.Length; i++)
            {
                var key = p.Keys[i];
                var value = p.Values[i];

                var att = ReflectionHelper.GetAttributeByName(target.GetType(), key);

                att.SetData(target, deserializeObject(value, att.Type));


            }
        }

        private void sendLocalChanges()
        {
            int length;
            ModelContainer.ModelContainer.ObjectChange[] array;
            TW.Model.GetEntityChanges(out array, out length);


            // Buffers


            for (int i = 0; i < length; i++)
            {
                var change = array[i];

                if (change.ChangeType == ModelContainer.ModelContainer.WorldChangeType.None) continue;
                if (Attribute.GetCustomAttribute(change.ModelObject.GetType(), typeof(NoSyncAttribute)) != null)
                    continue; // don't sync this type of object

                // Create a change packet

                ChangePacket p = createChangePacket(change.ModelObject, change.ChangeType);

                // Send!!

                foreach (var cl in clients)
                {
                    // Send to each previously synced client these delta updates
                    transporter.SendTo(cl, p);
                }




            }
        }

        private ChangePacket createChangePacket(IModelObject obj, ModelContainer.ModelContainer.WorldChangeType changeType)
        {
            // Clear buffers

            keys.Clear();
            values.Clear();

            var p = new ChangePacket();

            p.ChangeType = changeType;
            p.Guid = getObjectGuid(obj);
            p.TypeFullName = obj.GetType().FullName;

            foreach (var att in ReflectionHelper.GetAllAttributes(obj.GetType()))
            {
                keys.Add(att.Name);
                IModelObject source = obj;
                values.Add(serializeObject(att.GetData(source)));

            }

            p.Keys = keys.ToArray();
            p.Values = values.ToArray();
            return p;
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

        private string serializeObject(object obj)
        {
            if (obj == null)
                return nullString;
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
            if (value == nullString)
                return null;
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

        private List<IClient> clients = new List<IClient>();
        private Boolean isNewClient(IClient client)
        {
            return !clients.Contains(client);
        }
        private void markClientNotNew(IClient client)
        {
            if (!isNewClient(client))
                throw new InvalidOperationException();
            clients.Add(client);
        }


        private DictionaryTwoWay<IModelObject, Guid> guidMap = new DictionaryTwoWay<IModelObject, Guid>();
        private List<string> values = new List<string>();
        private List<string> keys = new List<string>();

        private void setObjectGuid(IModelObject obj, Guid guid)
        {
            if (guidMap.Contains(obj) || guidMap.Contains(guid))
                throw new InvalidOperationException("Already added!!");
            guidMap.Add(obj, guid);
        }

        private Guid getObjectGuid(IModelObject obj)
        {
            if (!guidMap.Contains(obj))
            {
                guidMap.Add(obj, Guid.NewGuid());
            }
            return guidMap[obj];
        }
        private IModelObject getObjectByGuid(Guid guid)
        {
            if (!guidMap.Contains(guid))
                return null;

            return guidMap[guid];
        }

        private IEnumerable<IModelObject> getSyncedObjects()
        {
            return guidMap.GetAllT();
        }
    }
}
