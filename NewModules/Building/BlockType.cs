using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for representing a type of block
    /// </summary>
    public class BlockType
    {
        public readonly IMesh Mesh;
        public BlockLayout Layout;
        public Matrix Transformation;

        public BlockType(IMesh mesh)
        {
            this.Mesh = mesh;
        }
    }
}
