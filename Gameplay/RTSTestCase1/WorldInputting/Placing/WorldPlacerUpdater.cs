using MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Placing;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Selecting;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldInputting
{
    /// <summary>
    /// Simulates the World Selection tool
    /// </summary>
    public class WorldPlacerUpdater
    {
        private readonly WorldPlacer placer;
        private BoundingBoxSelectableProvider selector;

        public WorldPlacerUpdater(WorldPlacer placer, WorldSelector wSelector)
        {
            this.placer = placer;

            selector = BoundingBoxSelectableProvider.Create
                (
                    items: placer.getItems(),
                    getBoundingBox: placer.getBoundingBox,
                    onClick: onClick

                );

            wSelector.AddProvider(selector);

        }

        private object heldObject;
        private float heldDistance;

        private void onClick(object obj)
        {
            
        }

        private void pickup(object item)
        {
            heldObject = item;
            //TODO
        }

        private void drop()
        {
            heldObject = null;
        }


        public void Simulate()
        {
            simulateInput();
            simulateRender();
        }

        private void simulateInput()
        {
            if (TW.Graphics.Mouse.RightMouseJustPressed)
            {
                if (heldObject != null)
                    drop();
                else
                    pickup(selector.Targeted);
            }
        }

        private void simulateRender()
        {
            foreach (var item in placer.getItems())
            {
                var bb = placer.getBoundingBox(item);

                var c = new Color4(0, 1, 0);

                if (selector.IsTargeted(item))
                    c = new Color4(1, 0, 0);

                TW.Graphics.LineManager3D.AddBox(bb, c);
            }
        }
    }
}