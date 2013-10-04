using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.SkyMerchant.Prototype.Parts;

namespace MHGameWork.TheWizards.Engine.Worlding
{
    /// <summary>
    /// Responsible for indicating that a modelobject should be physical.
    /// Responsible for binding modelobject properties to entities.
    /// 
    /// UpdatePhysical is a method that defines part of the modelobject. 
    /// It could also be in non-code format, but all code is the way to go. 
    /// It might be necessary to move some objects definitions to other locations later on.
    /// </summary>
    public interface IPhysical : IModelObject
    {
        IPhysicalPart Physical { get; set; }
        void UpdatePhysical();
    }
}