using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Client;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Client
{
    [TestFixture]
    public class ClientTest
    {
        /// <summary>
        /// This test is obsolete, since the class it test is not used in the current implementation
        /// </summary>
        [Test]
        [RequiresThread( System.Threading.ApartmentState.STA )]
        public void TestRunClientXNAGameService()
        {
            Database.Database db = new MHGameWork.TheWizards.Database.Database();
            ClientXNAGameService cg = new ClientXNAGameService( db );
            cg.XNAGame.DrawEvent += delegate { cg.XNAGame.Exit(); };


            cg.XNAGame.Run();
        }


        public static ClientPhysicsQuadTreeNode CreateTestClientPhysicsQuadtree()
        {
            ClientPhysicsQuadTreeNode root = new ClientPhysicsQuadTreeNode(
                new BoundingBox(
                new Vector3( -100, -100, -100 ), new Vector3( 100, 100, 100 ) )
                );



            QuadTree.Split( root, 4 );

            QuadTree.Merge( root.NodeData.LowerRight.NodeData.LowerRight.NodeData.UpperLeft );

            QuadTree.MergeRecursive( root.NodeData.LowerLeft.NodeData.UpperRight );

            QuadTree.MergeRecursive( root.NodeData.UpperRight );

            return root;
        }
        public static Curve3D CreateTestObject1MovementCurve()
        {
            Curve3D curve1 = new Curve3D();
            curve1.PostLoop = CurveLoopType.Cycle;
            curve1.AddKey( 0, new Vector3( 20, 2, 20 ) );
            curve1.AddKey( 1, new Vector3( -20, 2, 80 ) );
            curve1.AddKey( 3, new Vector3( -3, 2, -10 ) );
            curve1.AddKey( 4, new Vector3( -20, 2, -50 ) );
            curve1.AddKey( 5, new Vector3( -50, 2, -50 ) );
            curve1.AddKey( 6, new Vector3( 38, 2, -47 ) );
            curve1.AddKey( 7, new Vector3( 20, 2, 20 ) );

            return curve1;
        }
        /// <summary>
        /// This tests places a number of objects in the quadtree and highlights the nodes containing objects
        /// </summary>
        [Test]
        [RequiresThread( System.Threading.ApartmentState.STA )]
        public void TestPhysicsQuadTreeOrdenObjects()
        {


            ClientPhysicsQuadTreeNode root = CreateTestClientPhysicsQuadtree();

            List<ClientPhysicsTestSphere> spheres = new List<ClientPhysicsTestSphere>();

            spheres.Add( new ClientPhysicsTestSphere( new Vector3( 3, 0, 3 ), 1 ) );
            spheres.Add( new ClientPhysicsTestSphere( new Vector3( 33, 0, -83 ), 1 ) );
            spheres.Add( new ClientPhysicsTestSphere( new Vector3( -25, 0, 40 ), 1 ) );
            spheres.Add( new ClientPhysicsTestSphere( new Vector3( -25, 0, -35 ), 1 ) );
            for ( int i = 0; i < spheres.Count; i++ )
            {
                root.OrdenObject( spheres[ i ] );
            }


            ClientPhysicsTestSphere movingSphere = new ClientPhysicsTestSphere( Vector3.Zero, 2 );
            Curve3D curve = CreateTestObject1MovementCurve();

            float time = 0;

            QuadTreeVisualizerXNA visualizer = new QuadTreeVisualizerXNA();

            XNAGame game = new XNAGame();

            game.UpdateEvent +=
                delegate
                {
                    time += game.Elapsed;
                    movingSphere.Center = curve.Evaluate( time * ( 1 / 4f ) );
                    root.OrdenObject( movingSphere );
                };

            game.DrawEvent +=
                delegate
                {
                    for ( int i = 0; i < spheres.Count; i++ )
                    {
                        game.LineManager3D.AddCenteredBox( spheres[ i ].Center, spheres[ i ].Radius, Color.Red );
                    }
                    game.LineManager3D.AddCenteredBox( movingSphere.Center, movingSphere.Radius, Color.Red );
                    visualizer.RenderNodeGroundBoundig( game, root,
                        delegate( ClientPhysicsQuadTreeNode node, out Color col )
                        {
                            col = Color.Green;

                            return node.PhysicsObjects.Count == 0;
                        } );

                    visualizer.RenderNodeGroundBoundig( game, root,
                       delegate( ClientPhysicsQuadTreeNode node, out Color col )
                       {
                           col = Color.Orange;

                           return node.PhysicsObjects.Count > 0;
                       } );
                };



            game.Run();

        }


        /// <summary>
        /// Moves around some dynamic objects and show color info:
        /// Orange: one dynamic object
        /// Yellow: 2 or more dynamic objects
        /// Red: Physics enabled
        /// Purple: dynamic objects count negative ==> error in algorithm
        /// 
        /// Press P to pause
        /// </summary>
        [Test]
        [RequiresThread( System.Threading.ApartmentState.STA )]
        public void TestPhysicsEnableDisable()
        {
            List<ClientPhysicsTestSphere> spheres = new List<ClientPhysicsTestSphere>();

            ClientPhysicsQuadTreeNode root = CreateTestClientPhysicsQuadtree();



            spheres.Add( new ClientPhysicsTestSphere( new Vector3( 0, 0, 0 ), 2 ) );


            Curve3D curve1 = CreateTestObject1MovementCurve();





            float time = 0;
            bool progressTime = true;
            QuadTreeVisualizerXNA visualizer = new QuadTreeVisualizerXNA();

            XNAGame game = new XNAGame();

            game.UpdateEvent +=
                delegate
                {
                    if ( game.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.P ) )
                    {
                        progressTime = !progressTime;
                    }

                    if ( progressTime )
                        time += game.Elapsed;


                    spheres[ 0 ].Move( root, curve1.Evaluate( time * ( 1 / 4f ) ) );

                };

            game.DrawEvent +=
                delegate
                {
                    for ( int i = 0; i < spheres.Count; i++ )
                    {
                        game.LineManager3D.AddCenteredBox( spheres[ i ].Center, spheres[ i ].Radius, Color.Red );
                    }
                    visualizer.RenderNodeGroundBoundig( game, root,
                        delegate( ClientPhysicsQuadTreeNode node, out Color col )
                        {
                            col = Color.Green;

                            return node.DynamicObjectsCount == 0 && !node.PhysicsEnabled;
                        } );
                    visualizer.RenderNodeGroundBoundig( game, root,
                       delegate( ClientPhysicsQuadTreeNode node, out Color col )
                       {
                           col = Color.Red;

                           return node.DynamicObjectsCount == 0 && node.PhysicsEnabled;
                       } );

                    visualizer.RenderNodeGroundBoundig( game, root,
                       delegate( ClientPhysicsQuadTreeNode node, out Color col )
                       {
                           col = Color.Yellow;
                           if ( node.DynamicObjectsCount == 1 )
                               col = Color.Orange;

                           return node.DynamicObjectsCount > 0;
                       } );

                    visualizer.RenderNodeGroundBoundig( game, root,
                        delegate( ClientPhysicsQuadTreeNode node, out Color col )
                        {
                            col = Color.Purple;

                            return node.DynamicObjectsCount < 0;
                        } );

                };



            game.Run();

        }

        /// <summary>
        /// This class can be promoted to a real part of the wizards, eventually
        /// </summary>
        [Obsolete( "Has been replaced by a newer implementation, see ClientTestPhysicsSphere" )]
        public class ClientTestPhysicsSphereOld : IClientPhysicsObject
        {

            public Vector3 Center;
            public float Radius;

            public ClientTestPhysicsSphereOld( Vector3 center, float radius )
            {
                Center = center;
                Radius = radius;
            }

            public void Move( ClientPhysicsQuadTreeNode root, Vector3 newCenter )
            {
                // NOTE: IMPORTANT: this is actually a partial implementation of the algorithm itself

                Vector3 oldCenter = Center;
                ClientPhysicsQuadTreeNode oldNode = Node;


                // Update location in quadtree
                Center = newCenter;
                root.OrdenObject( this );

                // Update dynamic object count

                Node.AddDynamicObjectToIntersectingNodes( this ); // Add must come before remove to prevent overhead

                if ( oldNode != null )
                {
                    Center = oldCenter; // set old state
                    oldNode.RemoveDynamicObjectFromIntersectingNodes( this );

                    Center = newCenter; // set new state
                }
            }

            #region IClientPhysicsObject Members

            public void EnablePhysics()
            {

            }

            public void DisablePhysics()
            {

            }




            private ClientPhysicsQuadTreeNode node;
            public ClientPhysicsQuadTreeNode Node
            {
                get { return node; }
                set { node = value; }
            }

            public ContainmentType ContainedInNode( ClientPhysicsQuadTreeNode _node )
            {
                return _node.NodeData.BoundingBox.xna().Contains(new BoundingSphere(Center, Radius));
            }

            #endregion
        }

    }
}
