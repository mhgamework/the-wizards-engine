using System;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Placing
{
    public class WorldPlacer
    {
        private Func<object> getItems;
        private Func<object, Vector3> getPosition;
        private Func<object,Vector3, Vector3> setPosition;
        private Func<object, BoundingBox> getBoundingBox;
        private Func<object> createItem;

        public WorldPlacer(Func<object> getItems, Func<object, Vector3> getPosition, Func<object, Vector3, Vector3> setPosition, Func<object, BoundingBox> getBoundingBox, Func<object> createItem )
        {
            this.getItems = getItems;
            this.getPosition = getPosition;
            this.setPosition = setPosition;
            this.getBoundingBox = getBoundingBox;
            this.createItem = createItem;
        }
    }
}