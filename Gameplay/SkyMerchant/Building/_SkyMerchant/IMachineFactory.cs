using System.Collections.Generic;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.RTSTestCase1;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant.Building._SkyMerchant
{
    /// <summary>
    /// Responsible for creating and destroying machines
    /// Implemented by the engine
    /// </summary>
    public interface IMachineFactory
    {
        IMachine CreateMachine(IIsland island, Matrix orientation, IEnumerable<IItem> items);
        IEnumerable<IItem> DestoryMachine(IMachine machine);

        IEnumerable<IMachine> GetMachinesOn(IIsland island);
    }

    public class MachineFactory : IMachineFactory
    {
        public IMachine CreateMachine(IIsland island, Matrix orientation, IEnumerable<IItem> items)
        {
            // Ignore island, assume single island
            var ret = new SimpleMachine();
            ret.Items = items.ToList();
            ret.Physical.WorldMatrix = orientation;
            ret.Physical.Mesh = UtilityMeshes.CreateMeshWithText(1, "Machine", TW.Graphics);

            return ret;
        }

        public IEnumerable<IItem> DestoryMachine(IMachine machine)
        {
            TW.Data.RemoveObject((SimpleMachine)machine);

            return ((SimpleMachine) machine).Items;
        }

        public IEnumerable<IMachine> GetMachinesOn(IIsland island)
        {
            return TW.Data.Objects.OfType<SimpleMachine>();
        }
    }
}