using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.TWClient
{
    public class QuadTreeNode<T> : IDisposable  //: Common.Wereld.QuadTreeNode
    {

        public T Item;
      
        public QuadTree<T> Quadtree;

        public QuadTreeNode( QuadTree<T> tree )
            : base()
        {
            Quadtree = tree;
        }

        ~QuadTreeNode()
        {
            Dispose( false );
        }













































































        //private BoundingBox entityBounding;
        //private bool visible = false;
        //private List<ClientEntityHolder> entities = new List<ClientEntityHolder>();

        //private BoundingBox staticEntityBoundingBox = new BoundingBox();

        //private int lastOptimizeID = -1;



        ////TODO: maybe: should return true when the entity was added,
        ////				false when it already was in this node
        //public virtual void AddEntity( ClientEntityHolder entH )
        //{
        //    entities.Add( entH );

        //    //Update entityboundingbox

        //    EntityBoundingBox = MergeBoundingBoxes( EntityBoundingBox, entH.BoundingBox );

        //    /*if ( entities.Count == 0 )
        //    {
        //        if ( IsLeaf )
        //        {
        //            EntityBoundingBox = entH.BoundingBox;
        //        }
        //        else
        //        {
        //            BoundingBox b;
        //            b = CalculateTotalBoundingChildren();
        //            if ( b.Min == b.Max )
        //            {
        //                EntityBoundingBox = entH.BoundingBox;
        //            }
        //            else
        //            {
        //                EntityBoundingBox = BoundingBox.CreateMerged( b, entH.BoundingBox );
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if ( HasNoEntityBounding )
        //        {
        //            EntityBoundingBox = entH.BoundingBox;
        //        }
        //        else
        //        {
        //            EntityBoundingBox = BoundingBox.CreateMerged( EntityBoundingBox, entH.BoundingBox );
        //        }
        //    }*/




        //}

        //public virtual bool RemoveEntity( ClientEntityHolder entH )
        //{
        //    return entities.Remove( entH );

        //    //TODO: the node's bounding y zou moeten worden geupdate van tijd tot tijd
        //}


        //public void AddBoundingToEntityBounding( BoundingBox boundingBox )
        //{
        //    EntityBoundingBox = MergeBoundingBoxes( EntityBoundingBox, boundingBox );
        //}
        //public void AddBoundingToStaticEntityBounding( BoundingBox boundingBox )
        //{
        //    staticEntityBoundingBox = MergeBoundingBoxes( staticEntityBoundingBox, boundingBox );
        //    AddBoundingToEntityBounding( staticEntityBoundingBox );
        //}


        //public void RecalculateEntityBounding()
        //{
        //    //TODO recalculate static entity bounding
        //    //int i2;
        //    //if ( CalculateLevel() == 6 ) i2 = 0;
        //    //if ( terrainBlock != null ) i2 = 0;
        //    //if ( terrainBlock == null && entities.Count > 0 ) i2 = 0;
        //    //if ( entities.Count > 0 ) i2 = 0;

        //    entityBounding = CalculateTotalBoundingChildren();
        //    entityBounding = MergeBoundingBoxes( entityBounding, staticEntityBoundingBox );

        //    for ( int i = 0; i < entities.Count; i++ )
        //    {
        //        AddBoundingToEntityBounding( entities[ i ].BoundingBox );
        //    }
        //}


        public string BuildString()
        {
            return BuildString( 0 );
        }
        public string BuildString( int indentLenght )
        {
            const int indentStep = 3;
            string indent = "";
            string ret = "";
            for ( int i = 0; i < indentLenght; i++ )
            {
                indent += " ";
            }
            //ret += indent + "Node Entities:" + entities.Count.ToString();
            if ( upperLeft != null )
            {
                ret += Environment.NewLine + upperLeft.BuildString( indentLenght + indentStep );
            }
            if ( upperRight != null )
            {
                ret += Environment.NewLine + upperRight.BuildString( indentLenght + indentStep );
            }
            if ( lowerLeft != null )
            {
                ret += Environment.NewLine + lowerLeft.BuildString( indentLenght + indentStep );
            }
            if ( lowerRight != null )
            {
                ret += Environment.NewLine + lowerRight.BuildString( indentLenght + indentStep );
            }

            return ret;
        }

        //public void OnChildEntityBoundingChanged( QuadTreeNode child )
        //{
        //    EntityBoundingBox = MergeBoundingBoxes( EntityBoundingBox, child.EntityBoundingBox );
        //    /*if ( entities.Count == 0 )
        //    {
        //        EntityBoundingBox = CalculateTotalBoundingChildren();
        //    }
        //    else
        //    {
        //        EntityBoundingBox = MergeBoundingBoxes( EntityBoundingBox, child.EntityBoundingBox );
        //    }*/
        //}

        ///*public void OnEntityBoundingBoxChanged()
        //{
        //}*/
        //public void OnTerreinBlockBoundingChanged()
        //{
        //    AddBoundingToStaticEntityBounding( terrainBlock.BoundingBox );
        //}


        //public BoundingBox CalculateTotalBoundingChildren()
        //{
        //    if ( IsLeaf ) return new BoundingBox();

        //    BoundingBox b = new BoundingBox();
        //    bool empty = true;

        //    if ( !UpperLeft.HasNoEntityBounding )
        //    {
        //        /*if ( empty )
        //        {*/
        //        b = UpperLeft.EntityBoundingBox;
        //        empty = false;
        //        /*}
        //        else
        //        {
        //            b = BoundingBox.CreateMerged( b, upperLeft.EntityBoundingBox );
        //        }*/
        //    }

        //    if ( !UpperRight.HasNoEntityBounding )
        //    {
        //        if ( empty )
        //        {
        //            b = UpperRight.EntityBoundingBox;
        //            empty = false;
        //        }
        //        else
        //        {
        //            b = BoundingBox.CreateMerged( b, UpperRight.EntityBoundingBox );
        //        }
        //    }

        //    if ( !LowerLeft.HasNoEntityBounding )
        //    {
        //        if ( empty )
        //        {
        //            b = LowerLeft.EntityBoundingBox;
        //            empty = false;
        //        }
        //        else
        //        {
        //            b = BoundingBox.CreateMerged( b, LowerLeft.EntityBoundingBox );
        //        }
        //    }

        //    if ( !LowerRight.HasNoEntityBounding )
        //    {
        //        if ( empty )
        //        {
        //            b = LowerRight.EntityBoundingBox;
        //            empty = false;
        //        }
        //        else
        //        {
        //            b = BoundingBox.CreateMerged( b, LowerRight.EntityBoundingBox );
        //        }
        //    }


        //    return b;
        //}

        //protected override Common.Wereld.QuadTreeNode CreateChild( Vector3 min, Vector3 size )
        //{
        //    QuadTreeNode node = new QuadTreeNode();
        //    node.parent = this;
        //    node.boundingBox = new BoundingBox( min, min + size );
        //    node.boundingBox.Min.Y = this.BoundingBox.Min.Y;
        //    node.boundingBox.Max.Y = this.BoundingBox.Max.Y;
        //    return node;
        //}

        //public BoundingBox EntityBoundingBox
        //{
        //    get { return entityBounding; }
        //    set
        //    {
        //        entityBounding = value;
        //        if ( parent != null )
        //            Parent.OnChildEntityBoundingChanged( this );
        //    }
        //}

        //public bool Visible
        //{
        //    get { return visible; }
        //    set { visible = value; }
        //}

        //public List<ClientEntityHolder> Entities { get { return entities; } }
        public QuadTreeNode<T> UpperLeft
        {
            get { return upperLeft; }
            set { upperLeft = value; }
        }

        public QuadTreeNode<T> UpperRight
        {
            get { return upperRight; }
            set { upperRight = value; }
        }

        public QuadTreeNode<T> LowerLeft
        {
            get { return lowerLeft; }
            set { lowerLeft = value; }
        }

        public QuadTreeNode<T> LowerRight
        {
            get { return lowerRight; }
            set { lowerRight = value; }
        }

        public QuadTreeNode<T> Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        //public bool HasNoEntityBounding
        //{ get { return entityBounding.Min == entityBounding.Max; } }

        //public int LastOptimizeID
        //{ get { return lastOptimizeID; } set { lastOptimizeID = value; } }

        //public new XNAGeoMipMap.TerrainBlock TerrainBlock
        //{
        //    get { return (XNAGeoMipMap.TerrainBlock)terrainBlock; }

        //}










        private ulong address;
        protected QuadTreeNode<T> upperLeft;
        protected QuadTreeNode<T> upperRight;
        protected QuadTreeNode<T> lowerLeft;
        protected QuadTreeNode<T> lowerRight;
        protected QuadTreeNode<T> parent;
        protected BoundingBox boundingBox;
        //protected bool isStatic = false;
        //protected Common.GeoMipMap.TerrainBlock terrainBlock;

        public enum ChildDir
        {
            UpperLeft = 0,
            UpperRight = 1,
            LowerLeft = 2,
            LowerRight = 3
        }

        /*public QuadTreeNode()
            : base()
        {
            //UpdateAdress();
        }*/

        /*~QuadTreeNode()
        {
            Dispose( false );
        }*/


        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            lock ( this )
            {
                if ( disposing )
                {
                    if ( upperLeft != null )
                        upperLeft.Dispose();

                    if ( upperRight != null )
                        upperRight.Dispose();

                    if ( lowerLeft != null )
                        lowerLeft.Dispose();

                    if ( lowerRight != null )
                        lowerRight.Dispose();

                    if ( parent != null )
                        Quadtree.RemoveChild( parent, this );

                }

                upperLeft = null;
                upperRight = null;
                lowerLeft = null;
                lowerRight = null;
                parent = null;
            }

        }



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

        //public override string ToString()
        //{
        //    string ret = "";

        //    ret += "Node ";

        //    ret += "Level: " + CalculateLevel().ToString() + " ";
        //    if ( parent != null )
        //    {
        //        if ( parent.upperLeft == this ) ret += "UpperLeft ";
        //        if ( parent.upperRight == this ) ret += "UpperRight ";
        //        if ( parent.lowerLeft == this ) ret += "LowerLeft ";
        //        if ( parent.lowerRight == this ) ret += "LowerRight ";
        //    }
        //    if ( IsLeaf ) ret += "Leaf ";
        //    //if ( entities.Count > 0 ) ret += "Entities: " + entities.Count.ToString() + " ";


        //    return ret;
        //}
        public override string ToString()
        {
            return "Node " + NodeAddressToText( address );
        }





        /// <summary>
        /// Deprecated
        /// </summary>
        /// <returns></returns>
        public int CalculateLevel()
        {
            if ( parent == null ) return 0;

            return parent.CalculateLevel() + 1;
            //int level = 0;
            //QuadTreeNode iNode = parent;
            //while ( iNode != null )
            //{
            //    level += 1;
            //    iNode = iNode.parent;

            //}
            //return level;
        }






        public void UpdateAdress()
        {
            address = CalculateAddress();
        }

        public ulong CalculateAddress()
        {
            // Het adres
            // bit 0-3 : level van de node
            // bit 4-63 : Welke childnode het is, telkens 2 bits per level
            // 00 = upperLeft
            // 01 = upperRight
            // 10 = lowerLeft
            // 11 = lowerRight
            if ( parent == null ) return 0;

            //TODO: power operator wont work

            ulong levelBitmask = ( 1 << 0 ) + ( 1 << 1 ) + ( 1 << 2 ) + ( 1 << 3 );

            ulong parentLevel = parent.address & levelBitmask;

            ulong ret;

            ret = parent.address & ~levelBitmask;
            //Ret now contains the parents address with the 4 lvl bits set to 0
            //Now set this nodes lvl
            ret = ret | ( parentLevel + 1 );

            //Add the 2 bits at the end saying which child node this one is
            ulong childMask;
            if ( parent.upperLeft == this )
            {
                childMask = 0; //00
            }
            else if ( parent.upperRight == this )
            {
                childMask = 1; //01
            }
            else if ( parent.lowerLeft == this )
            {
                childMask = 2; //10
            }
            else if ( parent.lowerRight == this )
            {
                childMask = 3; //11
            }
            else
            {
                throw new Exception( "Impossible!!!" );
            }

            ret = ret | ( childMask << ( 4 + 2 * (int)( parentLevel + 1 - 1 ) ) );

            //string temp = TempULongToBitsString( ret );
            //string path = NodeAddressToText( ret );


            return ret;


        }

        public string NodeAddressToText( ulong _address )
        {
            string text = "";
            //Get address: first four bits of the address
            int level = (int)( _address & ( ( 1 << 0 ) + ( 1 << 1 ) + ( 1 << 2 ) + ( 1 << 3 ) ) );

            text += "Level: " + level.ToString() + " Root";

            //Get the location of the node for each lvl

            ulong mask = ( 1 << 0 ) + ( 1 << 1 );

            for ( int iLevel = 1; iLevel <= level; iLevel++ )
            {
                string locationName;
                //int location = (int)( address & ( mask << ( 4 + 2 * ( iLevel - 1 ) ) ) );
                int location = (int)( ( _address >> ( 4 + 2 * ( iLevel - 1 ) ) ) & mask );
                switch ( location )
                {
                    case 0:
                        locationName = "UpperLeft";
                        break;
                    case 1:
                        locationName = "UpperRight";
                        break;
                    case 2:
                        locationName = "LowerLeft";
                        break;
                    case 3:
                        locationName = "LowerRight";
                        break;
                    default:
                        throw new Exception( "Impossible" );
                }

                text += "." + locationName;


            }
            return text;
        }


        public string TempULongToBitsString( ulong val )
        {
            string ret = "";
            for ( int i = 0; i < 64; i++ )
            {
                ret = ( ( val & ( (ulong)1 << i ) ) > 0 ? "1" : "0" )
                    + ret;
            }
            return ret;
        }


        public BoundingBox BoundingBox
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }

        public ulong Address
        {
            get { return address; }
            set { address = value; }
        }

        public bool IsLeaf
        {
            get
            {
                //TODO: needs speedup?
                return ( upperLeft == null )
                    && ( upperRight == null )
                    && ( lowerLeft == null )
                    && ( lowerRight == null );
            }
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


        public bool CanMerge
        {
            get
            {
                if ( UpperLeft.IsLeaf
                    && UpperRight.IsLeaf
                    && LowerLeft.IsLeaf
                    && LowerRight.IsLeaf )
                    //&& !UpperLeft.Static
                    //&& !UpperRight.Static
                    //&& !LowerLeft.Static
                    //&& !LowerRight.Static )
                    return true;
                else
                    return false;

            }
        }

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
































        ////private BoundingBox entityBounding;
        ////private bool visible = false;
        ////private List<ClientEntityHolder> entities = new List<ClientEntityHolder>();

        ////private BoundingBox staticEntityBoundingBox = new BoundingBox();

        ////private int lastOptimizeID = -1;

        //public QuadTreeNode()
        //    : base()
        //{
        //}

        //~QuadTreeNode()
        //{
        //    Dispose( false );
        //}



        //protected override void Dispose( bool disposing )
        //{
        //    base.Dispose( disposing );

        //    lock ( this )
        //    {

        //    }

        //}



        ////TODO: maybe: should return true when the entity was added,
        ////				false when it already was in this node
        //public virtual void AddEntity( ClientEntityHolder entH )
        //{
        //    entities.Add( entH );

        //    //Update entityboundingbox

        //    EntityBoundingBox = MergeBoundingBoxes( EntityBoundingBox, entH.BoundingBox );

        //    /*if ( entities.Count == 0 )
        //    {
        //        if ( IsLeaf )
        //        {
        //            EntityBoundingBox = entH.BoundingBox;
        //        }
        //        else
        //        {
        //            BoundingBox b;
        //            b = CalculateTotalBoundingChildren();
        //            if ( b.Min == b.Max )
        //            {
        //                EntityBoundingBox = entH.BoundingBox;
        //            }
        //            else
        //            {
        //                EntityBoundingBox = BoundingBox.CreateMerged( b, entH.BoundingBox );
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if ( HasNoEntityBounding )
        //        {
        //            EntityBoundingBox = entH.BoundingBox;
        //        }
        //        else
        //        {
        //            EntityBoundingBox = BoundingBox.CreateMerged( EntityBoundingBox, entH.BoundingBox );
        //        }
        //    }*/




        //}

        //public virtual bool RemoveEntity( ClientEntityHolder entH )
        //{
        //    return entities.Remove( entH );

        //    //TODO: the node's bounding y zou moeten worden geupdate van tijd tot tijd
        //}


        //public void AddBoundingToEntityBounding( BoundingBox boundingBox )
        //{
        //    EntityBoundingBox = MergeBoundingBoxes( EntityBoundingBox, boundingBox );
        //}
        //public void AddBoundingToStaticEntityBounding( BoundingBox boundingBox )
        //{
        //    staticEntityBoundingBox = MergeBoundingBoxes( staticEntityBoundingBox, boundingBox );
        //    AddBoundingToEntityBounding( staticEntityBoundingBox );
        //}


        //public void RecalculateEntityBounding()
        //{
        //    //TODO recalculate static entity bounding
        //    //int i2;
        //    //if ( CalculateLevel() == 6 ) i2 = 0;
        //    //if ( terrainBlock != null ) i2 = 0;
        //    //if ( terrainBlock == null && entities.Count > 0 ) i2 = 0;
        //    //if ( entities.Count > 0 ) i2 = 0;

        //    entityBounding = CalculateTotalBoundingChildren();
        //    entityBounding = MergeBoundingBoxes( entityBounding, staticEntityBoundingBox );

        //    for ( int i = 0; i < entities.Count; i++ )
        //    {
        //        AddBoundingToEntityBounding( entities[ i ].BoundingBox );
        //    }
        //}


        //public override string BuildString()
        //{
        //    return BuildString( 0 );
        //}
        //public override string BuildString( int indentLenght )
        //{
        //    const int indentStep = 3;
        //    string indent = "";
        //    string ret = "";
        //    for ( int i = 0; i < indentLenght; i++ )
        //    {
        //        indent += " ";
        //    }
        //    ret += indent + "Node Entities:" + entities.Count.ToString();
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

        //public void OnChildEntityBoundingChanged( QuadTreeNode child )
        //{
        //    EntityBoundingBox = MergeBoundingBoxes( EntityBoundingBox, child.EntityBoundingBox );
        //    /*if ( entities.Count == 0 )
        //    {
        //        EntityBoundingBox = CalculateTotalBoundingChildren();
        //    }
        //    else
        //    {
        //        EntityBoundingBox = MergeBoundingBoxes( EntityBoundingBox, child.EntityBoundingBox );
        //    }*/
        //}

        ///*public void OnEntityBoundingBoxChanged()
        //{
        //}*/
        //public void OnTerreinBlockBoundingChanged()
        //{
        //    AddBoundingToStaticEntityBounding( terrainBlock.BoundingBox );
        //}


        //public BoundingBox CalculateTotalBoundingChildren()
        //{
        //    if ( IsLeaf ) return new BoundingBox();

        //    BoundingBox b = new BoundingBox();
        //    bool empty = true;

        //    if ( !UpperLeft.HasNoEntityBounding )
        //    {
        //        /*if ( empty )
        //        {*/
        //        b = UpperLeft.EntityBoundingBox;
        //        empty = false;
        //        /*}
        //        else
        //        {
        //            b = BoundingBox.CreateMerged( b, upperLeft.EntityBoundingBox );
        //        }*/
        //    }

        //    if ( !UpperRight.HasNoEntityBounding )
        //    {
        //        if ( empty )
        //        {
        //            b = UpperRight.EntityBoundingBox;
        //            empty = false;
        //        }
        //        else
        //        {
        //            b = BoundingBox.CreateMerged( b, UpperRight.EntityBoundingBox );
        //        }
        //    }

        //    if ( !LowerLeft.HasNoEntityBounding )
        //    {
        //        if ( empty )
        //        {
        //            b = LowerLeft.EntityBoundingBox;
        //            empty = false;
        //        }
        //        else
        //        {
        //            b = BoundingBox.CreateMerged( b, LowerLeft.EntityBoundingBox );
        //        }
        //    }

        //    if ( !LowerRight.HasNoEntityBounding )
        //    {
        //        if ( empty )
        //        {
        //            b = LowerRight.EntityBoundingBox;
        //            empty = false;
        //        }
        //        else
        //        {
        //            b = BoundingBox.CreateMerged( b, LowerRight.EntityBoundingBox );
        //        }
        //    }


        //    return b;
        //}


        //public BoundingBox EntityBoundingBox
        //{
        //    get { return entityBounding; }
        //    set
        //    {
        //        entityBounding = value;
        //        if ( parent != null )
        //            Parent.OnChildEntityBoundingChanged( this );
        //    }
        //}

        //public bool Visible
        //{
        //    get { return visible; }
        //    set { visible = value; }
        //}

        //public List<ClientEntityHolder> Entities { get { return entities; } }
        //public new QuadTreeNode UpperLeft
        //{
        //    get { return (QuadTreeNode)upperLeft; }
        //    set { upperLeft = value; }
        //}

        //public new QuadTreeNode UpperRight
        //{
        //    get { return (QuadTreeNode)upperRight; }
        //    set { upperRight = value; }
        //}

        //public new QuadTreeNode LowerLeft
        //{
        //    get { return (QuadTreeNode)lowerLeft; }
        //    set { lowerLeft = value; }
        //}

        //public new QuadTreeNode LowerRight
        //{
        //    get { return (QuadTreeNode)lowerRight; }
        //    set { lowerRight = value; }
        //}

        //public new QuadTreeNode Parent
        //{
        //    get { return (QuadTreeNode)parent; }
        //    set { parent = value; }
        //}

        //public bool HasNoEntityBounding
        //{ get { return entityBounding.Min == entityBounding.Max; } }

        //public int LastOptimizeID
        //{ get { return lastOptimizeID; } set { lastOptimizeID = value; } }

        //public new XNAGeoMipMap.TerrainBlock TerrainBlock
        //{
        //    get { return (XNAGeoMipMap.TerrainBlock)terrainBlock; }

        //}





    }
}
