using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Building;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.Tests.Building
{
    /// <summary>
    /// Responsible for creating new BlockTypes
    /// </summary>
    public class BlockTypeFactory
    {
        public List<BlockType> TypeList = new List<BlockType>();

        public BlockType CreateNewBlockType(IMesh mesh)
        {
            BlockType ret = new BlockType(mesh);
            TypeList.Add(ret);
            return ret;
        }
    }
}
