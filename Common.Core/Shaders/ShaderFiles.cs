using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.DirectX11.Graphics;

namespace MHGameWork.TheWizards.Shaders
{
    /// <summary>
    /// Responsible for statically typing the shader files into the code
    /// </summary>
    public class ShaderFiles
    {
        public static readonly FileInfo DeferredMesh = c("Deferred\\DeferredMesh.fx");
        public static readonly FileInfo DCHermiteTerrain = c("DualContouring\\HermiteTerrain.fx");
        public static readonly FileInfo DCSurface = c("DualContouring\\Surface.fx");

        private static FileInfo c(string name)
        {
            return new System.IO.FileInfo(CompiledShaderCache.Current.RootShaderPath + name);
        }
    }
}
