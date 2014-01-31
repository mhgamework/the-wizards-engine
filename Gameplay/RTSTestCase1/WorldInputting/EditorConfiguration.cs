using MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Placing;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Selecting;
using MHGameWork.TheWizards.RTSTestCase1._Tests;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldInputting
{
    /// <summary>
    /// Contains the configuration for the current active editor features.
    /// </summary>
    public class EditorConfiguration
    {
        public EditorConfiguration()
        {
            Menu = new EditorMenuConfiguration();
        }
        public EditorMenuConfiguration Menu { get; set; }

        public WorldPlacer Placer { get; set; }

        public BoundingBoxSelectableProvider SelectableProvider { get; set; }
    }
}