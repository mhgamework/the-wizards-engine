using System;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Graphics.Xna.Water
{
    public class SubGrid : IComparable<SubGrid>
    {
        public Mesh mesh;
        public Utils.BoundingBox box;
        public Vector3 camPos;

        public SubGrid[] children;
        public int totalIndices;

        private int mNumRows;
        private int mNumCols;
        private int mNumTris;
        private int mNumVerts;
        private bool mIsLeaf = false;

        #region Properties
        public int NumRows
        {
            get { return mNumRows; }
            set { mNumRows = value; }
        }

        public int NumCols
        {
            get { return mNumCols; }
            set { mNumCols = value; }
        }

        public int NumTris
        {
            get
            {
                if ( mNumTris < 0 )
                    mNumTris = ( mNumRows - 1 ) * ( mNumCols - 1 ) * 2;
                return mNumTris;
            }
            set { mNumTris = value; }
        }

        public int NumVerts
        {
            get
            {
                if ( mNumVerts < 0 )
                    mNumVerts = mNumRows * mNumCols;
                return mNumVerts;
            }
            set { mNumVerts = value; }
        }

        public bool IsLeaf
        {
            get { return mIsLeaf; }
            set { mIsLeaf = value; }
        }

        #endregion

        public SubGrid()
        {
            children = new SubGrid[ 4 ];

            mesh = null;

            mNumTris = -1;
            mNumVerts = -1;
        }

        public SubGrid( int numRows, int numCols )
        {
            children = new SubGrid[ 4 ];

            mesh = null;
            box = null;

            mNumRows = numRows;
            mNumCols = numCols;

            mNumTris = ( mNumRows - 1 ) * ( mNumCols - 1 ) * 2;
            mNumVerts = mNumRows * mNumCols;
            totalIndices = mNumVerts;
        }

        public static bool operator <( SubGrid left, SubGrid right )
        {
            Vector3 dist1 = left.box.Center - left.camPos;
            Vector3 dist2 = right.box.Center - left.camPos;

            return dist1.LengthSquared() < dist2.LengthSquared();
        }

        public static bool operator >( SubGrid left, SubGrid right )
        {
            Vector3 dist1 = left.box.Center - left.camPos;
            Vector3 dist2 = right.box.Center - left.camPos;

            return dist1.LengthSquared() > dist2.LengthSquared();
        }

        int System.IComparable<SubGrid>.CompareTo( SubGrid right )
        {
            if ( this < right )
                return -1;
            else if ( this > right )
                return 1;
            else
                return 0;
        }

        public const int NUM_ROWS = 33;
        public const int NUM_COLS = 33;
        public const int NUM_TRIS = ( NUM_ROWS - 1 ) * ( NUM_COLS - 1 ) * 2;
        public const int NUM_VERTS = NUM_ROWS * NUM_COLS;
    }
}
