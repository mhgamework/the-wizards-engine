using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX.Direct3D11;

namespace DirectX11.Graphics
{
    public interface IGraphicsManager
    {
        void AddBasicShader(BasicShader shader);
        Device Device { get; }
    }
}
