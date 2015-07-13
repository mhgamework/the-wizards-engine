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
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.Graphics;

namespace TreeGenerator.help
{
    public class Segment
    {

        public Branch parentBranch;
        int numVertices = 11;
        public List<TangentVertex> vertices = new List<TangentVertex>();
        public List<TangentVertex> Vertices
        {
            get { return vertices; }
        }
        public float AxialSplit;
        public float DropAngle;
        public float AbsoluteAxialSplit;
        public float AbsoluteDropAngle;
        public Vector3 Position;
        //public float WobbleAxialSplit;
        //public float WobbleDropAngle;
        public float Diameter;
        public Segment ParentSegment;
        public Directions directions;

        public Matrix TotalRotation;

        public Segment( Branch _parentBranch, Vector3 _position, float _dropAngle, float _AxialSplit, float _Diameter, Directions _directions )
        {
            parentBranch = _parentBranch;
            Position = _position;
            DropAngle = _dropAngle;
            AxialSplit = _AxialSplit;
            Diameter = _Diameter;
            directions = _directions;
        }



        public void CreateVertices()
        {
            vertices.Clear();
            numVertices = parentBranch.ParentTree.NumVertices;
            float r = Diameter / 2;
            double anglePerVer = ( 2 * MathHelper.Pi ) / ( numVertices - 1 );
            vertices.Add( new TangentVertex( Vector3.Zero, new Vector2( 0.5f, 0.5f ), Vector3.Up, new Vector3( 0, 0, 1 ) ) );
            double angleVert = 0;
            float V;

            if ( ParentSegment == null )
            {
                V = 0;
            }
            else
            {
                V = ParentSegment.vertices[ 1 ].V + 1;

            }
            for ( int i = 1; i < numVertices; i++ )
            {
                float U;
                U = i * 0.5f;
                vertices.Add( new TangentVertex( new Vector3( (float)( r * Math.Sin( angleVert ) ), 0.0f, (float)( r * Math.Cos( angleVert ) ) ), new Vector2( U, V ), new Vector3( (float)( Math.Sin( angleVert ) ), 0.0f, (float)( Math.Cos( angleVert ) ) ), new Vector3( (float)( Math.Cos( angleVert ) ), 0.0f, (float)( -Math.Sin( angleVert ) ) ) ) );
                angleVert += anglePerVer;

            }


            Vector3 axisDropAngle;
            Vector3 axisAxialSplit;
            axisAxialSplit = directions.Heading;
            axisDropAngle = directions.Right;


            // These drop angles are in the space of the parent segment
            //float segmentDropAngle;
            //float segmentAxialSplit;

            Matrix mRot;
            //Directions parentDirections;
            if ( ParentSegment != null )
            {

                if ( ParentSegment == parentBranch.ParentSegment )
                {
                    // This is the first segment of this branch that is not the parent segment. 
                    // it does not uses the parent segment but the branch-orientation instead
                    //parentDirections = parentBranch.BranchDirections;
                    //segmentDropAngle = DropAngle * 0.5f;
                    //segmentAxialSplit = AxialSplit * 0.5f;

                    //mRot = Matrix.CreateFromAxisAngle( directions.Right, parentBranch.DropAngle );
                    mRot = Matrix.CreateFromAxisAngle( parentBranch.BranchDirections.Right, parentBranch.DropAngle );

                }
                else
                {
                    //mRot = Matrix.CreateFromAxisAngle( directions.Right, DropAngle );
                    mRot = Matrix.CreateFromAxisAngle( ParentSegment.directions.Right, ParentSegment.DropAngle );
                    //parentDirections = ParentSegment.directions;
                    //segmentDropAngle = DropAngle * 0.5f;
                    //segmentAxialSplit = AxialSplit * 0.5f;
                }


                mRot = ParentSegment.TotalRotation * mRot;
            }
            else
            {
                // This is the FIRST segment of the whole tree
                //segmentDropAngle = DropAngle;
                //segmentAxialSplit = AxialSplit;
                //parentDirections = directions;
                mRot = Matrix.Identity;


            }

            TotalRotation = mRot;

            // Add half the local drop to get smoother branches
            mRot = mRot * Matrix.CreateFromAxisAngle( directions.Right, DropAngle );
            

            //segmentDropAngle = DropAngle * 1.0f;
            //segmentAxialSplit = AxialSplit * 1.0f;

            //Vector3 segmentRight = Vector3.Transform(parentDirections.Right, Matrix.CreateFromAxisAngle(parentDirections.Heading, segmentAxialSplit));// soùething id wrong poistion????


            //Vector3 tempRight = Vector3.Cross(parentDirections.Heading, directions.Heading);
            //segmentDropAngle = (float)Math.Acos(Vector3.Dot(parentDirections.Heading, directions.Heading)) * 0.5f;
            //Matrix mRot = Matrix.CreateFromAxisAngle(tempRight, segmentDropAngle);


            //Matrix mRot = Matrix.CreateFromAxisAngle(axisDropAngle, segmentDropAngle);
            Matrix m = mRot * Matrix.CreateTranslation( Position );
            for ( int i = 0; i < vertices.Count; i++ )
            {
                TangentVertex v = vertices[ i ];

                v.pos = Vector3.Transform( v.pos, m );
                v.tangent = Vector3.Transform( v.tangent, mRot );
                v.normal = Vector3.Transform( v.normal, mRot );
                vertices[ i ] = v;

            }
        }


    }
























    #region oud
    //List<Branch> childBranches = new List<Branch>();

    //public List<Branch> ChildBranches
    //{
    //    get { return childBranches; }
    //}




    //public List<Vector3> Vertices
    //{
    //    get { return vertices; }
    //    set { vertices = value; }
    //}

