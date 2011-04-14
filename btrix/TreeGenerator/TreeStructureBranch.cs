using System;
using System.Collections.Generic;
using System.Text;

namespace TreeGenerator
{
    public class TreeStructureBranch
    {
        //public TreeStructureBranch Parent;

        //TODO: maybe this is better
        //public TreeStructureBranchSegment PositionSegment;
        public float RelativePosition;

        public List<TreeStructureBranch> Branches = new List<TreeStructureBranch>();
        public List<TreeStructureLeaf> Leaves = new List<TreeStructureLeaf>();

        public List<TreeStructureBranchSegment> Segments = new List<TreeStructureBranchSegment>();


    }
}
