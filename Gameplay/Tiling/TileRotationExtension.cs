using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;

namespace MHGameWork.TheWizards.Tiling
{
    /// <summary>
    /// Provides functions for the TileRotation enum
    /// </summary>
    public static class TileRotationExtension
    {
        public static float GetRadians(this TileRotation rot)
        {
            switch (rot)
            {
                case TileRotation.Rotation0:
                    return 0;
                case TileRotation.Rotation180:
                    return MathHelper.Pi;
                case TileRotation.Rotation90:
                    return MathHelper.PiOver2;
                case TileRotation.Rotation270:
                    return MathHelper.PiOver2 * 3;

            }
            throw new InvalidOperationException();
        
        }

        public static TileRotation Rotate(this TileRotation rot, TileRotation rotation)
        {
            return (TileRotation) (((int) rot + (int) rotation)%4);
        }
    }
}
