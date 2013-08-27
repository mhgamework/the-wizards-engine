using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    [ModelObjectChanged]
    public class BasicPhysicsPart : EngineModelObject
    {
        #region Injection

        public Vector3 Velocity { get; set; }

        #endregion


        public BasicPhysicsPart()
        {
            
        }

        /// <summary>
        /// This method applies given force for this tick
        /// Don't apply elapsed to the force parameter yourself!
        /// </summary>
        /// <param name="force"></param>
        public void ApplyForce(Vector3 force)
        {
            Velocity += force * TW.Graphics.Elapsed; //TODO: incorporate mass here? (f=ma)
        } 
    }
}