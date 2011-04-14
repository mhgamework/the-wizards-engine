using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TreeGenerator
{
    class TreeFlowerType
    {
        public Range FlowerAxialSplit = new Range(0, 0);
        public Range FlowerDropAngle = new Range(0, 0);
        


        public TreeLeafType FlowerLeafTypeOpen=new TreeLeafType();
        
        public TreeLeafType FlowerleafTypeClosed=new TreeLeafType();

        public int FlowerLeafCount = 4;
      
        //flowerHeart
        public Range FlowerHeartDiameter = new Range(1,1);/// for now only circularish
        public Range FlowerHeartLength = new Range(2, 2);///distance of the cone that goes from the trunk to the flower
        //pistill and stamen these are represented by an image rendered on a face like the grass
        public Range PistillFaceCount = new Range(1, 1);
        public Range StamenFaceCount = new Range(1, 1);
        public Range PistillFaceDimentionsWidth = new Range(1, 1);
        public Range PistillFaceDimentionsLength = new Range(1, 1);
        public Range StamenFaceDimentionsWidth = new Range(1, 1);
        public Range StamenFaceDimentionsLength = new Range(1, 1);
        public Range PistillDistanceFromCenter = new Range(0, 0);
        public Range StamenDistanceFromCenter = new Range(0.1f, 0.1f);
    }
}
