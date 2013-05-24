using System;
using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Placing
{
    public class WorldPlacer
    {
        public Func<IEnumerable<object>> GetItems;
        public Func<object, Vector3> GetPosition;
        public Func<object, Vector3, Vector3> SetPosition;
        public Func<object, BoundingBox> GetBoundingBox;
        public Func<object> CreateItem;
        public readonly Action<object> DeleteItem;

        public WorldPlacer(Func<IEnumerable<object>> getItems, Func<object, Vector3> getPosition, Func<object, Vector3, 
            Vector3> setPosition, Func<object, BoundingBox> getBoundingBox, Func<object> createItem, Action<object> deleteItem)
        {
            this.GetItems = getItems;
            this.GetPosition = getPosition;
            this.SetPosition = setPosition;
            this.GetBoundingBox = getBoundingBox;
            this.CreateItem = createItem;
            this.DeleteItem = deleteItem;
        }
    }
}