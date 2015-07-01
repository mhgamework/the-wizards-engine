using System;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    public interface ICustomGBufferRenderer : IDisposable
    {
        void Draw();
    }
}