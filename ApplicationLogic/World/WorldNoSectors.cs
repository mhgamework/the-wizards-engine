using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Utilities;

namespace MHGameWork.TheWizards.World
{
    /// <summary>
    /// Responsible for providing access to World Data.
    /// Responsible for providing access to World changes
    /// Algorithm: store all changes for 1 frame into a buffer, so that all classes needing those changes can apply them in 1 frame
    /// </summary>
    public class WorldNoSectors
    {
        public EventList<Entity> Entities { get; private set; }

        public WorldNoSectors()
        {
            Entities = new EventList<Entity>(delegate(Entity obj)
                                             {
                                                 var change = dirtyEntities.Add();
                                                 change.Entity = obj;
                                                 change.ChangeType = WorldChangeType.Added;
                                             }, delegate(Entity obj)
                                                {
                                                    var change = dirtyEntities.Add();
                                                    change.Entity = obj;
                                                    change.ChangeType = WorldChangeType.Removed;
                                                });

            dirtyEntities = new PrefilledList<EntityChange>(() => new EntityChange());

        }

        private PrefilledList<EntityChange> dirtyEntities;
        public void GetEntityChanges(out EntityChange[] array, out int length)
        {
            dirtyEntities.GetArray(out array, out length);
        }



        public void ClearDirty()
        {
            for (int i = 0; i < dirtyEntities.Count; i++)
            {
                dirtyEntities[i].Entity.IsDirty = false;
            }
            dirtyEntities.Clear();
        }

        public void FlagDirty(Entity ent)
        {
            ent.IsDirty = true;
            var change = dirtyEntities.Add();
            change.Entity = ent;
            change.ChangeType = WorldChangeType.Modified;
        }

        public class EntityChange
        {
            public Entity Entity { get; set; }
            public WorldChangeType ChangeType { get; set; }
        }

        public enum WorldChangeType
        {
            None = 0,
            Added = 1,
            Modified = 2,
            Removed = 3

        }
    }
}
