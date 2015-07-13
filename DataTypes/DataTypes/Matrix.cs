using System;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards
{
    public struct Matrix
    {


        public static implicit operator Microsoft.Xna.Framework.Matrix(Matrix m)
        {
            throw new NotImplementedException();
        }
        public static implicit operator Matrix(Microsoft.Xna.Framework.Matrix m)
        {
            throw new NotImplementedException();
        }
        public static implicit operator SlimDX.Matrix(Matrix m)
        {
            throw new NotImplementedException();
        }
        public static implicit operator Matrix(SlimDX.Matrix m)
        {
            throw new NotImplementedException();
        }
    }
}