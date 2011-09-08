using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.Building
{
    /// <summary>
    /// Responsible for representing a type of block
    /// </summary>
    public class BlockType
    {
        public readonly IMesh mesh;

        public BlockType(IMesh mesh)
        {
            this.mesh = mesh;
        }
    }
}
