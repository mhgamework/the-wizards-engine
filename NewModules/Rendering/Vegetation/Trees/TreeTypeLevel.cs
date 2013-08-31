using System.Collections.Generic;
using DirectX11;
using SlimDX;


namespace MHGameWork.TheWizards.Rendering.Vegetation.Trees
{
    public class TreeTypeLevel
    {

        // Branch

        /// <summary>
        /// Number of branches of this level
        /// </summary>
        public Range BranchCount=new Range(1,1);

        public RangeSpreading BranchPositionFactor=new RangeSpreading(0,1,0.5f);

        public Range BranchDropAngle = new Range(MathHelper.PiOver4*0.5f, MathHelper.PiOver4);
        public Range BranchWobbleDropAngle=new Range(0,0);
        public Range BranchWobbleAxialSplit=new Range(0,0);
        
        public Range BranchLength=new Range(3,5);
        /// <summary>
        /// This determines how much the length of this levels branches decrease when 
        /// nearing the end of the parent branch
        /// </summary>
        public float BranchLengthDecrease=0.1f;
        public Range BranchEndDiameterFactor=new Range(0.45f,0.50f) ;
        public float BranchMaxSegmentLength=0.2f;

        // Nog twijfelachtig
        public Range BranchStartDiameterFactor=new Range(0.85f,0.85f);
        public RangeSpreading BranchAxialSplit= new RangeSpreading(0,MathHelper.TwoPi,0.5f);
       
        //more treelike parameters
        public float BranchBendingStrenght = 0.00f;//how much it minimum adds in radians(axialsplit)
        public float BranchBendingFlexibility = 0.0f;//vamue between 0 and x with 0 the samestrength in the end like in the beginning and x the amount stronger than in the beginning

        //more treelike parameters as in speedtree
        public bool Steps = true;
        public float StepsPerMeter =1f;
        public float BranchDistributionPercentage = 0.50f;
        public Range BranchStepSpreading = new Range(-0.5f, 0.5f);

        public Range BranchLenghtDegradation = new Range(0.5f, 0.5f);

        public List<TreeLeafType> LeafType=new List<TreeLeafType>();
        
        // doesn't really belong here but I don't know how else I would do it
        public Vector2 UVMap = new Vector2(3.30f, 0.2f);


        //later wind animation perhaps
        public float Strenght;


    }
}
