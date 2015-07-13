using System;

namespace MHGameWork.TheWizards
{
    public struct Plane
    {
        public static implicit operator Microsoft.Xna.Framework.Plane(Plane m)
        {
            throw new NotImplementedException();
        }
        public static implicit operator Plane(Microsoft.Xna.Framework.Plane m)
        {
            throw new NotImplementedException();
        }
        public static implicit operator SlimDX.Plane(Plane m)
        {
            throw new NotImplementedException();
        }
        public static implicit operator Plane(SlimDX.Plane m)
        {
            throw new NotImplementedException();
        }
    }
}