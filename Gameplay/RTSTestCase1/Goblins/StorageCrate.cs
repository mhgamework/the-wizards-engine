using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Goblins.Components;
using MHGameWork.TheWizards.RTSTestCase1._Engine;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    /// <summary>
    /// A crate that can store items!
    /// </summary>
    [ModelObjectChanged]
    public class StorageCrate:EngineModelObject
    {
        public StorageCrate()
        {

        }

        public Physical Physical { get; set; }
        public CommandHolderPart CommandHolder { get; set; }


    }
}