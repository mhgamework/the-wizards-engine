using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Selecting;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Placing
{
    /// <summary>
    /// Simulates the World Placer tool, by allowing for placing and deleting items
    /// TODO: moving support?
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
                    items: placer.GetItems(),
                    getBoundingBox: placer.GetBoundingBox,
                    onClick: onClick

                );

            wSelector.AddProvider(selector);

        }

        private void onClick(object obj)
        {

        }

        public void Simulate()
        {
            simulateInput();
            simulateRender();
        }

        public void Disable()
        {
            selector.Enabled = false;
        }

        private void simulateInput()
        {
            if (TW.Graphics.Mouse.RightMousePressed && selector.Targeted != null)
            {
                placer.DeleteItem(selector.Targeted);
            }
            if (TW.Graphics.Mouse.LeftMouseJustPressed && selector.Targeted == null)
            {
                var p = TW.Data.Get<CameraInfo>().GetGroundplanePosition();
                if (p.HasValue)
                {
                    var item = placer.CreateItem();
                    placer.Position = item, p.Value;
                }
            }
        }



        private void simulateRender()
        {
            
            var point = TW.Data.Get<CameraInfo>().GetGroundplanePosition();
            if (point.HasValue)
                TW.Graphics.LineManager3D.AddCenteredBox(point.Value, 0.3f, new Color4(1, 1, 0));

        }
    }
}