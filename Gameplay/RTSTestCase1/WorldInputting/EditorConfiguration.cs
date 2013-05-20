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
        public EditorMenuConfiguration Menu { get; private set; }
    }
}