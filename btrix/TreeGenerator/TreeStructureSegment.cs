using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using TreeGenerator.help;

namespace TreeGenerator
{
    public class TreeStructureSegment
    {
        //public TreeStructureBranch Parent;

        //TODO: maybe this is better
        //public TreeStructureBranchSegment PositionSegment;
        private List<TreeStructureSegment> children = new List<TreeStructureSegment>();
        public List<TreeStructureSegment> Children
        {
            get { return children; }
        }

        //public float Percentage;

        public Directions Direction;
        public float Length;
        /// <summary>
        /// Radius in het begin van de segment.
        /// </summary>
        public float Radius;

        public LevelAndTexureData LevelTextureData;
        /// <summary>
        /// Returns the newly created segment
        /// </summary>
        /// <param name="location">Factor between 0 and 1 where 0 is the start of this segment</param>
        /// <returns></returns>
        public TreeStructureSegment Split(float location)
        {
            TreeStructureSegment seg;
            seg = new TreeStructureSegment();
            seg.Length = Length * (1 - location);
            seg.children = children;
            seg.Direction = Direction;
            seg.Radius = CalculateRadius(location + (1 - location) * 0.5f);
            seg.LevelTextureData = this.LevelTextureData;
            Length = Length * location;
            children = new List<TreeStructureSegment>();
            children.Add(seg);
            Radius = CalculateRadius(location * 0.5f);

            return seg;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location">Factor between 0 and 1 where 0 is the start of this segment</param>
        public float CalculateRadius(float location)
        {
            if (children.Count == 0)
            {
                return Radius;
            }
            if (children.Count > 1)
            {
                //TODO: Mss oppervlakte hier gebruiken...
                return Radius;
            }

           return MathHelper.Lerp(this.Radius, this.Children[0].Radius, location);
        }

        //not sure yet

        public List<TreeStructureLeaf> Leaves=new List<TreeStructureLeaf>();


        //public int LevelIndex;

    }


    public struct LevelAndTexureData
    {
        public int Level ;
        public int TextureIndex ;
        public LevelAndTexureData(int _level,int _texturendex)
        {
            Level = _level;
            TextureIndex = _texturendex;
        }
    }

}
