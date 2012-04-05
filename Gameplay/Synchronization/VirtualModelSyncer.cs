﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.ModelContainer;
using MHGameWork.TheWizards.Utilities;

namespace MHGameWork.TheWizards.Synchronization
{
    /// <summary>
    /// TODO: fix unique id collision
    /// TODO: finish this
    /// </summary>
    public class VirtualModelSyncer : IVirtualEndpoint
    {
        private readonly ModelContainer.ModelContainer container;
        private readonly byte unqiueSyncerId;
        private IVirtualEndpoint remoteEndPoint;


        public VirtualModelSyncer(ModelContainer.ModelContainer container, byte unqiueSyncerID)
        {
            this.container = container;
            unqiueSyncerId = unqiueSyncerID;
        }

        public void setEndpoint(IVirtualEndpoint endPoint)
        {
            remoteEndPoint = endPoint;
        }

        public void ApplyRemoteChanges()
        {
            //Do nothing!, currently these are applied immediately
        }


        private ChangesBuffer localChanges = new ChangesBuffer();

        public void SendLocalChanges()
        {
            localChanges.Clear();
            int length;
            ModelContainer.ModelContainer.ObjectChange[] array;
            container.GetEntityChanges(out array, out length);

            for (int i = 0; i < length; i++)
            {
                var change = array[i];
                var syncedObject = getSyncedObject(change.ModelObject);
                var syncChange = localChanges.Add();
                syncChange.Object = syncedObject;
                syncChange.ChangeType = change.ChangeType;
            }

            remoteEndPoint.ApplyModelChanges(localChanges);

        }

        public class ChangesBuffer : IEnumerable<SyncChange>
        {
            private PrefilledList<SyncChange> buffer = new PrefilledList<SyncChange>(() => new SyncChange());

            public IEnumerator<SyncChange> GetEnumerator()
            {

                for (int i = 0; i < buffer.Count; i++)
                {
                    yield return buffer[i];
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public SyncChange Add()
            {
                return buffer.Add();
            }
            public void Clear()
            {
                buffer.Clear();
            }



        }





        void IVirtualEndpoint.ApplyModelChanges(ChangesBuffer changes)
        {
            foreach (var change in changes)
            {

                switch (change.ChangeType)
                {
                    case ModelContainer.ModelContainer.WorldChangeType.Added:
                        applyObjectAdded(change);
                        break;
                    case ModelContainer.ModelContainer.WorldChangeType.Modified:
                        applyObjectModified(change);
                        break;
                    case ModelContainer.ModelContainer.WorldChangeType.Removed:
                        applyObjectRemoved(change);
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
        public class SyncChange
        {
            public SyncedObject Object;
            public ModelContainer.ModelContainer.WorldChangeType ChangeType;
        }

        private void applyObjectAdded(SyncChange change)
        {
            var newInstance = (IModelObject)Activator.CreateInstance(change.Object.ModelObject.GetType());
            container.AddObject(newInstance);

            var syncedObject = new SyncedObject { ModelObject = newInstance, UniqueId = change.Object.UniqueId };
            objects.Add(syncedObject);


            synchronizeModelObject(change.Object.ModelObject, newInstance);
        }
        private void applyObjectModified(SyncChange change)
        {
            synchronizeModelObject(change.Object.ModelObject, objects.Find(o => o.UniqueId == change.Object.UniqueId).ModelObject);
        }
        private void applyObjectRemoved(SyncChange change)
        {
            var localObject = objects.Find(o => o.UniqueId == change.Object.UniqueId);
            container.RemoveObject(localObject.ModelObject);
            objects.Remove(localObject);
        }

        private void synchronizeModelObject(IModelObject source, IModelObject target)
        {
            var properties = target.GetType().GetProperties().Where(
              info => Attribute.IsDefined(info, typeof(ModelObjectChangedAttribute)));

            foreach (var prop in properties)
            {
                prop.SetValue(target, prop.GetValue(source, null), null);
            }
        }



        private List<SyncedObject> objects = new List<SyncedObject>();
        private int nextUniqueId = 1;

        private SyncedObject getSyncedObject(IModelObject obj)
        {
            var ret = objects.Find(o => o.ModelObject == obj);
            if (ret == null)
            {
                ret = new SyncedObject();
                ret.ModelObject = obj;
                ret.UniqueId = unqiueSyncerId << 24 + nextUniqueId++;
                objects.Add(ret);
            }
            return ret;
        }

        public class SyncedObject
        {
            public IModelObject ModelObject;
            public int UniqueId;
        }

    }
}
