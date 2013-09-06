using System.Collections.Generic;
using SlimDX;

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
}