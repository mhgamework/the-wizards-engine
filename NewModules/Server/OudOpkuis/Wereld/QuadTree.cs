using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Server.Wereld
{
    public class QuadTree : QuadTreeNode
    {
        private int maxNodeEntities = 4;
        private Vector2 minNodeSize = new Vector2( 10, 10 );

        private Color[] levelColors;

        public QuadTree( BoundingBox bounding )
        {
            BoundingBox = bounding;

            levelColors = new Color[ 5 ];
            levelColors[ 0 ] = Color.Red;
            levelColors[ 1 ] = Color.Orange;
            levelColors[ 2 ] = Color.Yellow;
            levelColors[ 3 ] = Color.Purple;
            levelColors[ 4 ] = Color.Green;

        }


        public void OrdenEntity( IEntityHolder entH )
        {
            OrdenEntity( this, entH, true );
            //return OrdenEntity( this, entH );
        }
        public void OrdenEntity( QuadTreeNode node, IEntityHolder entH, bool updateDepth )
        {
            QuadTreeNode containingNode = FindContainingNode( this, entH );
            if ( containingNode == null ) containingNode = this;

            entH.MoveToNode( containingNode );

            if ( updateDepth ) UpdateTreeDepth( entH.ContainingNode );




            /*if ( OrdenEntity( node, entH, true ) == false )
            {
                //enth
                return true; //of false?
            }
            else
            {
                return true;
            }*/
        }
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="ent"></param>
        /// <returns>Returns true if 'ent' was added to 'node' or to one of its children </returns>
        public bool OrdenEntity( QuadTreeNode node, ServerEntityHolder entH, bool updateDepth )
        {
            if ( node == null ) return false;
            //TODO: contains of intersects?
            if ( node.BoundingBox.Contains( entH.BoundingSphere ) != Microsoft.Xna.Framework.ContainmentType.Contains )
            {
                if ( node == this )
                {
                    //De ent bevindt zich buiten de Quadtree, voeg toch maar toe
                    entH.MoveToNode( node );
                    if ( updateDepth ) UpdateTreeDepth( node );
                }
                else { return false; }

            }

            if ( !OrdenEntity( node.UpperLeft, entH, updateDepth )
                && !OrdenEntity( node.UpperRight, entH, updateDepth )
                && !OrdenEntity( node.LowerLeft, entH, updateDepth )
                && !OrdenEntity( node.LowerRight, entH, updateDepth ) )
            {
                //Add entity to this node
                entH.MoveToNode( node );
                if ( updateDepth ) UpdateTreeDepth( node );

            }

            //Entity was added to this node or one of its children
            return true;

        }*/

        public QuadTreeNode FindContainingNode( QuadTreeNode node, IEntityHolder entH )
        {
            if ( node == null ) return null;

            if ( node.BoundingBox.Contains( entH.BoundingSphere ) != Microsoft.Xna.Framework.ContainmentType.Contains )
            {
                //entity zit niet in deze node
                return null;
            }


            //Entity volledig zit in deze node. Check of hij volledig in een van de children zit.

            QuadTreeNode ret;

            if ( ( ret = FindContainingNode( node.UpperLeft, entH ) ) != null ) return ret;
            if ( ( ret = FindContainingNode( node.UpperRight, entH ) ) != null ) return ret;
            if ( ( ret = FindContainingNode( node.LowerLeft, entH ) ) != null ) return ret;
            if ( ( ret = FindContainingNode( node.LowerRight, entH ) ) != null ) return ret;

            //Zit niet in een van de children.

            return node;
        }

        public void FindCollisionNodes( QuadTreeNode node, IEntityHolder entH, List<QuadTreeNode> collisionNodes )
        {
            if ( node.BoundingBox.Contains( entH.BoundingSphere ) == ContainmentType.Disjoint ) return;
            if ( node.IsLeaf )
            {
                collisionNodes.Add( node );
            }
            else
            {
                FindCollisionNodes( node.UpperLeft, entH, collisionNodes );
                FindCollisionNodes( node.UpperRight, entH, collisionNodes );
                FindCollisionNodes( node.LowerLeft, entH, collisionNodes );
                FindCollisionNodes( node.LowerRight, entH, collisionNodes );
            }



        }

        public void UpdateTreeDepth( QuadTreeNode node )
        {
            if ( node == null ) return;
            if ( node.IsLeaf )
            {
                if ( node.Entities.Count > maxNodeEntities )
                {
                    Vector3 diff = node.BoundingBox.Max - node.BoundingBox.Min;

                    //TODO: delen door 2 of maal 1/2?

                    if ( ( diff.X / 2 ) >= minNodeSize.X && ( diff.Z / 2 ) >= minNodeSize.Y )
                    {

                        List<IEntityHolder> ents = new List<IEntityHolder>( node.Entities );
                        /*for ( int i = 0; i < ents.Count; i++ )
                        {
                            ents[ i ].MoveToNode( null );
                        }
#if DEBUG
                        //node.Entities.Count should be 0!!!
                        if ( node.Entities.Count != null ) throw new Exception();
#endif*/
                        node.Entities.Clear();

                        node.Split();

                        for ( int i = 0; i < ents.Count; i++ )
                        {
                            OrdenEntity( node, ents[ i ], false );
                        }
                    }
                }
            }
            else
            {
                UpdateTreeDepth( node.UpperLeft );
                UpdateTreeDepth( node.UpperRight );
                UpdateTreeDepth( node.LowerLeft );
                UpdateTreeDepth( node.LowerRight );

                if ( node.CanMerge )
                {
                    //Check if we need to merge
                    //NOTE: < maxNodeEntities zou normal gezien <= maxNodeEntities moeten zijn
                    //		maar dit voorkomt mss een overvloedig mergen-splitten.
                    if ( ( node.UpperLeft.Entities.Count
                            + node.UpperRight.Entities.Count
                            + node.LowerLeft.Entities.Count
                            + node.LowerRight.Entities.Count ) < maxNodeEntities )
                    {
                        List<IEntityHolder> ents = new List<IEntityHolder>();
                        ents.AddRange( node.UpperLeft.Entities );
                        ents.AddRange( node.UpperRight.Entities );
                        ents.AddRange( node.LowerLeft.Entities );
                        ents.AddRange( node.LowerRight.Entities );

                        node.Merge();

                        //for ( int i = 0; i < ents.Count; i++ )
                        //{
                        //    OrdenEntity( node, ents[ i ] );
                        //}
                        //Als ent in een van de voormalige children zat, dan zit ent ook in deze node
                        //node.Entities.AddRange( ents );
                        for ( int i = 0; i < ents.Count; i++ )
                        {
                            ents[ i ].MoveToNode( node );
                        }
                    }
                }

            }

        }

        /*public void Render() { Render( this ); }
        public void Render(QuadTreeNode node)
        {
            if ( node == null ) return;
            for ( int i = 0; i < node.Entities.Count; i++ )
            {
                node.Entities[ i ].Render();
            }

            Render( node.UpperLeft );
            Render( node.UpperRight );
            Render( node.LowerLeft );
            Render( node.LowerRight );
        }*/


        /*public void RenderNodeEntityBoundingBox(Common.Engine.LineManager3D lineManager) { RenderNodeEntityBoundingBox( this ); }
        public void RenderNodeEntityBoundingBox(Common.Engine.LineManager3D lineManager, QuadTreeNode node)
        {
            if ( node == null ) return;

            RenderNodeEntityBoundingBox( node.UpperLeft );
            RenderNodeEntityBoundingBox( node.UpperRight );
            RenderNodeEntityBoundingBox( node.LowerLeft );
            RenderNodeEntityBoundingBox( node.LowerRight );

            if ( node.Entities.Count == 0 ) return;
            //FloorLowerLeft = min
            //TopUpperRight = max

            int level = node.CalculateLevel();
            Color col;
            if ( level < levelColors.Length )
                col = levelColors[ level ];
            else
                col = levelColors[ levelColors.Length - 1 ];

            Vector3 radius = node.EntityBoundingBox.Max - node.EntityBoundingBox.Min;
            Vector3 radX = new Vector3( radius.X, 0, 0 );
            Vector3 radY = new Vector3( 0, radius.Y, 0 );
            Vector3 radZ = new Vector3( 0, 0, radius.Z );
            Vector3 min = node.EntityBoundingBox.Min;


            Vector3 fll = min;
            Vector3 flr = min + radX;
            Vector3 ful = min + radZ;
            Vector3 fur = min + radX + radZ;
            Vector3 tll = min + radY;
            Vector3 tlr = min + radY + radX;
            Vector3 tul = min + radY + radZ;
            Vector3 tur = min + radY + radX + radZ; //= max



            //grondvlak
            lineManager.AddLine( fll, flr, col );
            lineManager.AddLine( flr, fur, col );
            lineManager.AddLine( fur, ful, col );
            lineManager.AddLine( ful, fll, col );

            //opstaande ribben
            lineManager.AddLine( fll, tll, col );
            lineManager.AddLine( flr, tlr, col );
            lineManager.AddLine( fur, tur, col );
            lineManager.AddLine( ful, tul, col );

            //bovenvlak
            lineManager.AddLine( tll, tlr, col );
            lineManager.AddLine( tlr, tur, col );
            lineManager.AddLine( tur, tul, col );
            lineManager.AddLine( tul, tll, col );

        }

        public void RenderNodeBoundingBox() { RenderNodeBoundingBox( this ); }
        public void RenderNodeBoundingBox(QuadTreeNode node)
        {
            if ( node == null ) return;

            RenderNodeBoundingBox( node.UpperLeft );
            RenderNodeBoundingBox( node.UpperRight );
            RenderNodeBoundingBox( node.LowerLeft );
            RenderNodeBoundingBox( node.LowerRight );

            //if ( node.IsLeaf == false ) return;
            //FloorLowerLeft = min
            //TopUpperRight = max
            int level = node.CalculateLevel();
            Color col;
            if ( level < levelColors.Length )
                col = levelColors[ level ];
            else
                col = levelColors[ levelColors.Length - 1 ];

            Vector3 radius = node.BoundingBox.Max - node.BoundingBox.Min;
            Vector3 radX = new Vector3( radius.X, 0, 0 );
            Vector3 radY = new Vector3( 0, radius.Y, 0 );
            Vector3 radZ = new Vector3( 0, 0, radius.Z );
            Vector3 min = node.BoundingBox.Min;
            min.Y = -1 + level;


            Vector3 fll = min;
            Vector3 flr = min + radX;
            Vector3 ful = min + radZ;
            Vector3 fur = min + radX + radZ;



            //grondvlak
            ServerClientMain.instance.LineManager3D.AddLine( fll, flr, col );
            ServerClientMain.instance.LineManager3D.AddLine( flr, fur, col );
            ServerClientMain.instance.LineManager3D.AddLine( fur, ful, col );
            ServerClientMain.instance.LineManager3D.AddLine( ful, fll, col );


        }*/






        #region Unit Testing
#if DEBUG
        public static void TestQuadTreeStructure()
        {
            QuadTree q = new QuadTree( new BoundingBox( new Vector3( 0, 0, 0 ), new Vector3( 1000, 1000, 1000 ) ) );
            q.Split();
            q.UpperLeft.Split();
            q.LowerLeft.Dispose();

            System.Windows.Forms.MessageBox.Show( q.BuildString() );


        }

        public static void TestOrdenEntities()
        {
            List<ServerEntityHolder> ents = new List<ServerEntityHolder>();
            QuadTree tree = new QuadTree( new BoundingBox( new Vector3( 0, -1000, 0 ), new Vector3( 1000, 1000, 1000 ) ) );
            Entities.Shuriken001 shur;


            shur = new Entities.Shuriken001();
            shur.Positie = new Vector3( 250, 100, 250 );
            ents.Add( new ServerEntityHolder( shur ) );
            tree.OrdenEntity( ents[ ents.Count - 1 ] );

            shur = new Entities.Shuriken001();
            shur.Positie = new Vector3( 250, 100, 750 );
            ents.Add( new ServerEntityHolder( shur ) );
            tree.OrdenEntity( ents[ ents.Count - 1 ] );

            shur = new Entities.Shuriken001();
            shur.Positie = new Vector3( 750, 100, 250 );
            ents.Add( new ServerEntityHolder( shur ) );
            tree.OrdenEntity( ents[ ents.Count - 1 ] );

            shur = new Entities.Shuriken001();
            shur.Positie = new Vector3( 750, 100, 750 );
            ents.Add( new ServerEntityHolder( shur ) );
            tree.OrdenEntity( ents[ ents.Count - 1 ] );


            System.Windows.Forms.MessageBox.Show( tree.BuildString() );


            shur = new Entities.Shuriken001();
            shur.Positie = new Vector3( 400, 100, 400 );
            ents.Add( new ServerEntityHolder( shur ) );
            tree.OrdenEntity( ents[ ents.Count - 1 ] );

            System.Windows.Forms.MessageBox.Show( tree.ToString() );

            QuadTreeNode n = ents[ ents.Count - 1 ].ContainingNode;
            ents[ ents.Count - 1 ].MoveToNode( null );

            tree.UpdateTreeDepth( n.Parent );

            System.Windows.Forms.MessageBox.Show( tree.BuildString() );

            n = ents[ ents.Count - 2 ].ContainingNode;
            ents[ ents.Count - 2 ].MoveToNode( null );

            tree.UpdateTreeDepth( n.Parent );

            System.Windows.Forms.MessageBox.Show( tree.BuildString() );
        }

        /*public static void TestRender()
        {
            List<ServerEntityHolder> ents = new List<ServerEntityHolder>();
            QuadTree tree = new QuadTree( new BoundingBox( new Vector3( 0, -1000, 0 ), new Vector3( 100, 1000, 100 ) ) );
            Entities.Shuriken001 shur;

            Game3DPlay.SpelObjecten.Spectater spec;
            TestServerClientMain.Start( "TestRenderModel",
                delegate
                {

                    shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
                    shur.Positie = new Vector3( 25, 10, 25 );
                    ents.Add( new ServerEntityHolder( shur ) );
                    tree.OrdenEntity( ents[ ents.Count - 1 ] );

                    shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
                    shur.Positie = new Vector3( 25, 10, 75 );
                    ents.Add( new ServerEntityHolder( shur ) );
                    tree.OrdenEntity( ents[ ents.Count - 1 ] );

                    shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
                    shur.Positie = new Vector3( 75, 10, 25 );
                    ents.Add( new ServerEntityHolder( shur ) );
                    tree.OrdenEntity( ents[ ents.Count - 1 ] );

                    shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
                    shur.Positie = new Vector3( 75, 10, 75 );
                    ents.Add( new ServerEntityHolder( shur ) );
                    tree.OrdenEntity( ents[ ents.Count - 1 ] );

                    shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
                    shur.Positie = new Vector3( 40, 10, 40 );
                    ents.Add( new ServerEntityHolder( shur ) );
                    tree.OrdenEntity( ents[ ents.Count - 1 ] );


                    spec = new MHGameWork.Game3DPlay.SpelObjecten.Spectater( TestServerClientMain.Instance );
                    TestServerClientMain.Instance.SetCamera( spec );

                },
                delegate
                {
                    if ( TestServerClientMain.Instance.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.F ) )
                    {
                        shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
                        shur.Positie = TestServerClientMain.instance.ActiveCamera.CameraPosition;
                        ents.Add( new ServerEntityHolder( shur ) );
                        tree.OrdenEntity( ents[ ents.Count - 1 ] );
                    }

                    if ( TestServerClientMain.Instance.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.G ) )
                    {
                        shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
                        shur.Positie = new Vector3( 5, 10, 5 );
                        ents.Add( new ServerEntityHolder( shur ) );
                        tree.OrdenEntity( ents[ ents.Count - 1 ] );

                        shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
                        shur.Positie = new Vector3( 10, 10, 10 );
                        ents.Add( new ServerEntityHolder( shur ) );
                        tree.OrdenEntity( ents[ ents.Count - 1 ] );

                        shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
                        shur.Positie = new Vector3( 15, 10, 15 );
                        ents.Add( new ServerEntityHolder( shur ) );
                        tree.OrdenEntity( ents[ ents.Count - 1 ] );

                        shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
                        shur.Positie = new Vector3( 20, 10, 20 );
                        ents.Add( new ServerEntityHolder( shur ) );
                        tree.OrdenEntity( ents[ ents.Count - 1 ] );
                    }

                    tree.Render();
                    tree.RenderNodeEntityBoundingBox();
                    tree.RenderNodeBoundingBox();
                } );
        }*/
#endif
        #endregion
    }
}
