using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.Pickupping;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Characters
{
    public interface IRTSCharacter : IPickupObject
    {
        Entity Used { get; set; }
        Entity Attacked { get; set; }
        Vector3 Position { get; set; }
    }
}