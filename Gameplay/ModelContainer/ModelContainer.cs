using System;
using MHGameWork.TheWizards.Utilities;

namespace MHGameWork.TheWizards.ModelContainer
{
    /// <summary>
    /// Responsible for providing access to World Data.
    /// Responsible for providing access to World changes
    /// Algorithm: store all changes for 1 frame into a buffer, so that all classes needing those changes can apply them in 1 frame
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
                                                 change.ChangeType = WorldChangeType.Added;
                                             }, delegate(IModelObject obj)
                                                {
                                                    var change = dirtyEntities.Add();
                                                    change.ModelObject = obj;
                                                    change.ChangeType = WorldChangeType.Removed;
                                                });

            dirtyEntities = new PrefilledList<ObjectChange>(() => new ObjectChange());

        }

        private PrefilledList<ObjectChange> dirtyEntities;
        public void GetEntityChanges(out ObjectChange[] array, out int length)
        {
            dirtyEntities.GetArray(out array, out length);
        }

        /// <summary>
        /// Registers a modelobject for change logging. This method is supposed to be called by the ModelObject's themselves
        /// </summary>
        /// <param name="obj"></param>
        public void AddObject(IModelObject obj)
        {
            if (Objects.Contains(obj)) throw new InvalidOperationException("Object already added");

            Objects.Add(obj);

            obj.Initialize(this);

            flagChanged(obj, WorldChangeType.Added);
        }
        public void RemoveObject(IModelObject obj)
        {
            if (!Objects.Contains(obj)) throw new InvalidOperationException("Object not in list");
            flagChanged(obj, WorldChangeType.Removed);
        }


        public void ClearDirty()
        {
            dirtyEntities.Clear();
        }

        public void NotifyObjectModified(IModelObject obj)
        {
            if (!Objects.Contains(obj)) throw new InvalidOperationException("Object not in this container!");
            flagChanged(obj, WorldChangeType.Modified);
        }

        private void flagChanged(IModelObject obj, WorldChangeType changeType)
        {
            var ret = getChange(obj);


            if (changeType == WorldChangeType.Added || changeType == WorldChangeType.Removed) // added or removed are exclusive
            {
                ret.ChangeType = changeType;
                return;
            }

            if (changeType == WorldChangeType.Modified)
            {
                if (ret.ChangeType == WorldChangeType.None || ret.ChangeType == WorldChangeType.Modified)
                {
                    ret.ChangeType = changeType;
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
            ret.ChangeType = WorldChangeType.None;

            return ret;
        }


        public class ObjectChange
        {
            public IModelObject ModelObject { get; set; }
            public WorldChangeType ChangeType { get; set; }
        }

        public enum WorldChangeType
        {
            None = 0,
            Added = 1,
            Modified = 2,
            Removed = 3

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






    }
}





