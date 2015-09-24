namespace MHGameWork.TheWizards
{
    public static class SlimDXConversion_Adapter
    {
        public static global::Microsoft.Xna.Framework.Graphics.Color xna(this global::SlimDX.Color4 v)
        {
            throw new System.InvalidOperationException();
        }

        public static global::SlimDX.Color4 dx(this global::System.Drawing.Color v)
        {
            throw new System.InvalidOperationException();
        }
    }
}