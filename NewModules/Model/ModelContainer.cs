using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Utilities;
using SlimDX;

namespace MHGameWork.TheWizards.Model
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

        public void AddObject(IModelObject obj)
        {
            if (Objects.Contains(obj)) throw new InvalidOperationException("Object already added");

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

            if (changeType == WorldChangeType.Modified && (ret.ChangeType == WorldChangeType.None || ret.ChangeType == WorldChangeType.Modified))
            {
                ret.ChangeType = changeType;
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





        // Helper methods (?)
        public Entity CreateNewEntity(IMesh mesh, Matrix worldMatrix)
        {
            var ent = new Entity(this);

            Objects.Add(ent);

            ent.Mesh = mesh;
            ent.WorldMatrix = worldMatrix;

            return ent;
        }

    }
}





