using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;

namespace MHGameWork.TheWizards
{
    /// <summary>
    /// Static class, containing functions that can be used to manipulate a QuadTree structure, build up from IQuadTreeNode objects.
    /// </summary>
    public static class QuadTree
    {
        //       private ulong address;
        //protected QuadTreeNode upperLeft;
        //protected QuadTreeNode upperRight;
        //protected QuadTreeNode lowerLeft;
        //protected QuadTreeNode lowerRight;
        //protected QuadTreeNode parent;
        //protected BoundingBox boundingBox = new BoundingBox();
        //protected bool isStatic = false;
        //protected Common.GeoMipMap.TerrainBlock terrainBlock;

        //public enum ChildDir
        //{
        //    UpperLeft = 0,
        //    UpperRight = 1,
        //    LowerLeft = 2,
        //    LowerRight = 3
        //}

        //public QuadTreeNode()
        //    : base()
        //{
        //    //UpdateAdress();
        //}

        //~QuadTreeNode()
        //{
        //    Dispose( false );
        //}


        //public void Dispose()
        //{
        //    Dispose( true );
        //    GC.SuppressFinalize( this );
        //}

        //protected virtual void Dispose( bool disposing )
        //{
        //    lock ( this )
        //    {
        //        if ( disposing )
        //        {
        //            if ( upperLeft != null )
        //                upperLeft.Dispose();

        //            if ( upperRight != null )
        //                upperRight.Dispose();

        //            if ( lowerLeft != null )
        //                lowerLeft.Dispose();

        //            if ( lowerRight != null )
        //                lowerRight.Dispose();

        //            if ( parent != null )
        //                parent.RemoveChild( this );

        //        }

        //        upperLeft = null;
        //        upperRight = null;
        //        lowerLeft = null;
        //        lowerRight = null;
        //        parent = null;
        //    }

        //}


        public static void Split<T>(T node) where T : class, IQuadTreeNode<T>
        {
            if (node.NodeData.UpperLeft != null && node.NodeData.UpperRight != null && node.NodeData.LowerLeft != null && node.NodeData.LowerRight != null) return;
            Vector3 half = new Vector3(node.NodeData.BoundingBox.Maximum.X - node.NodeData.BoundingBox.Minimum.X, 0, node.NodeData.BoundingBox.Maximum.Z - node.NodeData.BoundingBox.Minimum.Z);
            half *= 0.5f;
            half.Y = node.NodeData.BoundingBox.Maximum.Y - node.NodeData.BoundingBox.Minimum.Y;

            //NOTE: This is a heavy operation, since it copies the nodeData to the stack, and later in the code back to the heap.
            // Performance should be verified.
            QuadTreeNodeData<T> nodeData = node.NodeData; // WARNING: THIS IS A HEAVY OPERATION
            QuadTreeNodeData<T> childData = new QuadTreeNodeData<T>();
            Vector3 min;
            childData.Parent = node;

            if (node.NodeData.UpperLeft == null)
            {
                min = nodeData.BoundingBox.Minimum + new Vector3(0, 0, 0);
                childData.BoundingBox = new BoundingBox(min, min + half);
                nodeData.UpperLeft = node.CreateChild(childData);
                nodeData.UpperLeft.NodeData = childData;
            }
            if (node.NodeData.UpperRight == null)
            {
                min = nodeData.BoundingBox.Minimum + new Vector3(half.X, 0, 0);
                childData.BoundingBox = new BoundingBox(min, min + half);
                nodeData.UpperRight = node.CreateChild(childData);
            } nodeData.UpperRight.NodeData = childData;

            if (node.NodeData.LowerLeft == null)
            {
                min = nodeData.BoundingBox.Minimum + new Vector3(0, 0, half.Z);
                childData.BoundingBox = new BoundingBox(min, min + half);
                nodeData.LowerLeft = node.CreateChild(childData);
                nodeData.LowerLeft.NodeData = childData;
            }
            if (node.NodeData.LowerRight == null)
            {
                min = nodeData.BoundingBox.Minimum + new Vector3(half.X, 0, half.Z);
                childData.BoundingBox = new BoundingBox(min, min + half);
                nodeData.LowerRight = node.CreateChild(childData);
                nodeData.LowerRight.NodeData = childData;
            }

            node.NodeData = nodeData;

            /*upperLeft.UpdateAdress();
            upperRight.UpdateAdress();
            lowerLeft.UpdateAdress();
            lowerRight.UpdateAdress();*/

        }
        /// <summary>
        /// Splits given node recursively 'numLevels' times
        /// </summary>
        public static void Split<T>(T node, int numLevels) where T : class, IQuadTreeNode<T>
        {
            numLevels--;
            if (numLevels < 0) return;
            Split(node);

            Split(node.NodeData.UpperLeft, numLevels);
            Split(node.NodeData.UpperRight, numLevels);
            Split(node.NodeData.LowerLeft, numLevels);
            Split(node.NodeData.LowerRight, numLevels);

        }

