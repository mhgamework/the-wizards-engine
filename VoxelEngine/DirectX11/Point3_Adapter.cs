namespace DirectX11
{
    public struct Point3_Adapter
    {
        public global::SlimDX.Vector3 ToVector3()
        {
            throw new System.InvalidOperationException();
        }

        public static System.Boolean operator ==(global::DirectX11.Point3_Adapter p, global::DirectX11.Point3_Adapter p2)
        {
            throw new System.InvalidOperationException();
        }

        public static bool operator !=( Point3_Adapter p, Point3_Adapter p2 )
        {
            return !( p == p2 );
        }

        public static global::DirectX11.Point3_Adapter operator -(global::DirectX11.Point3_Adapter p, global::DirectX11.Point3_Adapter p2)
        {
            throw new System.InvalidOperationException();
        }

        public static global::DirectX11.Point3_Adapter operator +(global::DirectX11.Point3_Adapter p, global::DirectX11.Point3_Adapter p2)
        {
            throw new System.InvalidOperationException();
        }

        public static global::DirectX11.Point3_Adapter UnitX()
        {
            throw new System.InvalidOperationException();
        }

        public static global::DirectX11.Point3_Adapter UnitY()
        {
            throw new System.InvalidOperationException();
        }

        public static global::DirectX11.Point3_Adapter UnitZ()
        {
            throw new System.InvalidOperationException();
        }
    }
}