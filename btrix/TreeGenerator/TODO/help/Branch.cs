using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using TreeGenerator.help;

namespace TreeGenerator.help
{
    public class Branch
    {
        public Tree ParentTree;
        public Branch ParentBranch;
        public List<Branch> ChildBranches = new List<Branch>();
        public int NumSegments = 5;
        public int ParentSegmentIndex = -1;
        public float DropAngle = MathHelper.Pi;
        public float AxialSplit = 0;
        public float minDiameter = 1;
        public float maxDiameter = 1;
        public float length = 3;
        public Directions BranchDirections = new Directions(Vector3.Up, Vector3.UnitZ);
        public float
            WobbleAxialSplitBranchMax,
            WobbleAxialSplitBranchMin,
            WobbleDropAngleBranchMax,
            WobbleDropAngleBranchMin;
        public int seed = 123;
        public Seeder seeder;


        public List<Segment> Segments = new List<Segment>();


        public Segment ParentSegment
        {
            get
            {
                if (ParentBranch == null) return Segments[0];
                return ParentBranch.Segments[ParentSegmentIndex];
            }
            //set { myVar = value; }
        }


        private Branch(Branch parent)
        {
            ParentBranch = parent;
            if (ParentBranch != null) ParentTree = parent.ParentTree;
        }

        public Branch CreateChildBranch(int segmentIndex)
        {
            if (segmentIndex == 0) throw new InvalidOperationException("In this case you should be adding a child to this branches parent!");
            if (segmentIndex >= NumSegments) throw new InvalidOperationException("This branch doesn't have that many segments!");

            Branch b = new Branch(this);
            ChildBranches.Add(b);

            b.ParentSegmentIndex = segmentIndex;
            b.ParentBranch = this;
            return b;
        }

        public static Branch CreateTrunk(Tree tree)
        {
            Branch b = new Branch(null);
            b.ParentTree = tree;

            return b;

        }



        public void CreateSegments()
        {

            Segments.Clear();
            if (ParentBranch == null)
            {
                // This is the trunk!! Create the parent segment.
                Segment segment = new Segment(this, ParentTree.Position, DropAngle, AxialSplit, maxDiameter, BranchDirections);//todo make direction works
                Segments.Add(segment);

                BranchDirections = Directions.DirectionsFromAngles(new Directions(Vector3.Up, Vector3.UnitZ), DropAngle, AxialSplit);

            }
            else
            {
                Segments.Add(ParentBranch.Segments[ParentSegmentIndex]);
                BranchDirections = Directions.DirectionsFromAngles(ParentSegment.directions, DropAngle, AxialSplit);
                maxDiameter = ParentSegment.Diameter * .5f;

            }

            float segmentLength = length / (NumSegments - 1);

            for (int i = 1; i < NumSegments; i++)
            {

                Segment Segment;
                /*Segment parentSegment;
                if (parentSegment == null)
                {
                    parentSegment=ParentBranch.Segments[ParentBranch.ParentSegmentIndex];
                }*/
                float diameter = maxDiameter - ((maxDiameter - minDiameter) / length) * i * segmentLength;

                Vector3 parentPosition;
                Directions parentDirections;
                float parentAbsoluteDropAngle;
                float parentAbsoluteAxialSplit;

                if (i == 1)
                {
                    parentPosition = Segments[i - 1].Position;
                    parentDirections = BranchDirections;
                    parentAbsoluteDropAngle = ParentSegment.AbsoluteDropAngle + DropAngle;
                    parentAbsoluteAxialSplit = ParentSegment.AbsoluteAxialSplit + AxialSplit;
                }
                else
                {
                    parentPosition = Segments[i - 1].Position;
                    parentDirections = Segments[i - 1].directions;
                    parentAbsoluteDropAngle = Segments[i - 1].AbsoluteDropAngle;
                    parentAbsoluteAxialSplit = Segments[i - 1].AbsoluteAxialSplit;
                }
                seeder = new Seeder(seed + i);
                Vector3 position = parentPosition + parentDirections.Heading * segmentLength;
                Segment = new Segment(this, position, 0.0f, 0.0f, diameter, new Directions());//todo make wobble thingy work
                Segment.DropAngle += seeder.NextFloat(WobbleDropAngleBranchMin, WobbleDropAngleBranchMax);
                Segment.AxialSplit += seeder.NextFloat(WobbleAxialSplitBranchMin, WobbleAxialSplitBranchMax);
                Segment.AbsoluteDropAngle = Segment.DropAngle + parentAbsoluteDropAngle;
                Segment.AbsoluteAxialSplit = Segment.AbsoluteAxialSplit + parentAbsoluteAxialSplit;
                Segment.directions = Directions.DirectionsFromAngles(parentDirections, Segment.DropAngle, Segment.AxialSplit);
                Segment.ParentSegment = Segments[i - 1];
                Segments.Add(Segment);
                //parentSegment = Segment;
            }
            for (int i = 0; i < ChildBranches.Count; i++)
            {
                ChildBranches[i].CreateSegments();

            }
        }