        public static void Merge<T>(IQuadTreeNode<T> node) where T : class, IQuadTreeNode<T>
        {
            QuadTreeNodeData<T> nodeData = node.NodeData;
            /*if (nodeData.UpperLeft != null)
                upperLeft.Dispose();

            if (nodeData.UpperRight != null)
                upperRight.Dispose();

            if (nodeData.LowerLeft != null)
                lowerLeft.Dispose();

            if (nodeData.LowerRight != null)
                lowerRight.Dispose();*/

            nodeData.UpperLeft = null;
            nodeData.UpperRight = null;
            nodeData.LowerLeft = null;
            nodeData.LowerRight = null;

            node.NodeData = nodeData;
        }

        public static void MergeRecursive<T>(T node) where T : class, IQuadTreeNode<T>
        {


            Merge(node.NodeData.UpperLeft);
            Merge(node.NodeData.UpperRight);
            Merge(node.NodeData.LowerLeft);
            Merge(node.NodeData.LowerRight);

            Merge(node);

        }

        public static T GetRootNode<T>(T node) where T : class, IQuadTreeNode<T>
        {
            while (node.NodeData.Parent != null)
                node = node.NodeData.Parent;

            return node;
        }



        //protected virtual void RemoveChild( QuadTreeNode child )
        //{
        //    if ( upperLeft == child )
        //    {
        //        upperLeft = null;
        //    }
        //    if ( upperRight == child )
        //    {
        //        upperRight = null;
        //    }
        //    if ( lowerLeft == child )
        //    {
        //        lowerLeft = null;
        //    }
        //    if ( lowerRight == child )
        //    {
        //        lowerRight = null;
        //    }
        //}


        //public BoundingBox MergeBoundingBoxes( BoundingBox box1, BoundingBox box2 )
        //{
        //    if ( box1.Min == box1.Max )
        //    {
        //        return box2;
        //    }
        //    else if ( box2.Min == box2.Max )
        //    {
        //        return box1;
        //    }
        //    else
        //    {
        //        return BoundingBox.CreateMerged( box1, box2 );
        //    }
        //}



        //public virtual string BuildString()
        //{
        //    return BuildString( 0 );
        //}
        //public virtual string BuildString( int indentLenght )
        //{
        //    const int indentStep = 3;
        //    string indent = "";
        //    string ret = "";
        //    for ( int i = 0; i < indentLenght; i++ )
        //    {
        //        indent += " ";
        //    }
        //    //ret += indent + "Node Entities:" + entities.Count.ToString();
        //    if ( upperLeft != null )
        //    {
        //        ret += Environment.NewLine + upperLeft.BuildString( indentLenght + indentStep );
        //    }
        //    if ( upperRight != null )
        //    {
        //        ret += Environment.NewLine + upperRight.BuildString( indentLenght + indentStep );
        //    }
        //    if ( lowerLeft != null )
        //    {
        //        ret += Environment.NewLine + lowerLeft.BuildString( indentLenght + indentStep );
        //    }
        //    if ( lowerRight != null )
        //    {
        //        ret += Environment.NewLine + lowerRight.BuildString( indentLenght + indentStep );
        //    }

        //    return ret;
        //}

        ////public override string ToString()
        ////{
        ////    string ret = "";

        ////    ret += "Node ";

        ////    ret += "Level: " + CalculateLevel().ToString() + " ";
        ////    if ( parent != null )
        ////    {
        ////        if ( parent.upperLeft == this ) ret += "UpperLeft ";
        ////        if ( parent.upperRight == this ) ret += "UpperRight ";
        ////        if ( parent.lowerLeft == this ) ret += "LowerLeft ";
        ////        if ( parent.lowerRight == this ) ret += "LowerRight ";
        ////    }
        ////    if ( IsLeaf ) ret += "Leaf ";
        ////    //if ( entities.Count > 0 ) ret += "Entities: " + entities.Count.ToString() + " ";


