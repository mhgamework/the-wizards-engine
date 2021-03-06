﻿using System;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.Utilities;
using MS.Win32;

namespace MHGameWork.TheWizards.Data
{
    /// <summary>
    /// Responsible for providing access to World Data.
    /// Responsible for providing access to World changes
    /// Algorithm: store all changes for 1 frame into a buffer, so that all classes needing those changes can apply them in 1 frame
    /// TODO: partially implemented, contains dummies
    /// </summary>
    public class ModelContainer
    {
        private class EntitiesCollection
        {
            private PrefilledList<ObjectChange> list;
            private Dictionary<IModelObject, ObjectChange> getChange = new Dictionary<IModelObject, ObjectChange>();
            public EntitiesCollection(Func<ObjectChange> func)
            {
                list = new PrefilledList<ObjectChange>(func);
            }


            public void GetArray(out ObjectChange[] array, out int length)
            {
                list.GetArray(out array, out length);
            }

            public ObjectChange this[int index]
            {
                get { return list[index]; }
            }

            public void Clear()
            {
                list.Clear();
                getChange.Clear();
            }

            public ObjectChange Add(IModelObject obj)
            {
                var ret = list.Add();
                ret.ModelObject = obj;
                getChange.Add(ret.ModelObject, ret);
                return ret;
            }

            public int Count
            {
                get { return list.Count; }
            }

            public ObjectChange GetChange(IModelObject obj)
            {
                ObjectChange ret;

                if (getChange.TryGetValue(obj, out ret)) return ret;

                ret = Add(obj);
                ret.Change = ModelChange.None;

                return ret;
            }
        }

        public EventList<IModelObject> Objects { get; private set; }

        public ModelContainer()
        {
            Objects = new EventList<IModelObject>(delegate(IModelObject obj)
                                             {
                                                 //TODO: this was massive boooobooo but a genius idea :)))
                                                 // Listen on this list but do it correct this time
                                                 //var change = dirtyEntities.Add();
                                                 //change.ModelObject = obj;
                                                 //change.Change = ModelChange.Added;
                                                 flagChanged(obj, ModelChange.Added);
                                             }, delegate(IModelObject obj)
                                                {
                                                    //var change = dirtyEntities.Add();
                                                    //change.ModelObject = obj;
                                                    //change.Change = ModelChange.Removed;
                                                    flagChanged(obj, ModelChange.Removed);
                                                });

            dirtyEntities = new EntitiesCollection(() => new ObjectChange());

        }

        private EntitiesCollection dirtyEntities;
        public void GetObjectChanges(out ObjectChange[] array, out int length)
        {
            dirtyEntities.GetArray(out array, out length);
        }

        /// <summary>
        /// Registers a modelobject for change logging. This method is supposed to be called by the ModelObject's themselves, not by user
        /// </summary>
        /// <param name="obj"></param>
        public virtual void AddObject(IModelObject obj)
        {
            if (Objects.Contains(obj)) throw new InvalidOperationException("Object already added");

            Objects.Add(obj);

            obj.Initialize(this);

            //flagChanged(obj, ModelChange.Added);
        }
        /// <summary>
        /// TODO: this should be called automatically using a weak references system
        /// </summary>
        /// <param name="obj"></param>
        public virtual void RemoveObject(IModelObject obj)
        {
            if (!Objects.Contains(obj))
            {
                // Was already removed by someone else (can occur on clean!)
                //Console.WriteLine("WARNING: destruction of object in the same frame as creation: " + obj.GetType().Name);
                //flagChanged(obj, ModelChange.Removed);
                return;
            }
            Objects.Remove(obj);
            //flagChanged(obj, ModelChange.Removed);
        }


        public void ClearDirty()
        {
            dirtyEntities.Clear();
        }

        public virtual void NotifyObjectModified(IModelObject obj)
        {
            // Note: Removed for performance reasons!     if (!Objects.Contains(obj)) return;
            flagChanged(obj, ModelChange.Modified);
        }

        private void flagChanged(IModelObject obj, ModelChange change)
        {
            var ret = dirtyEntities.GetChange(obj);

            if (change == ModelChange.Removed && ret.Change == ModelChange.Added)
            {
                // Object was added, and removed again, so nothing happened!
                ret.Change = ModelChange.None;
                return;
            }

            if (change == ModelChange.Added || change == ModelChange.Removed) // added or removed are exclusive
            {
                ret.Change = change;
                return;
            }

            if (change == ModelChange.Modified)
            {
                if (ret.Change == ModelChange.None || ret.Change == ModelChange.Modified)
                {
                    ret.Change = change;
                }
                return;
            }

            throw new InvalidOperationException("This is not possible!");



        }




        public class ObjectChange
        {
            public IModelObject ModelObject { get; set; }
            public ModelChange Change { get; set; }
        }


        /// <summary>
        /// Returns or creates a unique instance of the modelobject of given type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T GetSingleton<T>() where T : class, IModelObject, new()
        {
            foreach (var obj in Objects)
            {
                if (obj is T)
                    return (T)obj;
            }

            var ret = new T();
            return ret;
        }


        public bool HasChanged(IModelObject obj)
        {
            //TODO: dummy implementation
            return true;

        }


        public IEnumerable<ObjectChange> GetChangesOfType<T>() where T : IModelObject
        {
            return getChangesOfTypeEnumerable<T>().ToArray();
        }

        private IEnumerable<ObjectChange> getChangesOfTypeEnumerable<T>() where T : IModelObject
        {
            for (int i = 0; i < dirtyEntities.Count; i++)
            {
                var change = dirtyEntities[i];
                if (change.Change == ModelChange.None) continue;
                if (change.ModelObject is T)
                    yield return change;
            }
        }

        public IEnumerable<T> GetChangedObjects<T>() where T : class, IModelObject
        {
            return getChangedObjectsEnumerable<T>().ToArray();
        }

        private IEnumerable<T> getChangedObjectsEnumerable<T>() where T : class, IModelObject
        {
            for (int i = 0; i < dirtyEntities.Count; i++)
            {
                var change = dirtyEntities[i];
                if (change.Change == ModelChange.None) continue;
                if (change.ModelObject is T)
                    yield return change.ModelObject as T;
            }
        }
    }
}