        public void CreateVertices()
        {
            if (this == ParentTree.Trunk)
            {
                for (int i = 0; i < NumSegments; i++)
                {
                    Segments[i].CreateVertices();
                }
            }
            else
            {
                for (int i = 1; i < NumSegments; i++)
                {
                    Segments[i].CreateVertices();
                }
            }
        }



        /*public static Directions CalculatedHeadingAndRight(Directions parentDirections, float _dropAngle, float _axialSplit)
                {
                    Directions d;
                    if (this.ParentBranch == null)
                    {
                        parentHeading = Vector3.Up;


                    }
                    else
                    {
                        parentHeading = ParentBranch.CalculateHeading();

                    }

                    right = Vector3.Transform(parentHeading, Matrix.CreateFromAxisAngle(parentHeading, _axialSplit));// soùething id wrong poistion????
                    heading = Vector3.Transform(parentHeading, Matrix.CreateFromAxisAngle(right, _dropAngle));
                    d.Heading = heading;
                    d.Right = right;

                    return d;
            
                }*/

        #region old1.0
        //public static Vector3 CalculateHeading(Vector3 parentHeading, float _dropAngle, float _axialSplit)
        //{
        //    /*Vector3 parentHeading;
        //    if (this.ParentBranch == null)
        //    {
        //        parentHeading = Vector3.Up;


        //    }
        //    else
        //    {
        //        parentHeading = ParentBranch.CalculateHeading();

        //    }*/

        //    //Vector3 headingAxialsplit = new Vector3(-(float)Math.Sin(_axialSplit), 0.0f, -(float)Math.Cos(_axialSplit));
        //    //Vector3 headingDropAngle = new Vector3(-(float)Math.Sin(_dropAngle), (float)Math.Cos(_dropAngle), 0.0f);

        //    Vector3 localY, localX, localZ;
        //    localY = parentHeading;
        //    localX = Vector3.Cross(localY, Vector3.UnitZ);
        //    localZ = Vector3.Cross(localY, localX);

        //    Vector3 partDropAngle = -(float)Math.Cos(_dropAngle) * parentHeading;
        //    Vector3 axisDropAngle = Vector3.Cross(parentHeading, -Vector3.UnitZ);
        //    Vector3 partAxisSplit = Vector3.Transform(axisDropAngle, Matrix.CreateFromAxisAngle(parentHeading, _axialSplit));
        //    partAxisSplit *= (float)Math.Sin(_dropAngle);
        //    Vector3 heading = partDropAngle + partAxisSplit;
        //    heading.Normalize();

        //    return heading;
        //}
        #endregion


        #region "Oud"
















        //private Vector2 angle;
        //public Vector2 Angle
        //{
        //    get { return angle; }
        //}

