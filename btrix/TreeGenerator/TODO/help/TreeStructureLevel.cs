using System;
using System.Collections.Generic;
using System.Text;

namespace TreeGenerator.help
{
    public class TreeStructureLevel
    {

        public float
            DiameterBranchMax = 0.5f,
            DiameterBranchMin = 0.3f,
            AxialSplitBranchMax = 0,
            AxialSplitBranchMin = 0,
            DropAngleBranchMax = 0,
            DropAngleBranchesMin = 0,
            WobbleAxialSplitBranchMax = 0,
            WobbleAxialSplitBranchMin = 0,
            WobbleDropAngleBranchMax = 0,
            WobbleDropAngleBranchMin = 0,
            LengthBranchMax = 3,
            LengthBranchMin = 2,
            PositionRatioSubBranchMin = 2,
            PositionRatioSubBranchMax = 3,
            Spreading = 1,
            TaperMin = 0,
            TaperMax = 1.0f,
            BranchStartRatio = 0.5f,
            BranchEndRatio = 1f,
            BranchLengthDecrase = 0.5f;
        public int NumSegments = 3, NumBranchMax = 6, NumBranchMin = 2;


    }
}
