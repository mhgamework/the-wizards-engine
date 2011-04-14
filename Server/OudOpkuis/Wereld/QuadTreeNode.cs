using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Server.Wereld
{
    public class QuadTreeNode : Common.Wereld.QuadTreeNode
    {
        private bool _physicsEnabled = false;
        private List<IEntityHolder> entities = new List<IEntityHolder>();
        private int _dynamicEntitiesReferenceCount = 0;

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
                if ( disposing )
                {


                }


            }

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



        //TODO: maybe: should return true when the entity was added,
        //				false when it already was in this node
        public virtual void AddEntity( IEntityHolder entH )
        {
            entities.Add( entH );



        }

        public virtual bool RemoveEntity( IEntityHolder entH )
        {
            return entities.Remove( entH );

            //TODO: the node's bounding y zou moeten worden geupdate van tijd tot tijd
        }


        public void AddDynamicEntityReference()
        {
            DynamicEntitiesReferenceCount++;
        }

        public void RemoveDynamicEntityReference()
        {
            DynamicEntitiesReferenceCount--;
        }

        public void OnPhysicsEnabled()
        {
            //Enable physics of all the entities in this node
            for ( int i = 0; i < entities.Count; i++ )
            {
                entities[ i ].EnablePhysics();
            }


            if ( Parent != null ) Parent.OnChildPhysicsEnabled();
        }
        public void OnPhysicsDisabled()
        {
            //Disabled physics of all the entities in this node
            for ( int i = 0; i < entities.Count; i++ )
            {
                entities[ i ].DisablePhysics();
            }
            if ( Parent != null ) Parent.OnChildPhysicsDisabled();
        }

        public void OnChildPhysicsEnabled()
        {
            PhysicsEnabled = true;
        }
        public void OnChildPhysicsDisabled()
        {

            PhysicsEnabled = UpperLeft.PhysicsEnabled || UpperRight.PhysicsEnabled
                            || LowerLeft.PhysicsEnabled || LowerRight.PhysicsEnabled;
        }


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

        public bool PhysicsEnabled
        {
            get { return _physicsEnabled; }
            set
            {
                if ( _physicsEnabled != value )
                {
                    _physicsEnabled = value;

                    if ( _physicsEnabled == true )
                    {
                        OnPhysicsEnabled();
                    }
                    else
                    {
                        OnPhysicsDisabled();
                    }

                }

            }
        }


        /// <summary>
        /// This contains the number of dynamic entities that is in this node or partially in this node.
        /// </summary>
        public int DynamicEntitiesReferenceCount
        {
            get { return _dynamicEntitiesReferenceCount; }
            set
            {
                _dynamicEntitiesReferenceCount = value;
                if ( _dynamicEntitiesReferenceCount != 0 )
                    PhysicsEnabled = true;
                else
                    PhysicsEnabled = false;
            }
        }

        public List<IEntityHolder> Entities { get { return entities; } }





    }
}
