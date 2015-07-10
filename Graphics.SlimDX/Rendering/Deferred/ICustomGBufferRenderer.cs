using System;

namespace MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Deferred
{
    public interface ICustomGBufferRenderer : IDisposable
    {
        void Draw();
    }
}