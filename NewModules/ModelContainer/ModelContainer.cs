using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Utilities;
using MS.Win32;

namespace MHGameWork.TheWizards.ModelContainer
{
    /// <summary>
    /// Responsible for providing access to World Data.
    /// Responsible for providing access to World changes
    /// Algorithm: store all changes for 1 frame into a buffer, so that all classes needing those changes can apply them in 1 frame
    /// TODO: partially implemented, contains dummies
    /// </summary>
    public class ModelContainer
    {

        public EventList<IModelObject> Objects { get; private set; }

        public ModelContainer()
        {
            Objects = new EventList<IModelObject>(delegate(IModelObject obj)
                                             {
                                                 var change = dirtyEntities.Add();
                                                 change.ModelObject = obj;
                                                 change.Change = ModelChange.Added;
                                             }, delegate(IModelObject obj)
                                                {
                                                    var change = dirtyEntities.Add();
                                                    change.ModelObject = obj;
                                                    change.Change = ModelChange.Removed;
                                                });

            dirtyEntities = new PrefilledList<ObjectChange>(() => new ObjectChange());

        }

        private PrefilledList<ObjectChange> dirtyEntities;
        public void GetObjectChanges(out ObjectChange[] array, out int length)
        {
            dirtyEntities.GetArray(out array, out length);
        }

        /// <summary>
        /// Registers a modelobject for change logging. This method is supposed to be called by the ModelObject's themselves, not by user
        /// </summary>
        /// <param name="obj"></param>
        public void AddObject(IModelObject obj)
        {
            if (Objects.Contains(obj)) throw new InvalidOperationException("Object already added");

            Objects.Add(obj);

            obj.Initialize(this);

            flagChanged(obj, ModelChange.Added);
        }
        /// <summary>
        /// TODO: this should be called automatically using a weak references system
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveObject(IModelObject obj)
        {
            if (!Objects.Contains(obj)) throw new InvalidOperationException("Object not in list");
            Objects.Remove(obj);
            flagChanged(obj, ModelChange.Removed);
        }


        public void ClearDirty()
        {
            dirtyEntities.Clear();
        }

        public void NotifyObjectModified(IModelObject obj)
        {
            if (!Objects.Contains(obj)) throw new InvalidOperationException("Object not in this container!");
            flagChanged(obj, ModelChange.Modified);
        }

        private void flagChanged(IModelObject obj, ModelChange change)
        {
            var ret = getChange(obj);

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

        private ObjectChange getChange(IModelObject obj)
        {
            for (int i = 0; i < dirtyEntities.Count; i++)
            {
                if (dirtyEntities[i].ModelObject != obj)
                    continue;
                return dirtyEntities[i];
            }

            var ret = dirtyEntities.Add();
            ret.ModelObject = obj;
            ret.Change = ModelChange.None;

            return ret;
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
        public T GetSingleton<T>() where T : class, IModelObject, new()
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
            for (int i = 0; i < dirtyEntities.Count; i++)
            {
                var change = dirtyEntities[i];
                if (change.ModelObject is T)
                    yield return change;
            }
        }




    }
}





