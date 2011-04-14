using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.TWClient
{
    public class QuadTree<T> //: QuadTreeNode
    {

        public QuadTreeNode<T> RootNode;

        public QuadTree( BoundingBox bounding )
        {
            RootNode = new QuadTreeNode<T>( this );
            RootNode.BoundingBox = bounding;
            //BoundingBox = bounding;

            //levelColors = new Color[ 8 ];
            //levelColors[ 0 ] = Color.Red;
            //levelColors[ 1 ] = Color.Orange;
            //levelColors[ 2 ] = Color.Yellow;
            //levelColors[ 3 ] = Color.Purple;
            //levelColors[ 4 ] = Color.LightGreen;
            //levelColors[ 5 ] = Color.Green;
            //levelColors[ 6 ] = Color.Brown;
            //levelColors[ 7 ] = Color.DarkGoldenrod;

        }


        public void Split( QuadTreeNode<T> node )
        {
            if ( node.UpperLeft != null && node.UpperRight != null && node.LowerLeft != null && node.LowerRight != null ) return;
            Vector3 half = new Vector3( node.BoundingBox.Max.X - node.BoundingBox.Min.X, 0, node.BoundingBox.Max.Z - node.BoundingBox.Min.Z );
            half *= 0.5f;


            if ( node.UpperLeft == null )
            {
                node.UpperLeft = CreateChild( node, node.BoundingBox.Min + new Vector3( 0, 0, 0 ), half );
            }
            if ( node.UpperRight == null )
            {
                node.UpperRight = CreateChild( node, node.BoundingBox.Min + new Vector3( half.X, 0, 0 ), half );
            }

            if ( node.LowerLeft == null )
            {
                node.LowerLeft = CreateChild( node, node.BoundingBox.Min + new Vector3( 0, 0, half.Z ), half );
            }
            if ( node.LowerRight == null )
            {
                node.LowerRight = CreateChild( node, node.BoundingBox.Min + new Vector3( half.X, 0, half.Z ), half );
            }

            node.UpperLeft.UpdateAdress();
            node.UpperRight.UpdateAdress();
            node.LowerLeft.UpdateAdress();
            node.LowerRight.UpdateAdress();

        }

        public void Merge( QuadTreeNode<T> node )
        {
            if ( node.UpperLeft != null )
                node.UpperLeft.Dispose();

            if ( node.UpperRight != null )
                node.UpperRight.Dispose();

            if ( node.LowerLeft != null )
                node.LowerLeft.Dispose();

            if ( node.LowerRight != null )
                node.LowerRight.Dispose();

            node.UpperLeft = null;
            node.UpperRight = null;
            node.LowerLeft = null;
            node.LowerRight = null;
        }

        public void SplitToLevel( QuadTreeNode<T> node, int level )
        {
            if ( GetLevel( node ) > level ) return;

            Split( node );
            SplitToLevel( node.UpperLeft, level );
            SplitToLevel( node.UpperRight, level );
            SplitToLevel( node.LowerLeft, level );
            SplitToLevel( node.LowerRight, level );

        }






































        private int maxNodeEntities = 4;
        private Vector2 minNodeSize = new Vector2( 10, 10 );

        private Color[] levelColors;




        //        public bool OrdenEntity( ClientEntityHolder entH ) { return OrdenEntity( this, entH ); }
        //        public bool OrdenEntity( QuadTreeNode node, ClientEntityHolder entH )
        //        {
        //            return OrdenEntity( node, entH, true );
        //        }
        //        /// <summary>
        //        /// 
        //        /// </summary>
        //        /// <param name="node"></param>
        //        /// <param name="ent"></param>
        //        /// <returns>Returns true if 'ent' was added to 'node' or to one of its children </returns>
        //        public bool OrdenEntity( QuadTreeNode node, ClientEntityHolder entH, bool updateDepth )
        //        {
        //            if ( node == null ) return false;
        //            //TODO: contains of intersects?
        //            if ( node.BoundingBox.Contains( entH.BoundingSphere ) != Microsoft.Xna.Framework.ContainmentType.Contains )
        //            {
        //                if ( node == this )
        //                {
        //                    //De ent bevindt zich buiten de Quadtree, voeg toch maar toe
        //                    entH.MoveToNode( node );
        //                    if ( updateDepth ) UpdateTreeDepth( node );
        //                }
        //                else { return false; }

        //            }

        //            if ( !OrdenEntity( node.UpperLeft, entH, updateDepth )
        //                && !OrdenEntity( node.UpperRight, entH, updateDepth )
        //                && !OrdenEntity( node.LowerLeft, entH, updateDepth )
        //                && !OrdenEntity( node.LowerRight, entH, updateDepth ) )
        //            {
        //                //Add entity to this node
        //                entH.MoveToNode( node );
        //                if ( updateDepth ) UpdateTreeDepth( node );

        //            }

        //            //Entity was added to this node or one of its children
        //            return true;

        //        }

        //        public void UpdateTreeDepth( QuadTreeNode node )
        //        {
        //            if ( node == null ) return;
        //            if ( node.IsLeaf )
        //            {
        //                if ( node.Entities.Count > maxNodeEntities )
        //                {
        //                    Vector3 diff = node.BoundingBox.Max - node.BoundingBox.Min;

        //                    //TODO: delen door 2 of maal 1/2?

        //                    if ( ( diff.X / 2 ) >= minNodeSize.X && ( diff.Z / 2 ) >= minNodeSize.Y )
        //                    {

        //                        List<ClientEntityHolder> ents = new List<ClientEntityHolder>( node.Entities );
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

        //                        for ( int i = 0; i < ents.Count; i++ )
        //                        {
        //                            OrdenEntity( node, ents[ i ], false );
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                UpdateTreeDepth( node.UpperLeft );
        //                UpdateTreeDepth( node.UpperRight );
        //                UpdateTreeDepth( node.LowerLeft );
        //                UpdateTreeDepth( node.LowerRight );

        //                if ( node.CanMerge )
        //                {
        //                    //Check if we need to merge
        //                    //NOTE: < maxNodeEntities zou normal gezien <= maxNodeEntities moeten zijn
        //                    //		maar dit voorkomt mss een overvloedig mergen-splitten.
        //                    if ( ( node.UpperLeft.Entities.Count
        //                            + node.UpperRight.Entities.Count
        //                            + node.LowerLeft.Entities.Count
        //                            + node.LowerRight.Entities.Count ) < maxNodeEntities )
        //                    {
        //                        List<ClientEntityHolder> ents = new List<ClientEntityHolder>();
        //                        ents.AddRange( node.UpperLeft.Entities );
        //                        ents.AddRange( node.UpperRight.Entities );
        //                        ents.AddRange( node.LowerLeft.Entities );
        //                        ents.AddRange( node.LowerRight.Entities );

        //                        node.Merge();

        //                        //for ( int i = 0; i < ents.Count; i++ )
        //                        //{
        //                        //    OrdenEntity( node, ents[ i ] );
        //                        //}
        //                        //Als ent in een van de voormalige children zat, dan zit ent ook in deze node
        //                        //node.Entities.AddRange( ents );
        //                        for ( int i = 0; i < ents.Count; i++ )
        //                        {
        //                            ents[ i ].MoveToNode( node );
        //                        }
        //                    }
        //                }

        //            }

        //        }

        //public void Render() { Render( this ); }
        //public void Render( QuadTreeNode node )
        //{
        //    if ( node == null ) return;
        //    for ( int i = 0; i < node.Entities.Count; i++ )
        //    {
        //        node.Entities[ i ].Render();
        //    }

        //    Render( node.UpperLeft );
        //    Render( node.UpperRight );
        //    Render( node.LowerLeft );
        //    Render( node.LowerRight );
        //}


        //public void RenderNodeEntityBoundingBox() { RenderNodeEntityBoundingBox( this ); }
        //public void RenderNodeEntityBoundingBox( QuadTreeNode node )
        //{
        //    if ( node == null ) return;

        //    RenderNodeEntityBoundingBox( node.UpperLeft );
        //    RenderNodeEntityBoundingBox( node.UpperRight );
        //    RenderNodeEntityBoundingBox( node.LowerLeft );
        //    RenderNodeEntityBoundingBox( node.LowerRight );

        //    //if ( node.Entities.Count ==0 ) return;
        //    //FloorLowerLeft = min
        //    //TopUpperRight = max

        //    if ( node.EntityBoundingBox.Min.X == node.EntityBoundingBox.Max.X
        //        || node.EntityBoundingBox.Min.Y == node.EntityBoundingBox.Max.Y
        //        || node.EntityBoundingBox.Min.Z == node.EntityBoundingBox.Max.Z ) return;

        //    int level = node.CalculateLevel();
        //    Color col;
        //    if ( level < levelColors.Length )
        //        col = levelColors[ level ];
        //    else
        //        col = levelColors[ levelColors.Length - 1 ];

        //    RenderBoundingBox( node.EntityBoundingBox, col );
        //    /*Vector3 radius = node.EntityBoundingBox.Max - node.EntityBoundingBox.Min;
        //    Vector3 radX = new Vector3( radius.X, 0, 0 );
        //    Vector3 radY = new Vector3( 0, radius.Y, 0 );
        //    Vector3 radZ = new Vector3( 0, 0, radius.Z );
        //    Vector3 min = node.EntityBoundingBox.Min;


        //    Vector3 fll = min;
        //    Vector3 flr = min + radX;
        //    Vector3 ful = min + radZ;
        //    Vector3 fur = min + radX + radZ;
        //    Vector3 tll = min + radY;
        //    Vector3 tlr = min + radY + radX;
        //    Vector3 tul = min + radY + radZ;
        //    Vector3 tur = min + radY + radX + radZ; //= max



        //    //grondvlak
        //    ServerClientMain.instance.LineManager3D.AddLine( fll, flr, col );
        //    ServerClientMain.instance.LineManager3D.AddLine( flr, fur, col );
        //    ServerClientMain.instance.LineManager3D.AddLine( fur, ful, col );
        //    ServerClientMain.instance.LineManager3D.AddLine( ful, fll, col );

        //    //opstaande ribben
        //    ServerClientMain.instance.LineManager3D.AddLine( fll, tll, col );
        //    ServerClientMain.instance.LineManager3D.AddLine( flr, tlr, col );
        //    ServerClientMain.instance.LineManager3D.AddLine( fur, tur, col );
        //    ServerClientMain.instance.LineManager3D.AddLine( ful, tul, col );

        //    //bovenvlak
        //    ServerClientMain.instance.LineManager3D.AddLine( tll, tlr, col );
        //    ServerClientMain.instance.LineManager3D.AddLine( tlr, tur, col );
        //    ServerClientMain.instance.LineManager3D.AddLine( tur, tul, col );
        //    ServerClientMain.instance.LineManager3D.AddLine( tul, tll, col );*/

        //}

        //public void RenderBoundingBox( BoundingBox box, Color col )
        //{
        //    Vector3 radius = box.Max - box.Min;
        //    Vector3 radX = new Vector3( radius.X, 0, 0 );
        //    Vector3 radY = new Vector3( 0, radius.Y, 0 );
        //    Vector3 radZ = new Vector3( 0, 0, radius.Z );
        //    Vector3 min = box.Min;



        //    Vector3 fll = min;
        //    Vector3 flr = min + radX;
        //    Vector3 ful = min + radZ;
        //    Vector3 fur = min + radX + radZ;
        //    Vector3 tll = min + radY;
        //    Vector3 tlr = min + radY + radX;
        //    Vector3 tul = min + radY + radZ;
        //    Vector3 tur = min + radY + radX + radZ; //= max



        //    //grondvlak
        //    ServerClientMainOud.instance.LineManager3D.AddLine( fll, flr, col );
        //    ServerClientMainOud.instance.LineManager3D.AddLine( flr, fur, col );
        //    ServerClientMainOud.instance.LineManager3D.AddLine( fur, ful, col );
        //    ServerClientMainOud.instance.LineManager3D.AddLine( ful, fll, col );

        //    //opstaande ribben
        //    ServerClientMainOud.instance.LineManager3D.AddLine( fll, tll, col );
        //    ServerClientMainOud.instance.LineManager3D.AddLine( flr, tlr, col );
        //    ServerClientMainOud.instance.LineManager3D.AddLine( fur, tur, col );
        //    ServerClientMainOud.instance.LineManager3D.AddLine( ful, tul, col );

        //    //bovenvlak
        //    ServerClientMainOud.instance.LineManager3D.AddLine( tll, tlr, col );
        //    ServerClientMainOud.instance.LineManager3D.AddLine( tlr, tur, col );
        //    ServerClientMainOud.instance.LineManager3D.AddLine( tur, tul, col );
        //    ServerClientMainOud.instance.LineManager3D.AddLine( tul, tll, col );


        //    //diagonalen
        //    ServerClientMainOud.instance.LineManager3D.AddLine( tll, flr, col );
        //    ServerClientMainOud.instance.LineManager3D.AddLine( fll, tlr, col );

        //    ServerClientMainOud.instance.LineManager3D.AddLine( tlr, fur, col );
        //    ServerClientMainOud.instance.LineManager3D.AddLine( flr, tur, col );

        //    ServerClientMainOud.instance.LineManager3D.AddLine( tur, ful, col );
        //    ServerClientMainOud.instance.LineManager3D.AddLine( fur, tul, col );

        //    ServerClientMainOud.instance.LineManager3D.AddLine( tul, fll, col );
        //    ServerClientMainOud.instance.LineManager3D.AddLine( ful, tll, col );


        //}

        //public void RenderNodeBoundingBox() { RenderNodeBoundingBox( this ); }
        //public void RenderNodeBoundingBox( QuadTreeNode<T> node )
        //{
        //    if ( node == null ) return;

        //    RenderNodeBoundingBox( node.UpperLeft );
        //    RenderNodeBoundingBox( node.UpperRight );
        //    RenderNodeBoundingBox( node.LowerLeft );
        //    RenderNodeBoundingBox( node.LowerRight );

        //    //if ( node.IsLeaf == false ) return;
        //    //FloorLowerLeft = min
        //    //TopUpperRight = max
        //    int level = node.CalculateLevel();
        //    Color col;
        //    if ( level < levelColors.Length )
        //        col = levelColors[ level ];
        //    else
        //        col = levelColors[ levelColors.Length - 1 ];

        //    Vector3 radius = node.BoundingBox.Max - node.BoundingBox.Min;
        //    Vector3 radX = new Vector3( radius.X, 0, 0 );
        //    Vector3 radY = new Vector3( 0, radius.Y, 0 );
        //    Vector3 radZ = new Vector3( 0, 0, radius.Z );
        //    Vector3 min = node.BoundingBox.Min;
        //    min.Y = -1 + level;


        //    Vector3 fll = min;
        //    Vector3 flr = min + radX;
        //    Vector3 ful = min + radZ;
        //    Vector3 fur = min + radX + radZ;



        //    //grondvlak
        //    ServerClientMainOud.instance.LineManager3D.AddLine( fll, flr, col );
        //    ServerClientMainOud.instance.LineManager3D.AddLine( flr, fur, col );
        //    ServerClientMainOud.instance.LineManager3D.AddLine( fur, ful, col );
        //    ServerClientMainOud.instance.LineManager3D.AddLine( ful, fll, col );


        //}





        //protected abstract QuadTreeNode<T> CreateChild( Vector3 min, Vector3 size );


        public virtual void RemoveChild( QuadTreeNode<T> node, QuadTreeNode<T> child )
        {
            if ( node.UpperLeft == child )
            {
                node.UpperLeft = null;
            }
            if ( node.UpperRight == child )
            {
                node.UpperRight = null;
            }
            if ( node.LowerLeft == child )
            {
                node.LowerLeft = null;
            }
            if ( node.LowerRight == child )
            {
                node.LowerRight = null;
            }
        }

        public int GetLevel( QuadTreeNode<T> node )
        {
            return GetLevel( node.Address );
        }

        public int GetLevel( ulong _address )
        {
            //First four bytes of address
            return (int)( _address & ( ( 1 << 0 ) + ( 1 << 1 ) + ( 1 << 2 ) + ( 1 << 3 ) ) );
        }
        public QuadTreeNode<T>.ChildDir GetChildDirAtLevel( ulong _address, int level )
        {
            if ( level == 0 ) throw new ArgumentException( "Value cannot be 0", "level" );
            return (QuadTreeNode<T>.ChildDir)( ( _address >> ( 4 + 2 * ( level - 1 ) ) ) & ( ( 1 << 0 ) + ( 1 << 1 ) ) );
        }


        public QuadTreeNode<T> GetChild( QuadTreeNode<T> node, QuadTreeNode<T>.ChildDir dir )
        {
            switch ( dir )
            {
                case QuadTreeNode<T>.ChildDir.UpperLeft:
                    return node.UpperLeft;
                case QuadTreeNode<T>.ChildDir.UpperRight:
                    return node.UpperRight;
                case QuadTreeNode<T>.ChildDir.LowerLeft:
                    return node.LowerLeft;
                case QuadTreeNode<T>.ChildDir.LowerRight:
                    return node.LowerRight;
                default:
                    throw new ArgumentException( "Invalid value", "dir" );
            }

        }

        public QuadTreeNode<T> FindOrCreateNode( QuadTreeNode<T> node, ulong nAddress )
        {
            int level = GetLevel( node );
            int targetLevel = GetLevel( nAddress );

            if ( level < targetLevel )
            {
                //Need to go deeper.

                //Split this node. If this node allready has children then the call we be omitted.
                Split( node );

                return FindOrCreateNode( GetChild( node, GetChildDirAtLevel( nAddress, level + 1 ) ), nAddress );


            }
            else if ( level == targetLevel )
            {
                if ( node.Address == nAddress )
                {
                    //Yes!
                    return node;
                }
                else
                {
                    //Oo, error
                    throw new InvalidOperationException();
                }
            }
            else
            {
                //level > targetLevel

                throw new InvalidOperationException( "The node you are looking for is not a child of this node!! (or other issue)" );
            }

        }


        protected QuadTreeNode<T> CreateChild( QuadTreeNode<T> parent, Vector3 min, Vector3 size )
        {
            QuadTreeNode<T> node = new QuadTreeNode<T>( this );
            node.Parent = parent;
            BoundingBox bb = new BoundingBox( min, min + size );
            bb.Min.Y = parent.BoundingBox.Min.Y;
            bb.Max.Y = parent.BoundingBox.Max.Y;
            node.BoundingBox = bb;

            return node;
        }







        #region Unit Testing
#if DEBUG
        public static void TestQuadTreeStructure()
        {
            QuadTree<object> q = new QuadTree<object>( new BoundingBox( new Vector3( 0, 0, 0 ), new Vector3( 1000, 1000, 1000 ) ) );

            throw new Exception( "TODOException" );
            //q.Split();
            //q.UpperLeft.Split();
            //q.LowerLeft.Dispose();

            //System.Windows.Forms.MessageBox.Show( q.BuildString() );


        }

        //public static void TestOrdenEntities()
        //{
        //    List<ClientEntityHolder> ents = new List<ClientEntityHolder>();
        //    QuadTree tree = new QuadTree( new BoundingBox( new Vector3( 0, -1000, 0 ), new Vector3( 1000, 1000, 1000 ) ) );
        //    Entities.Shuriken001 shur;


        //    shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
        //    shur.Positie = new Vector3( 250, 100, 250 );
        //    ents.Add( new ClientEntityHolder( shur ) );
        //    tree.OrdenEntity( ents[ ents.Count - 1 ] );

        //    shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
        //    shur.Positie = new Vector3( 250, 100, 750 );
        //    ents.Add( new ClientEntityHolder( shur ) );
        //    tree.OrdenEntity( ents[ ents.Count - 1 ] );

        //    shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
        //    shur.Positie = new Vector3( 750, 100, 250 );
        //    ents.Add( new ClientEntityHolder( shur ) );
        //    tree.OrdenEntity( ents[ ents.Count - 1 ] );

        //    shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
        //    shur.Positie = new Vector3( 750, 100, 750 );
        //    ents.Add( new ClientEntityHolder( shur ) );
        //    tree.OrdenEntity( ents[ ents.Count - 1 ] );


        //    System.Windows.Forms.MessageBox.Show( tree.BuildString() );


        //    shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
        //    shur.Positie = new Vector3( 400, 100, 400 );
        //    ents.Add( new ClientEntityHolder( shur ) );
        //    tree.OrdenEntity( ents[ ents.Count - 1 ] );

        //    System.Windows.Forms.MessageBox.Show( tree.ToString() );

        //    QuadTreeNode n = ents[ ents.Count - 1 ].Node;
        //    ents[ ents.Count - 1 ].MoveToNode( null );

        //    tree.UpdateTreeDepth( n.Parent );

        //    System.Windows.Forms.MessageBox.Show( tree.BuildString() );

        //    n = ents[ ents.Count - 2 ].Node;
        //    ents[ ents.Count - 2 ].MoveToNode( null );

        //    tree.UpdateTreeDepth( n.Parent );

        //    System.Windows.Forms.MessageBox.Show( tree.BuildString() );
        //}

        //public static void TestRender()
        //{
        //    List<ClientEntityHolder> ents = new List<ClientEntityHolder>();
        //    QuadTree tree = new QuadTree( new BoundingBox( new Vector3( 0, -1000, 0 ), new Vector3( 100, 1000, 100 ) ) );
        //    Entities.Shuriken001 shur;

        //    Game3DPlay.SpelObjecten.Spectater spec;
        //    TestServerClientMain.Start( "TestRenderModel",
        //        delegate
        //        {

        //            shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
        //            shur.Positie = new Vector3( 25, 10, 25 );
        //            ents.Add( new ClientEntityHolder( shur ) );
        //            tree.OrdenEntity( ents[ ents.Count - 1 ] );

        //            shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
        //            shur.Positie = new Vector3( 25, 10, 75 );
        //            ents.Add( new ClientEntityHolder( shur ) );
        //            tree.OrdenEntity( ents[ ents.Count - 1 ] );

        //            shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
        //            shur.Positie = new Vector3( 75, 10, 25 );
        //            ents.Add( new ClientEntityHolder( shur ) );
        //            tree.OrdenEntity( ents[ ents.Count - 1 ] );

        //            shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
        //            shur.Positie = new Vector3( 75, 10, 75 );
        //            ents.Add( new ClientEntityHolder( shur ) );
        //            tree.OrdenEntity( ents[ ents.Count - 1 ] );

        //            shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
        //            shur.Positie = new Vector3( 40, 10, 40 );
        //            ents.Add( new ClientEntityHolder( shur ) );
        //            tree.OrdenEntity( ents[ ents.Count - 1 ] );


        //            spec = new MHGameWork.Game3DPlay.SpelObjecten.Spectater( TestServerClientMain.Instance );
        //            TestServerClientMain.Instance.SetCamera( spec );

        //        },
        //        delegate
        //        {
        //            if ( TestServerClientMain.Instance.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.F ) )
        //            {
        //                shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
        //                shur.Positie = TestServerClientMain.instance.ActiveCamera.CameraPosition;
        //                ents.Add( new ClientEntityHolder( shur ) );
        //                tree.OrdenEntity( ents[ ents.Count - 1 ] );
        //            }

        //            if ( TestServerClientMain.Instance.ProcessEventArgs.Keyboard.IsKeyStateDown( Microsoft.Xna.Framework.Input.Keys.G ) )
        //            {
        //                shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
        //                shur.Positie = new Vector3( 5, 10, 5 );
        //                ents.Add( new ClientEntityHolder( shur ) );
        //                tree.OrdenEntity( ents[ ents.Count - 1 ] );

        //                shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
        //                shur.Positie = new Vector3( 10, 10, 10 );
        //                ents.Add( new ClientEntityHolder( shur ) );
        //                tree.OrdenEntity( ents[ ents.Count - 1 ] );

        //                shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
        //                shur.Positie = new Vector3( 15, 10, 15 );
        //                ents.Add( new ClientEntityHolder( shur ) );
        //                tree.OrdenEntity( ents[ ents.Count - 1 ] );

        //                shur = new MHGameWork.TheWizards.ServerClient.Entities.Shuriken001();
        //                shur.Positie = new Vector3( 20, 10, 20 );
        //                ents.Add( new ClientEntityHolder( shur ) );
        //                tree.OrdenEntity( ents[ ents.Count - 1 ] );
        //            }

        //            tree.Render();
        //            tree.RenderNodeEntityBoundingBox();
        //            tree.RenderNodeBoundingBox();
        //        } );
        //}
#endif
        #endregion
    }
}
