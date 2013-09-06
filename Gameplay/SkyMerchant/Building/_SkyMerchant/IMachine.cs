using System.Collections.Generic;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Building._SkyMerchant
{
    /// <summary>
    /// Responsible for the world related aspect of a machine
    /// Implemented by the engine
    /// </summary>
    public interface IMachine
    {

    }

    [ModelObjectChanged]
    public class SimpleMachine : EngineModelObject, IMachine
    {
        public Physical Physical = new Physical();
        public List<IItem> Items = new List<IItem>();
    }
}