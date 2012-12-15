using MHGameWork.TheWizards.Collections;
using MHGameWork.TheWizards.Data;

namespace MHGameWork.TheWizards.Persistence
{
    /// <summary>
    /// Implements a helper class ussed in the ModelSerializer
    /// It maps IModelObjects to IDs
    /// 
    /// TODO: maybe make this more generic?
    /// </summary>
    public class MyObjectDictionary : ModelObjectSerializer.IIDResolver
    {
        private DictionaryTwoWay<int, IModelObject> objectDictionary = new DictionaryTwoWay<int, IModelObject>();
        private int nextObjectID = 1;

        public MyObjectDictionary()
        {
        }

        int ModelObjectSerializer.IIDResolver.GetObjectID(IModelObject obj)
        {
            return getObjectID(obj);
        }

        IModelObject ModelObjectSerializer.IIDResolver.GetObjectByID(int id)
        {
            return getObjectByID(id);
        }

        public int getObjectID(IModelObject obj)
        {
            if (!objectDictionary.Contains(obj))
            {
                objectDictionary.Add(getNewObjectID(), obj);
            }
            return objectDictionary[obj];
        }

        private int getNewObjectID()
        {
            return nextObjectID++;
        }

        public IModelObject getObjectByID(int id)
        {
            if (!objectDictionary.Contains(id))
                return null;

            return objectDictionary[id];
        }

        public void setObjectID(IModelObject obj, int id)
        {
            objectDictionary.set(id, obj);
            nextObjectID = id + 1;
        }
    }
}