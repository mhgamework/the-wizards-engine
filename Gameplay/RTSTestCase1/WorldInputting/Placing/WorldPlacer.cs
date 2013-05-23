using System;
using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Placing
{
    public class WorldPlacer
    {
        public Func<IEnumerable<object>> getItems;
        public Func<object, Vector3> getPosition;
        public Func<object, Vector3, Vector3> setPosition;
        public Func<object, BoundingBox> getBoundingBox;
        public Func<object> createItem;

        public WorldPlacer(Func<IEnumerable<object>> getItems, Func<object, Vector3> getPosition, Func<object, Vector3, Vector3> setPosition, Func<object, BoundingBox> getBoundingBox, Func<object> createItem, Action<object> deleteItem)
        {
            this.getItems = getItems;
            this.getPosition = getPosition;
            this.setPosition = setPosition;
            this.getBoundingBox = getBoundingBox;
            this.createItem = createItem;
        }
    }
}