using System.Collections.Generic;

namespace MHGameWork.TheWizards.Rendering.Vegetation.Trees
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
