using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using TreeGenerator.help;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.Graphics;
namespace TreeGenerator
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : XNAGame
    {
        TreestructureGenerator t = null;
        List<Tree> trees = new List<Tree>();
       List<Leaf> leaves=new List<Leaf>();
        int treeRows = 5;
        int treeCols = 5;
        protected override void Initialize()
        {
            base.Initialize();

            t = new TreestructureGenerator( "hjk" );
            t.LoadData();


            Random rand = new Random();
            rand.Next( 50000 );

            short k = 0;


            for ( int j = 0; j < treeCols; j++ )
            {





                for ( int i = 0; i < treeRows; i++ )
                {




                    Tree tree;



                    tree = t.CreateTree( rand.Next( 50000 ), new Vector3( (float)( i * 5 + ( rand.NextDouble() * 5 - 2.5f ) ), 0, (float)( j * 5 + ( rand.NextDouble() * 5 - 2.5f ) ) ), 7 );


                    tree.Trunk.CreateSegments();
                    Tree.CreateVertices( tree.Trunk );
                    tree.CreateMesh( this );
                    for (int l = 0; l < tree.Trunk.ChildBranches.Count; l++)
                    {
                        leaves.Add(new Leaf(tree.Trunk.ChildBranches[l], 1, 1, 0.5f, l, 0, 0, 0, 0, 0, 0));
                    }
                    

                    trees.Add( tree );
                }
            }
            for (int i = 0; i < leaves.Count; i++)
            {
                leaves[i].CreateVerticesCross();
                leaves[i].CreateMeshCross(this);
            }

        }
        
        protected override void Draw()
        {
            base.Draw();

            /*if ( Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.G ) )
            {
                Random r = new Random();
                tree = t.CreateTree( r.Next( 0, 10000 ), new Vector3( 2.0f, 2.0f, 2.0f ), 11 );
                tree.Trunk.CreateSegments();
                Tree.CreateVertices( tree.Trunk );
                tree.CreateMesh( game );

            }*/

            for ( int i = 0; i < trees.Count; i++ )
            {
                trees[ i ].Render( this );


            }
            for (int i = 0; i < leaves.Count; i++)
            {
                leaves[i].Render(this);
            }




        }

        //public void drawVertices(Branch branch)
        //{
        //    for (int i = 1; i < branch.Segments.Count; i++)
        //    {
        //        for (int j = 0; j < branch.Segments[0].Vertices.Count; j++)
        //        {
        //            Vector3 v1 = branch.Segments[i - 1].Vertices[j];
        //            Vector3 v2 = branch.Segments[i].Vertices[j];
        //            LineManager3D.AddLine(v1, v2, CreateRandomColor(i + 1000 * j));

        //        }
        //    }
        //    for (int i = 0; i < branch.Segments.Count; i++)
        //    {
        //        for (int j = 2; j < branch.Segments[0].Vertices.Count; j++)
        //        {
        //            Vector3 v1 = branch.Segments[i].Vertices[j - 1];
        //            Vector3 v2 = branch.Segments[i].Vertices[j];
        //            LineManager3D.AddLine(v1, v2, CreateRandomColor(i));

        //        }
        //    }

        //    for (int i = 1; i < branch.Segments.Count; i++)
        //    {
        //        for (int j = 0; j < branch.Segments[i].ChildBranches.Count; j++)
        //        {
        //            drawBranch(branch.Segments[i].ChildBranches[j]);

        //        }
        //    }

        //}

        public static Color CreateRandomColor( int seed )
        {
            return Color.Brown;
            seed++;

            Random r = new Random( seed );
            Color c = new Color( (byte)r.Next( 0, 255 ), (byte)r.Next( 0, 255 ), (byte)r.Next( 0, 255 ), 255 );
            return c;
        }
    }
}