        ///*public Branch ParentBranch
        //{
        //    get { return ParentSegment.ParentBranch; }
        //    //set { myVar = value; }
        //}*/


        //public Segment ParentSegment
        //{
        //    get { return segmentsOud[0]; }
        //    //set { myVar = value; }
        //}

        //private List<Segment> segmentsOud;

        ///*public List<Segment> Segments
        //{
        //    get { return segments; }
        //    //set { segments = value; }
        //}*/



        //public Vector3 Position;

        ///// <summary>
        ///// This is the construtor for a trunk! DO NOT USE
        ///// </summary>
        //public Branch(Tree _parentTree, Vector3 _position)
        //{
        //    ParentTree = _parentTree;
        //    segmentsOud = new List<Segment>();
        //    Position = _position;
        //    //Segment segment = new Segment(_diameter, _direction, _position);
        //    //segments.Add(segment);

        //    //segments[0].CreateVertices(numVertices);
        //    //segments[0].Rotate();
        //    //segments[0].Move();
        //}


        ///*public Branch(Segment _parentSegment)
        //{
        //    ParentTree = _parentSegment.ParentBranch.ParentTree;
        //    segmentsOud = new List<Segment>();
        //    segmentsOud.Add(_parentSegment);
        //}*/


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="length"></param>
        ///// <param name="minDiameter"></param>
        ///// <param name="maxDiameter"></param>
        ///// <param name="numVertices"></param>
        ///// <param name="numSegments"></param>
        ///// 
        //public void CreateSegmentsOud(float length, float minDiameter, float maxDiameter, Vector2 angle, int numSegments)
        //{

        //    if (segmentsOud.Count == 0)
        //    {
        //        // This is the trunk!! Create the parent segment.
        //        Segment segment = new Segment(null, maxDiameter, angle, Position);
        //        segmentsOud.Add(segment);
        //        segmentsOud[0].CreateVertices(this.ParentTree.NumVertices);
        //    }

        //    float segmentLength = length / (numSegments - 1);

        //    for (int i = 1; i < numSegments; i++)
        //    {
        //        float diameter = maxDiameter - ((maxDiameter - minDiameter) / length) * i * segmentLength;

        //        Vector3 pos;
        //        Vector3 dir;
        //        dir = new Vector3((float)Math.Cos((double)angle.X), (float)Math.Sin((double)angle.Y), 0);
        //        pos = ParentSegment.Position + dir * (segmentLength * i);

        //        Segment segment = new Segment(this, diameter, angle, pos);

        //        segment.CreateVertices(ParentTree.NumVertices);

        //        /*segment.Move(ParentSegment.Position);
        //        segment.Rotate();
        //        segment.Move();*/
        //        segmentsOud.Add(segment);
        //    }
        //}

        ////public void Move(Vector3 position)
        ////{
        ////    for (int i = 0; i < segments.Count; i++)
        ////    {
        ////        segments[i].Move(position);
        ////    }
        ////}

        ////public void rotate(Vector3 dir)
        ////{
        ////    for (int i = 0; i < segments.Count; i++)
        ////    {
        ////        segments[i].Rotate(dir);
        ////    }
        ////}


        //void ConnectSegments()//todo connect all segments to each other ot form a cilinder
        //{

        //}


        ///// <summary>
        ///// still needs evaluation if needed
        ///// todo still has to be made
        ///// </summary>
        ///// <returns></returns>
        ///// 

        //public void ConnectToOtherBranch(Branch branch, bool begin_endThis, bool begin_endOther)
        //{
        //    if (begin_endThis)
        //    {
        //        if (begin_endOther)
        //        {

        //        }
        //        else
        //        {

        //        }
        //    }
        //    else
        //    {
        //        if (begin_endOther)
        //        {

        //        }
        //        else
        //        {

        //        }
        //    }
        //}

        #endregion
    }
}
