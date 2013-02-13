using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Characters
{
    public interface IRTSCharacter
    {
        Thing Holding { get; set; }
        Entity Used { get; set; }
        Entity Attacked { get; set; }
        Vector3 Position { get; set; }
    }
}