using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Collections;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Networking.Server;
using MHGameWork.TheWizards.Reflection;
using MHGameWork.TheWizards.Synchronization;
using MHGameWork.TheWizards.WorldRendering;

namespace MHGameWork.TheWizards.Simulators
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
            typeSerializer = TypeSerializer.Create();
        }

        bool firstSimulate = true;

        public void Simulate()
        {
            if (firstSimulate)
            {
                // Broadcast dummy change to the remotes to notify them of our existance
                transporter.SendAll(new ChangePacket { Change = ModelChange.None });
                firstSimulate = false;
            }

            // Send local changes
            sendLocalChanges();

            // Accept remote changes
            acceptRemoteChanges();
        }

        private void acceptRemoteChanges()
        {
            var failed = new List<ChangePacket>();

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
                                           createChangePacket(obj, ModelChange.Added));
                    }
                    markClientNotNew(client);
                }

                // Relay changes to the previously synced clients (except the origin of the packet!!)! TODO: bounce problem??

                foreach (var cl in clients)
                {
                    if (cl == client) continue;

                    transporter.SendTo(cl, p);
                }

                // Queue for applying
                failed.Add(p);
            }

            var retry = new List<ChangePacket>();

            var allFailed = true;

            // Do some form of constraint processing to apply all the changes
            // TODO: this fails when using circular dependencies in the ModelObjects!!!
            while (failed.Count > 0)
            {
                var swap = retry;
                retry = failed;
                failed = swap;

                failed.Clear();

                foreach (var p in retry)
                {
                    // Attempt to integrate in our model

                    if (applyChangePacket(p))
                    {
                        allFailed = false;
                        continue;
                    }
                    failed.Add(p);
                }

                if (allFailed)
                {
                    // Could not apply all packets!! 
                    Console.WriteLine("Could not apply all ChangePackets!! Discarding some packets Oo...");
                    break;
                }
            }
        }

        /// <summary>
        /// Returns true when the changed was applied successfully
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private bool applyChangePacket(ChangePacket p)
        {
            // First decode the serialized objects, and see if there is an unresolved IModelObject amongst them


            IModelObject target;
            object[] deserialized;
            switch (p.Change)
            {
                case ModelChange.Added:

                    var type = typeSerializer.Deserialize(p.TypeFullName);

                    if (!deserializeValues(type, p, out deserialized))
                        return false;

                    target = (IModelObject)Activator.CreateInstance(type);
                    setObjectGuid(target, p.Guid);
                    if (target.Container == null)
                        throw new InvalidOperationException("Modelobject did not add itself to a container!! => " +
                                                            p.TypeFullName);

                    applyValues(target, p.Keys, deserialized); // Set initial data

                    break;
                case ModelChange.Modified:

                    target = getObjectByGuid(p.Guid);

                    if (!deserializeValues(target.GetType(), p, out deserialized))
                        return false;

                    applyValues(target, p.Keys, deserialized); // Apply changes

                    break;
                case ModelChange.Removed:
                    target = getObjectByGuid(p.Guid);
                    TW.Model.RemoveObject(target);
                    return true;
                case ModelChange.None:
                    // do nothing! This should be a dummy packet to notify us of a remote's existance
                    return true;
                default:
                    throw new InvalidOperationException();
            }



            return true;
        }

        private void applyValues(object target, string[] names, object[] values)
        {
            for (int i = 0; i < names.Length; i++)
            {

                var key = names[i];
                var value = values[i];

                var att = ReflectionHelper.GetAttributeByName(target.GetType(), key);

                att.SetData(target, value);


            }
        }

        /// <summary>
        /// Returns false when not everything can be deserialized
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="p"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private bool deserializeValues(Type targetType, ChangePacket p, out object[] ret)
        {
            ret = new object[p.Keys.Length];
            for (int i = 0; i < p.Keys.Length; i++)
            {
                var key = p.Keys[i];
                var value = p.Values[i];

                var att = ReflectionHelper.GetAttributeByName(targetType, key);

                if (!deserializeObject(value, att.Type, out ret[i]))
                {
                    return false;
                }

            }

            return true;
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

                if (change.Change == ModelChange.None) continue;
                if (Attribute.GetCustomAttribute(change.ModelObject.GetType(), typeof(NoSyncAttribute)) != null)
                    continue; // don't sync this type of object

                // Create a change packet

                ChangePacket p = createChangePacket(change.ModelObject, change.Change);

                // Send!!

                foreach (var cl in clients)
                {
                    // Send to each previously synced client these delta updates
                    transporter.SendTo(cl, p);
                }




            }
        }

        private ChangePacket createChangePacket(IModelObject obj, ModelChange change)
        {
            // Clear buffers

            keys.Clear();
            values.Clear();

            var p = new ChangePacket();

            p.Change = change;
            p.Guid = getObjectGuid(obj);
            p.TypeFullName = typeSerializer.Serialize(obj.GetType());

            foreach (var att in ReflectionHelper.GetAllAttributes(obj.GetType()))
            {
                var data = att.GetData(obj);
                if (data != null && Attribute.GetCustomAttribute(data.GetType(), typeof(NoSyncAttribute)) != null)
                    continue; // Do not sync this type of object!!!!


                keys.Add(att.Name);


                values.Add(serializeObject(data));

            }

            p.Keys = keys.ToArray();
            p.Values = values.ToArray();
            return p;
        }



        private string serializeObject(object obj)
        {
            if (obj == null)
                return nullString;
            var type = obj.GetType();
            if (typeof(IModelObject).IsAssignableFrom(type))
            {

                return getObjectGuid((IModelObject)obj).ToString();
            }
            if (typeof(IAsset).IsAssignableFrom(type))
            {
                var asset = (IAsset)obj;
                return asset.Guid.ToString();
            }

            return stringSerializer.Serialize(obj);

        }

        /// <summary>
        /// Returns false when the string can't be deserialized
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool deserializeObject(string value, Type type, out object deserialized)
        {
            deserialized = null;

            if (value == nullString)
                return true;
            if (typeof(IModelObject).IsAssignableFrom(type))
            {
                var guid = Guid.Parse(value);
                var obj = getObjectByGuid(guid);
                if (obj == null)
                {
                    return false;// We dont know about the existance of this object yet! ==> Fail

                }
                deserialized = obj;
                return true;
            }
            if (typeof(IAsset).IsAssignableFrom(type))
            {
                deserialized = TW.Model.GetSingleton<RenderingModel>().AssetFactory.GetAsset(type, Guid.Parse(value));
                return true;

            }


            deserialized = stringSerializer.Deserialize(value, type);
            return true;

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
        private TypeSerializer typeSerializer;

        private void setObjectGuid(IModelObject obj, Guid guid)
        {
            if (guidMap.Contains(obj) || guidMap.Contains(guid))
                throw new InvalidOperationException("Already added!!");
            guidMap.Add(obj, guid);
        }

        private Guid getObjectGuid(IModelObject obj)
        {
            // Algorithm safety :D
            if (Attribute.GetCustomAttribute(obj.GetType(), typeof(NoSyncAttribute)) != null)
                throw new InvalidOperationException("This is not allowed!!! EVAR");

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