        ////    return ret;
        ////}
        //public override string ToString()
        //{
        //    return "Node " + NodeAddressToText( address );
        //}


        //public QuadTreeNode FindOrCreateNode( ulong nAddress )
        //{
        //    int level = GetLevel( this );
        //    int targetLevel = GetLevel( nAddress );

        //    if ( level < targetLevel )
        //    {
        //        //Need to go deeper.

        //        //Split this node. If this node allready has children then the call we be omitted.
        //        Split();

        //        return GetChild( GetChildDirAtLevel( nAddress, level + 1 ) ).FindOrCreateNode( nAddress );


        //    }
        //    else if ( level == targetLevel )
        //    {
        //        if ( address == nAddress )
        //        {
        //            //Yes!
        //            return this;
        //        }
        //        else
        //        {
        //            //Oo, error
        //            throw new InvalidOperationException();
        //        }
        //    }
        //    else
        //    {
        //        //level > targetLevel

        //        throw new InvalidOperationException( "The node you are looking for is not a child of this node!! (or other issue)" );
        //    }

        //}


        public static int CalculateLevel<T>(IQuadTreeNode<T> node) where T : IQuadTreeNode<T>
        {
            if (node.NodeData.Parent == null) return 0;

            return CalculateLevel(node.NodeData.Parent) + 1;
        }

        //public QuadTreeNode GetChild( ChildDir dir )
        //{
        //    switch ( dir )
        //    {
        //        case ChildDir.UpperLeft:
        //            return upperLeft;
        //            break;
        //        case ChildDir.UpperRight:
        //            return upperRight;
        //            break;
        //        case ChildDir.LowerLeft:
        //            return lowerLeft;
        //            break;
        //        case ChildDir.LowerRight:
        //            return lowerRight;
        //            break;
        //        default:
        //            throw new ArgumentException( "Invalid value", "dir" );
        //    }

        //}

        //public ChildDir GetChildDirAtLevel( ulong address, int level )
        //{
        //    if ( level == 0 ) throw new ArgumentException( "Value cannot be 0", "level" );
        //    return (ChildDir)( ( address >> ( 4 + 2 * ( level - 1 ) ) ) & ( ( 1 << 0 ) + ( 1 << 1 ) ) );
        //}

        //public int GetLevel( QuadTreeNode node )
        //{
        //    return GetLevel( node.Address );
        //}

        //public int GetLevel( ulong address )
        //{
        //    //First four bytes of address
        //    return (int)( address & ( ( 1 << 0 ) + ( 1 << 1 ) + ( 1 << 2 ) + ( 1 << 3 ) ) );
        //}

        //public void UpdateAdress()
        //{
        //    address = CalculateAddress();
        //}

        //public ulong CalculateAddress()
        //{
        //    // Het adres
        //    // bit 0-3 : level van de node
        //    // bit 4-63 : Welke childnode het is, telkens 2 bits per level
        //    // 00 = upperLeft
        //    // 01 = upperRight
        //    // 10 = lowerLeft
        //    // 11 = lowerRight
        //    if ( parent == null ) return 0;

        //    //TODO: power operator wont work

        //    ulong levelBitmask = ( 1 << 0 ) + ( 1 << 1 ) + ( 1 << 2 ) + ( 1 << 3 );

        //    ulong parentLevel = parent.address & levelBitmask;

        //    ulong ret;

        //    ret = parent.address & ~levelBitmask;
        //    //Ret now contains the parents address with the 4 lvl bits set to 0
        //    //Now set this nodes lvl
        //    ret = ret | ( parentLevel + 1 );

        //    //Add the 2 bits at the end saying which child node this one is
        //    ulong childMask;
        //    if ( parent.upperLeft == this )
        //    {
        //        childMask = 0; //00
        //    }
        //    else if ( parent.upperRight == this )
        //    {
        //        childMask = 1; //01
        //    }
        //    else if ( parent.lowerLeft == this )
        //    {
        //        childMask = 2; //10
        //    }
        //    else if ( parent.lowerRight == this )
        //    {
        //        childMask = 3; //11
        //    }
        //    else
        //    {
        //        throw new Exception( "Impossible!!!" );
        //    }

        //    ret = ret | ( childMask << ( 4 + 2 * (int)( parentLevel + 1 - 1 ) ) );

