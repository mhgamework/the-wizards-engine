using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Client
{
    /// <summary>
    /// This class stores IClientPhysicsObjects in a quadtree structure. It also stores whether physics in a specific part of the QuadTree is enabled or disabled. It should be enabled when an dynamic object is in the node or one of its children, and should be disabled when no dynamic objects are in the node or one of its children.
    /// </summary>
    public class ClientPhysicsQuadTreeNode : IQuadTreeNode<ClientPhysicsQuadTreeNode>
    {
        private bool physicsEnabled;
        public bool PhysicsEnabled
        {
            get { return physicsEnabled; }
            //set { physicsEnabled = value; }
        }

        private List<IClientPhysicsObject> physicsObjects;
        public List<IClientPhysicsObject> PhysicsObjects
        {
            get { return physicsObjects; }
            //set { physicsObjects = value; }
        }

        public int DynamicObjectsCount
        {
            get { return dynamicObjectsCount; }
        }

        private int dynamicObjectsCount;

        public ClientPhysicsQuadTreeNode(BoundingBox boundingBox)
        {
            nodeData = new QuadTreeNodeData<ClientPhysicsQuadTreeNode>(boundingBox);
            physicsEnabled = false;
            physicsObjects = new List<IClientPhysicsObject>();
            dynamicObjectsCount = 0;
        }

        private ClientPhysicsQuadTreeNode()
        {
            physicsEnabled = false;
            physicsObjects = new List<IClientPhysicsObject>();
            dynamicObjectsCount = 0;
        }



        /// <summary>
        /// Adds given obj to the tree, or updates its location in the tree
        /// This was previously/currently used for initially positioning static objects. This assigns the clientphysobject's node to its containing node and adds the object to this node.
        /// This has a a bad side-effect that the object's physics is enabled far to often. Better use AddStaticObject
        /// </summary>
        /// <param name="obj"></param>
        public void OrdenObject(IClientPhysicsObject obj)
        {
            ordenObject(this, obj);
            //return OrdenEntity( this, obj );
        }

      private static void ordenObject(ClientPhysicsQuadTreeNode node, IClientPhysicsObject obj)
        {
            ClientPhysicsQuadTreeNode containingNode = findContainingNode(node, obj);
            if (containingNode == null) containingNode = node;



            // Now the obj.Node parameter is used, but it is possible to shift the removing from old node responsability
            //  to the ICientPhysicsObject itself. This way, objects that never move do not have to store the Node field.
            if (obj.Node != null)
            {
                obj.Node.removePhysicsObject(obj);
            }

            containingNode.addPhysicsObject(obj);
            obj.Node = containingNode;



        }

        /// <summary>
        /// This increases the DynamicObject count by one. Notice that this doesn't store the list of dynamic objects itself
        /// WARNING: this function is not really at the same level as AddStaticObject
        /// </summary>
        /// <param name="obj"></param>
        public void AddDynamicObject(IClientPhysicsObject obj)
        {
            dynamicObjectsCount++;

        }
        public void RemoveDynamicObject(IClientPhysicsObject obj)
        {
            dynamicObjectsCount--;
        }

        public void AddStaticObject(IClientPhysicsObject obj)
        {
            ClientPhysicsQuadTreeNode containingNode = findContainingNode(this, obj);
            if (containingNode == null) containingNode = this;

            // Now the obj.Node parameter is used, but it is possible to shift the removing from old node responsability
            //  to the ICientPhysicsObject itself. This way, objects that never move do not have to store the Node field.
            if (obj.Node != null)
                throw new InvalidOperationException("A Static Object can only be added once!!!");

            obj.Node = containingNode;

            addStaticObjectRecurs(containingNode, obj);

        }
        private static void addStaticObjectRecurs(ClientPhysicsQuadTreeNode node, IClientPhysicsObject obj)
        {


            if (obj.ContainedInNode(node) == ContainmentType.Disjoint) return;
            if (QuadTree.IsLeafNode(node))
            {
                node.addPhysicsObject(obj);
                
                //TODO: need to enable physics right away?
                //if (node.dynamicObjectsCount > 0) node.enablePhysics(); else node.disablePhysics();


            }
            else
            {
                addStaticObjectRecurs(node.nodeData.UpperLeft, obj);
                addStaticObjectRecurs(node.nodeData.UpperRight, obj);
                addStaticObjectRecurs(node.nodeData.LowerLeft, obj);
                addStaticObjectRecurs(node.nodeData.LowerRight, obj);
            }

       
            



        }

        private static ClientPhysicsQuadTreeNode findContainingNode(ClientPhysicsQuadTreeNode node, IClientPhysicsObject obj)
        {
            if (node == null) return null;

            if (obj.ContainedInNode(node) != Microsoft.Xna.Framework.ContainmentType.Contains)
            {
                //entity zit niet in deze node
                return null;
            }


            //Entity volledig zit in deze node. Check of hij volledig in een van de children zit.

            ClientPhysicsQuadTreeNode ret;

            if ((ret = findContainingNode(node.nodeData.UpperLeft, obj)) != null) return ret;
            if ((ret = findContainingNode(node.nodeData.UpperRight, obj)) != null) return ret;
            if ((ret = findContainingNode(node.nodeData.LowerLeft, obj)) != null) return ret;
            if ((ret = findContainingNode(node.nodeData.LowerRight, obj)) != null) return ret;

            //Zit niet volleidg in een van de children.

            return node;
        }

        /// <summary>
        /// This function adds 1 to the dynamic object count of all leaf nodes intersecting or containing given obj
        /// </summary>
        /// <param name="obj"></param>
        public void AddDynamicObjectToIntersectingNodes(IClientPhysicsObject obj)
        {
            changeDynamicCountForIntersectingNodes(this, obj, 1);
        }

        /// <summary>
        /// This function subtracts 1 to the dynamic object count of all leaf nodes intersecting or containing given obj
        /// </summary>
        /// <param name="obj"></param>
        public void RemoveDynamicObjectFromIntersectingNodes(IClientPhysicsObject obj)
        {
            changeDynamicCountForIntersectingNodes(this, obj, -1);
        }


        /// <summary>
        /// This function adds 'change' to all the leaf nodes that intersect or contain given object
        /// </summary>
        /// <param name="node"></param>
        /// <param name="obj"></param>
        /// <param name="change"></param>
        private static void changeDynamicCountForIntersectingNodes(ClientPhysicsQuadTreeNode node, IClientPhysicsObject obj, int change)
        {
            if (obj.ContainedInNode(node) == ContainmentType.Disjoint) return;
            if (QuadTree.IsLeafNode(node))
            {
                node.dynamicObjectsCount += change;

                if (node.dynamicObjectsCount < 0)
                    throw new InvalidOperationException("Invalid dynamicobjectscount. Cannot go below zero!");
                
                // update the physicsenabled flags
                if (node.dynamicObjectsCount > 0) node.enablePhysics(); else node.disablePhysics();
                

            }
            else
            {
                changeDynamicCountForIntersectingNodes(node.nodeData.UpperLeft, obj, change);
                changeDynamicCountForIntersectingNodes(node.nodeData.UpperRight, obj, change);
                changeDynamicCountForIntersectingNodes(node.nodeData.LowerLeft, obj, change);
                changeDynamicCountForIntersectingNodes(node.nodeData.LowerRight, obj, change);
            }


        }

        private void enablePhysics()
        {
            if (physicsEnabled) return;
            physicsEnabled = true;
            for (int i = 0; i < physicsObjects.Count; i++)
            {
                physicsObjects[i].EnablePhysics();
            }

            if (nodeData.Parent != null)
                nodeData.Parent.enablePhysics();

        }
        private void disablePhysics()
        {
            if (!physicsEnabled) return;
            //if (nodeData.Parent == null) System.Diagnostics.Debugger.Break();
            physicsEnabled = false;
            for (int i = 0; i < physicsObjects.Count; i++)
            {
                physicsObjects[i].DisablePhysics();
            }

            if (nodeData.Parent != null)

                if (!nodeData.Parent.nodeData.LowerLeft.physicsEnabled
                    && !nodeData.Parent.nodeData.LowerRight.physicsEnabled
                    && !nodeData.Parent.nodeData.UpperRight.physicsEnabled
                    && !nodeData.Parent.nodeData.UpperLeft.physicsEnabled)
                    nodeData.Parent.disablePhysics();

        }



        /*public void FindCollisionNodes(ClientPhysicsQuadTreeNode node, IClientPhysicsObject entH, List<ClientPhysicsQuadTreeNode> collisionNodes)
        {
            if (entH.ContainedInNode(node) == ContainmentType.Disjoint) return;
            if (QuadTree.IsLeafNode(node))
            {
                collisionNodes.Add(node);
            }
            else
            {
                FindCollisionNodes(node.nodeData.UpperLeft, entH, collisionNodes);
                FindCollisionNodes(node.nodeData.UpperRight, entH, collisionNodes);
                FindCollisionNodes(node.nodeData.LowerLeft, entH, collisionNodes);
                FindCollisionNodes(node.nodeData.LowerRight, entH, collisionNodes);
            }



        }*/


        private void addPhysicsObject(IClientPhysicsObject obj)
        {
            physicsObjects.Add(obj);
        }

        private void removePhysicsObject(IClientPhysicsObject obj)
        {
            physicsObjects.Remove(obj);
        }

        //        public void UpdateTreeDepth(QuadTreeNode node)
        //        {
        //            if (node == null) return;
        //            if (node.IsLeaf)
        //            {
        //                if (node.Entities.Count > maxNodeEntities)
        //                {
        //                    Vector3 diff = node.BoundingBox.Max - node.BoundingBox.Min;

        //                    //TODO: delen door 2 of maal 1/2?

        //                    if ((diff.X / 2) >= minNodeSize.X && (diff.Z / 2) >= minNodeSize.Y)
        //                    {

        //                        List<ClientEntityHolder> ents = new List<ClientEntityHolder>(node.Entities);
        //                        /*for ( int i = 0; i < ents.Count; i++ )
        //                        {
        //                            ents[ i ].MoveToNode( null );
        //                        }
        //#if DEBUG
        //                        //node.Entities.Count should be 0!!!
        //                        if ( node.Entities.Count != null ) throw new Exception();
        //#endif*/
        //                        node.Entities.Clear();

        //                        node.Split();

        //                        for (int i = 0; i < ents.Count; i++)
        //                        {
        //                            OrdenEntity(node, ents[i], false);
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                UpdateTreeDepth(node.UpperLeft);
        //                UpdateTreeDepth(node.UpperRight);
        //                UpdateTreeDepth(node.LowerLeft);
        //                UpdateTreeDepth(node.LowerRight);

        //                if (node.CanMerge)
        //                {
        //                    //Check if we need to merge
        //                    //NOTE: < maxNodeEntities zou normal gezien <= maxNodeEntities moeten zijn
        //                    //		maar dit voorkomt mss een overvloedig mergen-splitten.
        //                    if ((node.UpperLeft.Entities.Count
        //                            + node.UpperRight.Entities.Count
        //                            + node.LowerLeft.Entities.Count
        //                            + node.LowerRight.Entities.Count) < maxNodeEntities)
        //                    {
        //                        List<ClientEntityHolder> ents = new List<ClientEntityHolder>();
        //                        ents.AddRange(node.UpperLeft.Entities);
        //                        ents.AddRange(node.UpperRight.Entities);
        //                        ents.AddRange(node.LowerLeft.Entities);
        //                        ents.AddRange(node.LowerRight.Entities);

        //                        node.Merge();

        //                        //for ( int i = 0; i < ents.Count; i++ )
        //                        //{
        //                        //    OrdenEntity( node, ents[ i ] );
        //                        //}
        //                        //Als ent in een van de voormalige children zat, dan zit ent ook in deze node
        //                        //node.Entities.AddRange( ents );
        //                        for (int i = 0; i < ents.Count; i++)
        //                        {
        //                            ents[i].MoveToNode(node);
        //                        }
        //                    }
        //                }

        //            }

        //        }
















        #region IQuadTreeNode<ClientPhysicsQuadTreeNode> Members

        private QuadTreeNodeData<ClientPhysicsQuadTreeNode> nodeData;
        public QuadTreeNodeData<ClientPhysicsQuadTreeNode> NodeData
        {
            get
            {
                return nodeData;
            }
            set
            {
                nodeData = value;
            }
        }

        public ClientPhysicsQuadTreeNode CreateChild(QuadTreeNodeData<ClientPhysicsQuadTreeNode> _nodeData)
        {
            return new ClientPhysicsQuadTreeNode();
        }

        #endregion
    }
}
