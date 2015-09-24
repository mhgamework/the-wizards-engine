namespace SlimDX
{
    public struct Vector3_Adapter
    {
        public static global::SlimDX.Vector3_Adapter operator +(global::SlimDX.Vector3_Adapter left, global::SlimDX.Vector3_Adapter right)
        {
            throw new System.InvalidOperationException();
        }

        public static global::SlimDX.Vector3_Adapter operator *(System.Single scale, global::SlimDX.Vector3_Adapter vector)
        {
            throw new System.InvalidOperationException();
        }
    }
}