        //    //string temp = TempULongToBitsString( ret );
        //    //string path = NodeAddressToText( ret );


        //    return ret;


        //}

        //public string NodeAddressToText( ulong address )
        //{
        //    string text = "";
        //    //Get address: first four bits of the address
        //    int level = (int)( address & ( ( 1 << 0 ) + ( 1 << 1 ) + ( 1 << 2 ) + ( 1 << 3 ) ) );

        //    text += "Level: " + level.ToString() + " Root";

        //    //Get the location of the node for each lvl

        //    ulong mask = ( 1 << 0 ) + ( 1 << 1 );

        //    for ( int iLevel = 1; iLevel <= level; iLevel++ )
        //    {
        //        string locationName;
        //        //int location = (int)( address & ( mask << ( 4 + 2 * ( iLevel - 1 ) ) ) );
        //        int location = (int)( ( address >> ( 4 + 2 * ( iLevel - 1 ) ) ) & mask );
        //        switch ( location )
        //        {
        //            case 0:
        //                locationName = "UpperLeft";
        //                break;
        //            case 1:
        //                locationName = "UpperRight";
        //                break;
        //            case 2:
        //                locationName = "LowerLeft";
        //                break;
        //            case 3:
        //                locationName = "LowerRight";
        //                break;
        //            default:
        //                throw new Exception( "Impossible" );
        //        }

        //        text += "." + locationName;


        //    }
        //    return text;
        //}


        //public string TempULongToBitsString( ulong val )
        //{
        //    string ret = "";
        //    for ( int i = 0; i < 64; i++ )
        //    {
        //        ret = ( ( val & ( (ulong)1 << i ) ) > 0 ? "1" : "0" )
        //            + ret;
        //    }
        //    return ret;
        //}

        //public QuadTreeNode UpperLeft
        //{
        //    get { return upperLeft; }
        //    set { upperLeft = value; }
        //}

        //public QuadTreeNode UpperRight
        //{
        //    get { return upperRight; }
        //    set { upperRight = value; }
        //}

        //public QuadTreeNode LowerLeft
        //{
        //    get { return lowerLeft; }
        //    set { lowerLeft = value; }
        //}

        //public QuadTreeNode LowerRight
        //{
        //    get { return lowerRight; }
        //    set { lowerRight = value; }
        //}

        //public QuadTreeNode Parent
        //{
        //    get { return parent; }
        //    set { parent = value; }
        //}

        //public BoundingBox BoundingBox
        //{
        //    get { return boundingBox; }
        //    set { boundingBox = value; }
        //}

        //public ulong Address
        //{
        //    get { return address; }
        //    set { address = value; }
        //}

        /// <summary>
        /// Returns true if this node doesn't have any children, otherwise false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsLeafNode<T>(IQuadTreeNode<T> node) where T : class, IQuadTreeNode<T>
        {
            //Optimization: partial splits are not allowd currently
            return node.NodeData.LowerLeft == null;
            //TODO: needs speedup?
            /*return (node.NodeData.UpperLeft == null)
                && (node.NodeData.UpperRight == null)
                && (node.NodeData.LowerLeft == null)
                && (node.NodeData.LowerRight == null);*/

        }


        ///// <summary>
        ///// TODO: static and dynamic entities
        ///// </summary>
        //public bool Static
        //{
        //    get { return isStatic || ( terrainBlock != null ); }
        //}
        //public void SetStatic( bool nIsStatic )
        //{
        //    isStatic = nIsStatic;
        //}


        //public bool CanMerge
        //{
        //    get
        //    {
        //        if ( UpperLeft.IsLeaf
        //            && UpperRight.IsLeaf
        //            && LowerLeft.IsLeaf
        //            && LowerRight.IsLeaf
        //            && !UpperLeft.Static
        //            && !UpperRight.Static
        //            && !LowerLeft.Static
        //            && !LowerRight.Static )
        //            return true;
        //        else
        //            return false;

        //    }
        //}

        //public Common.GeoMipMap.TerrainBlock TerrainBlock
        //{
        //    get { return terrainBlock; }
        //    set
        //    { terrainBlock = value; }
        //}


        //#region IQuadtreeNode Members


        //public IQuadtreeNode GetIChild( QuadtreeChildDirection childDir )
        //{
        //    throw new Exception( "The method or operation is not implemented." );
        //}

        //#endregion
    }
}
