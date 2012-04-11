using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards.Tiling
{
    public static class TileFaceExtensions
    {
        private static Vector3[] normals;
        private static Vector3[] ups;
        private static Vector3[] rights;

        static TileFaceExtensions()
        {
            normals = new Vector3[7];
            ups = new Vector3[7];
            rights = new Vector3[7];

            normals[(int)TileFace.Back] = Vector3.UnitZ;
            normals[(int)TileFace.Front] = -Vector3.UnitZ;
            normals[(int)TileFace.Left] = Vector3.UnitX;
            normals[(int)TileFace.Right] = -Vector3.UnitX;
            normals[(int)TileFace.Up] = Vector3.UnitY;
            normals[(int)TileFace.Down] = -Vector3.UnitY;

            ups[(int)TileFace.Back] = Vector3.UnitY;
            ups[(int)TileFace.Front] = Vector3.UnitY;
            ups[(int)TileFace.Left] = Vector3.UnitY;
            ups[(int)TileFace.Right] = Vector3.UnitY;
            ups[(int)TileFace.Up] = Vector3.UnitZ;
            ups[(int)TileFace.Down] = -Vector3.UnitZ;


            for (int i = 0; i < normals.Length; i++)
            {
                rights[i] = Vector3.Cross(ups[i], normals[i]);
            }
        }

        public static Vector3 Normal(this TileFace face) { return normals[(int)face]; }
        public static Vector3 Up(this TileFace face) { return ups[(int)face]; }
        public static Vector3 Right(this TileFace face) { return rights[(int)face]; }
    }
}