    //float diameter;
    //private Vector2 angle;

    //public Vector2 Angle
    //{
    //    get { return angle; }
    //}
    ////private Vector3 position;

    ////public Vector3 Position
    ////{
    ////    get { return position; }
    ////    //set { position = value; }
    ////}



    //public void AddChildBranch(Vector2 angle, float minDiameter, float maxDiameter, int numSegments, float length) //todo 
    //{

    //    Branch child = null;//new Branch(this);
    //    child.CreateSegmentsOud(length, minDiameter, maxDiameter, angle, numSegments);
    //    //child.rotate(direction);
    //    //child.Move(this.position);
    //    //child.Segments[0].Vertices = vertices;// I don't know if this works

    //    childBranches.Add(child);
    //}

    //public void CreateVertices(int _numVert)
    //{
    //    numVertices = _numVert;
    //    float r = diameter / 2;
    //    double anglePerVer = (2 * MathHelper.Pi) / (numVertices - 1);
    //    vertices = new List<Vector3>();
    //    vertices.Add(Vector3.Zero);
    //    double angleVert = 0;
    //    for (int i = 1; i < numVertices; i++)
    //    {
    //        vertices.Add(new Vector3((float)(r * Math.Sin(angleVert)), 0.0f, (float)(r * Math.Cos(angleVert))));
    //        angleVert += anglePerVer;

    //    }

    //    //Quaternion rotation = dqf; //TODO //CreateRotationBetweenVectors(new Vector3(0, 0, 1), direction);

    //    Matrix m = Matrix.CreateRotationX(angle.X) * Matrix.CreateRotationZ(angle.Y) * Matrix.CreateTranslation(position);// need to be fixed
    //    for (int i = 0; i < vertices.Count; i++)
    //    {
    //        vertices[i] = Vector3.Transform(vertices[i], m);
    //    }



    //    /*Rotate();
    //    Move();*/
    //}

    #region brol
    /*public void Move(Vector3 position)
        {
            position += position;
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] += position;
            }
        }
        public void Move()
        {
            Vector3 pos = position;
            Move(this.position);
            position = pos;
        }

        public void Rotate(Vector3 Direction)
        {
            Quaternion rotation = CreateRotationBetweenVectors(new Vector3(0, 0, 1), Direction);
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = Vector3.Transform(vertices[i], rotation);
            }
        }
        public void Rotate()
        {
            Rotate(this.direction);
        }

        public static Quaternion CreateRotationBetweenVectors(Vector3 original, Vector3 target)
        {
            original.Normalize();
            target.Normalize();
            // This of course can be faster, but the coding is fastest this way!
            // First get rotation from original to forward , then from target to forward, then mix!

            Quaternion qOri = CreateFromLookDir(original);
            Quaternion qTarget = CreateFromLookDir(target);

            // F * A = O
            // F * B = T
            // O * X = T
            // F * A * X = F * B
            // X = A^(-1) * B

            // Quaternion total = qTarget * Quaternion.Inverse(qOri);

            // Dus, natuurlijk werkt dit niet. Maar gelukkig blijkt het volgende te werken

            Quaternion total = qTarget * Quaternion.Inverse(qOri);

#if (DEBUG)
            Vector3 newDir = Vector3.Transform(original, total);
            if (!VectorsEqual(newDir, target)) throw new Exception("This algoritm doesnt work!");
#endif

            return total;
        }

        /// <summary>
        /// Returns a rotation quaternion that makes the camera in its current position
        /// look at a given point.
        /// </summary>
        /// <returns></returns>
        public static Quaternion CreateFromLookDir(Vector3 dir)
        {
            dir.Normalize();

            // now some magic trigoniometry
            // TODO: This probably can be done faster

            // dir.y ^ 2 + radius ^ 2 = dir.lengthsquared
            float radius = (float)Math.Sqrt(1 - dir.Y * dir.Y);

            float angleY;
            float angleX;
            if (radius < 0.0001)
            {
                if (dir.Y > 0)
                {
                    angleY = 0;
                    angleX = MathHelper.PiOver2;
                }
                else
                {
                    angleY = 0;
                    angleX = -MathHelper.PiOver2;
                }

            }
            else
            {

                angleY = (float)Math.Acos(MathHelper.Clamp(-dir.Z / radius, -1, 1));
                angleX = (float)Math.Asin(MathHelper.Clamp(dir.Y, -1, 1));



                if (dir.X > 0) angleY = -angleY;
                //if ( dir.Z > 0 && dir.Y > 0 ) angleX = angleX + MathHelper.PiOver2;
                //if ( dir.Z > 0 && dir.Y < 0 ) angleX = angleX - MathHelper.PiOver2;

            }

            Quaternion q;
            q = Quaternion.CreateFromYawPitchRoll(angleY, angleX, 0);

            //Since i was stupid enough to design this algoritm with the base vector vector.Right
            // i need to add this line to get vector3.forward
            //q = q * Quaternion.CreateFromAxisAngle( Vector3.Up, -MathHelper.PiOver2 );

#if (DEBUG)
            Vector3 newDir = Vector3.Transform(Vector3.Forward, q);
            if (!VectorsEqual(dir, newDir)) throw new Exception("This algoritm doesnt work!");
#endif
            return q;
        }

        private static bool VectorsEqual(Vector3 v1, Vector3 v2)
        {
            Vector3 diff = v1 - v2;
            if (Math.Abs(diff.X) > 0.01) return false;
            if (Math.Abs(diff.Y) > 0.01) return false;
            if (Math.Abs(diff.Z) > 0.01) return false;

            return true;
        }*/
    #endregion

    #endregion
}



