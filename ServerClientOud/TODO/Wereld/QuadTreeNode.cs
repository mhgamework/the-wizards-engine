using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.ServerClient.Wereld
{
    public class QuadTreeNode : Common.Wereld.QuadTreeNode
    {

        private BoundingBox entityBounding;
        private bool visible = false;
        private List<ClientEntityHolder> entities = new List<ClientEntityHolder>();

        private BoundingBox staticEntityBoundingBox = new BoundingBox();

        private int lastOptimizeID = -1;

        public QuadTreeNode()
            : base()
        {
        }

        ~QuadTreeNode()
        {
            Dispose( false );
        }



        protected override void Dispose( bool disposing )
        {
            base.Dispose( disposing );

            lock ( this )
            {

            }

        }



        //TODO: maybe: should return true when the entity was added,
        //				false when it already was in this node
        public virtual void AddEntity( ClientEntityHolder entH )
        {
            entities.Add( entH );

            //Update entityboundingbox

            EntityBoundingBox = MergeBoundingBoxes( EntityBoundingBox, entH.BoundingBox );

            /*if ( entities.Count == 0 )
            {
                if ( IsLeaf )
                {
                    EntityBoundingBox = entH.BoundingBox;
                }
                else
                {
                    BoundingBox b;
                    b = CalculateTotalBoundingChildren();
                    if ( b.Min == b.Max )
                    {
                        EntityBoundingBox = entH.BoundingBox;
                    }
                    else
                    {
                        EntityBoundingBox = BoundingBox.CreateMerged( b, entH.BoundingBox );
                    }
                }
            }
            else
            {
                if ( HasNoEntityBounding )
                {
                    EntityBoundingBox = entH.BoundingBox;
                }
                else
                {
                    EntityBoundingBox = BoundingBox.CreateMerged( EntityBoundingBox, entH.BoundingBox );
                }
            }*/




        }

        public virtual bool RemoveEntity( ClientEntityHolder entH )
        {
            return entities.Remove( entH );

            //TODO: the node's bounding y zou moeten worden geupdate van tijd tot tijd
        }


        public void AddBoundingToEntityBounding( BoundingBox boundingBox )
        {
            EntityBoundingBox = MergeBoundingBoxes( EntityBoundingBox, boundingBox );
        }
        public void AddBoundingToStaticEntityBounding( BoundingBox boundingBox )
        {
            staticEntityBoundingBox = MergeBoundingBoxes( staticEntityBoundingBox, boundingBox );
            AddBoundingToEntityBounding( staticEntityBoundingBox );
        }


        public void RecalculateEntityBounding()
        {
            //TODO recalculate static entity bounding
            //int i2;
            //if ( CalculateLevel() == 6 ) i2 = 0;
            //if ( terrainBlock != null ) i2 = 0;
            //if ( terrainBlock == null && entities.Count > 0 ) i2 = 0;
            //if ( entities.Count > 0 ) i2 = 0;

            entityBounding = CalculateTotalBoundingChildren();
            entityBounding = MergeBoundingBoxes( entityBounding, staticEntityBoundingBox );

            for ( int i = 0; i < entities.Count; i++ )
            {
                AddBoundingToEntityBounding( entities[ i ].BoundingBox );
            }
        }


        public override string BuildString()
        {
            return BuildString( 0 );
        }
        public override string BuildString( int indentLenght )
        {
            const int indentStep = 3;
            string indent = "";
            string ret = "";
            for ( int i = 0; i < indentLenght; i++ )
            {
                indent += " ";
            }
            ret += indent + "Node Entities:" + entities.Count.ToString();
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

        public void OnChildEntityBoundingChanged( QuadTreeNode child )
        {
            EntityBoundingBox = MergeBoundingBoxes( EntityBoundingBox, child.EntityBoundingBox );
            /*if ( entities.Count == 0 )
            {
                EntityBoundingBox = CalculateTotalBoundingChildren();
            }
            else
            {
                EntityBoundingBox = MergeBoundingBoxes( EntityBoundingBox, child.EntityBoundingBox );
            }*/
        }

        /*public void OnEntityBoundingBoxChanged()
        {
        }*/
        public void OnTerreinBlockBoundingChanged()
        {
            AddBoundingToStaticEntityBounding( terrainBlock.BoundingBox );
        }


        public BoundingBox CalculateTotalBoundingChildren()
        {
            if ( IsLeaf ) return new BoundingBox();

            BoundingBox b = new BoundingBox();
            bool empty = true;

            if ( !UpperLeft.HasNoEntityBounding )
            {
                /*if ( empty )
                {*/
                b = UpperLeft.EntityBoundingBox;
                empty = false;
                /*}
                else
                {
                    b = BoundingBox.CreateMerged( b, upperLeft.EntityBoundingBox );
                }*/
            }

            if ( !UpperRight.HasNoEntityBounding )
            {
                if ( empty )
                {
                    b = UpperRight.EntityBoundingBox;
                    empty = false;
                }
                else
                {
                    b = BoundingBox.CreateMerged( b, UpperRight.EntityBoundingBox );
                }
            }

            if ( !LowerLeft.HasNoEntityBounding )
            {
                if ( empty )
                {
                    b = LowerLeft.EntityBoundingBox;
                    empty = false;
                }
                else
                {
                    b = BoundingBox.CreateMerged( b, LowerLeft.EntityBoundingBox );
                }
            }

            if ( !LowerRight.HasNoEntityBounding )
            {
                if ( empty )
                {
                    b = LowerRight.EntityBoundingBox;
                    empty = false;
                }
                else
                {
                    b = BoundingBox.CreateMerged( b, LowerRight.EntityBoundingBox );
                }
            }


            return b;
        }

        protected override Common.Wereld.QuadTreeNode CreateChild( Vector3 min, Vector3 size )
        {
            QuadTreeNode node = new QuadTreeNode();
            node.parent = this;
            node.boundingBox = new BoundingBox( min, min + size );
            node.boundingBox.Min.Y = this.BoundingBox.Min.Y;
            node.boundingBox.Max.Y = this.BoundingBox.Max.Y;
            return node;
        }

        public BoundingBox EntityBoundingBox
        {
            get { return entityBounding; }
            set
            {
                entityBounding = value;
                if ( parent != null )
                    Parent.OnChildEntityBoundingChanged( this );
            }
        }

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        public List<ClientEntityHolder> Entities { get { return entities; } }
        public new QuadTreeNode UpperLeft
        {
            get { return (QuadTreeNode)upperLeft; }
            set { upperLeft = value; }
        }

        public new QuadTreeNode UpperRight
        {
            get { return (QuadTreeNode)upperRight; }
            set { upperRight = value; }
        }

        public new QuadTreeNode LowerLeft
        {
            get { return (QuadTreeNode)lowerLeft; }
            set { lowerLeft = value; }
        }

        public new QuadTreeNode LowerRight
        {
            get { return (QuadTreeNode)lowerRight; }
            set { lowerRight = value; }
        }

        public new QuadTreeNode Parent
        {
            get { return (QuadTreeNode)parent; }
            set { parent = value; }
        }

        public bool HasNoEntityBounding
        { get { return entityBounding.Min == entityBounding.Max; } }

        public int LastOptimizeID
        { get { return lastOptimizeID; } set { lastOptimizeID = value; } }

        public new XNAGeoMipMap.TerrainBlock TerrainBlock
        {
            get { return (XNAGeoMipMap.TerrainBlock)terrainBlock; }

        }
    }
}
