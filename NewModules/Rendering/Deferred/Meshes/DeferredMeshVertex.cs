using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;
using SlimDX.Direct3D11;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    public struct DeferredMeshVertex
    {
        public Vector4 Pos;
        public Vector3 Normal;
        public Vector2 UV;

        public const int SizeInBytes = 4 * (4 + 3 + 2);
        public static readonly InputElement[] Elements = new[]
                                                                {
                                                                    new InputElement("POSITION",0, SlimDX.DXGI.Format.R32G32B32A32_Float,0,0),
                                                                    new InputElement("NORMAL",0, SlimDX.DXGI.Format.R32G32B32_Float,16,0),
                                                                    new InputElement("TEXCOORD",0, SlimDX.DXGI.Format.R32G32_Float,16+12,0)
                                                                };

        public DeferredMeshVertex(Vector4 pos, Vector2 uv, Vector3 normal)
            : this()
        {
            Pos = pos;
            Normal = normal;
            UV = uv;
        }
    }
}
