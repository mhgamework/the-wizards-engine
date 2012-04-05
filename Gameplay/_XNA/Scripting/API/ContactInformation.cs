using Microsoft.Xna.Framework;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards._XNA.Scripting.API
{
    /// <summary>
    /// This is currently for use with the RegisterContact function in the EntityHandle
    /// </summary>
    public struct ContactInformation
    {
        public IEntity OtherEntity;
        //public IEntity EntityB;
        public ContactPairFlag Flags;
        public Vector3 FrictionForce;
        public Vector3 NormalForce;
        
    }
}
