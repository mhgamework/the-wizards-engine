using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.Graphics;

namespace TreeGenerator.help
{
    public class TreestructureGenerator
    {
        public string TreeTypeXML;
        public string TreeType;
        private int NumNiveaus;
        public List<TreeStructureLevel> levels = new List<TreeStructureLevel>();

        float spreadingTrees;

        public TreestructureGenerator( string treeTypeXML )
        { TreeTypeXML = treeTypeXML; }

        public void LoadData() //Todo load data from XML
        {

            //TreeType = "Conifer";
            //TreeType = "Fat2";
            TreeType = "Pine";
            if ( TreeType == "Conifer" || TreeType == "conifer" )
            {
                TreeStructureLevel l;

                l = new TreeStructureLevel();
                l.LengthBranchMax = 5;
                l.LengthBranchMin = 3;
                l.DiameterBranchMax = 0.1f;
                l.DiameterBranchMin = 0.1f;
                l.NumBranchMax = 20;
                l.NumBranchMin = 15;
                l.NumSegments = 20;
                l.TaperMax = 0.5f;
                l.TaperMin = 0.2f;
                l.BranchStartRatio = 0.5f;
                l.BranchEndRatio = 1f;
                l.WobbleDropAngleBranchMin = -0.10f;
                l.WobbleDropAngleBranchMax = 0.1f;
                l.WobbleAxialSplitBranchMin = -0.1f;
                l.WobbleAxialSplitBranchMax = 0.1f;


                levels.Add( l );

                l = new TreeStructureLevel();
                l.LengthBranchMax = 5.5f;
                l.LengthBranchMin = 1.5f;
                l.DiameterBranchMax = 0.05f;
                l.DiameterBranchMin = 0.05f;
                l.NumBranchMax = 10;
                l.NumBranchMin = 5;
                l.NumSegments = 20;
                l.TaperMax = 0.5f;
                l.TaperMin = 0.2f;
                l.DropAngleBranchesMin = MathHelper.PiOver4;
                l.DropAngleBranchMax = MathHelper.PiOver4;
                l.AxialSplitBranchMax = MathHelper.TwoPi;
                l.AxialSplitBranchMin = 0;
                l.BranchStartRatio = 0.5f;
                l.BranchEndRatio = 1f;
                l.WobbleDropAngleBranchMax = 0.4f;
                l.WobbleDropAngleBranchMin = -0.1f;
                levels.Add( l );


                l = new TreeStructureLevel();
                l.LengthBranchMax = 1.5f;
                l.LengthBranchMin = 0.5f;
                l.DiameterBranchMax = 0.02f;
                l.DiameterBranchMin = 0.05f;
                l.NumBranchMax = 7;
                l.NumBranchMin = 3;
                l.NumSegments = 10;
                l.TaperMax = 0.5f;
                l.TaperMin = 0.2f;
                l.DropAngleBranchesMin = MathHelper.PiOver4;
                l.DropAngleBranchMax = MathHelper.PiOver4;
                l.AxialSplitBranchMax = MathHelper.TwoPi;
                l.AxialSplitBranchMin = 0;
                l.BranchStartRatio = 0.25f;
                l.WobbleDropAngleBranchMax = -0.1f;
                l.WobbleDropAngleBranchMin = 0.1f;

                levels.Add( l );


            }
            else if ( TreeType == "Pine" )
            {
                TreeStructureLevel l;

                l = new TreeStructureLevel();
                l.LengthBranchMax = 14;
                l.LengthBranchMin = 10;
                l.BranchLengthDecrase = 0.65f;
                l.DiameterBranchMax = 0.5f;
                l.DiameterBranchMin = 0.4f;
                l.NumBranchMax = 50;
                l.NumBranchMin = 40;
                l.NumSegments = 20;
                l.TaperMax = 0.1f;
                l.TaperMin = 0.05f;
                l.BranchStartRatio = 0.15f;
                l.BranchEndRatio = .95f;
                l.WobbleDropAngleBranchMin = -0.05f;
                l.WobbleDropAngleBranchMax = 0.05f;
                l.WobbleAxialSplitBranchMin = -0.1f;
                l.WobbleAxialSplitBranchMax = 0.1f;


                levels.Add( l );


                l = new TreeStructureLevel();
                l.LengthBranchMax = 4.5f;
                l.LengthBranchMin = 4.0f;
                l.BranchLengthDecrase = .7f;
                l.DiameterBranchMax = 0.05f;
                l.DiameterBranchMin = 0.05f;
                l.NumBranchMax = 7;
                l.NumBranchMin = 5;
                l.NumSegments = 10;
                l.TaperMax = 0.3f;
                l.TaperMin = 0.1f;
                l.DropAngleBranchesMin = MathHelper.PiOver2 - 0.8f;
                l.DropAngleBranchMax = MathHelper.PiOver2 - 0.4f;
                l.AxialSplitBranchMax = MathHelper.TwoPi;
                l.AxialSplitBranchMin = 0;
                l.BranchStartRatio = 0.25f;
                l.BranchEndRatio = 1f;
                l.WobbleDropAngleBranchMax = 0.35f;
                l.WobbleDropAngleBranchMin = -0.05f;
                l.WobbleAxialSplitBranchMin = -0.3f;
                l.WobbleAxialSplitBranchMax = 0.3f;
                levels.Add( l );

                l = new TreeStructureLevel();
                l.LengthBranchMax = 1.5f;
                l.LengthBranchMin = 1.0f;
                l.BranchLengthDecrase = 1.0f;
                l.DiameterBranchMax = 0.05f;
                l.DiameterBranchMin = 0.05f;
                l.NumBranchMax = 10;
                l.NumBranchMin = 5;
                l.NumSegments = 5;
                l.TaperMax = 0.3f;
                l.TaperMin = 0.1f;
                l.DropAngleBranchesMin = MathHelper.PiOver2 - 0.8f;
                l.DropAngleBranchMax = MathHelper.PiOver2 - 0.4f;
                l.AxialSplitBranchMax = MathHelper.TwoPi;
                l.AxialSplitBranchMin = 0;
                l.BranchStartRatio = 0.25f;
                l.BranchEndRatio = 1f;
                l.WobbleDropAngleBranchMax = 0.35f;
                l.WobbleDropAngleBranchMin = -0.05f;
                l.WobbleAxialSplitBranchMin = -0.3f;
                l.WobbleAxialSplitBranchMax = 0.3f;
                levels.Add( l );
                

                return;

                l = new TreeStructureLevel();
                l.LengthBranchMax = 4.5f;
                l.LengthBranchMin = 4.0f;
                l.DiameterBranchMax = 0.02f;
                l.DiameterBranchMin = 0.05f;
                l.NumBranchMax = 7;
                l.NumBranchMin = 3;
                l.NumSegments = 10;
                l.TaperMax = 0.5f;
                l.TaperMin = 0.2f;
                l.DropAngleBranchesMin = MathHelper.PiOver4 - 0.2f;
                l.DropAngleBranchMax = MathHelper.PiOver4;
                l.AxialSplitBranchMax = MathHelper.TwoPi;
                l.AxialSplitBranchMin = 0;
                l.BranchStartRatio = 0.25f;
                l.WobbleDropAngleBranchMax = -0.1f;
                l.WobbleDropAngleBranchMin = 0.1f;

                levels.Add( l );


                l = new TreeStructureLevel();
                l.LengthBranchMax = 1.5f;
                l.LengthBranchMin = 0.5f;
                l.DiameterBranchMax = 0.02f;
                l.DiameterBranchMin = 0.05f;
                l.NumBranchMax = 7;
                l.NumBranchMin = 3;
                l.NumSegments = 10;
                l.TaperMax = 0.5f;
                l.TaperMin = 0.2f;
                l.DropAngleBranchesMin = MathHelper.PiOver4;
                l.DropAngleBranchMax = MathHelper.PiOver4;
                l.AxialSplitBranchMax = MathHelper.TwoPi;
                l.AxialSplitBranchMin = 0;
                l.BranchStartRatio = 0.25f;
                l.WobbleDropAngleBranchMax = -0.1f;
                l.WobbleDropAngleBranchMin = 0.1f;



                levels.Add( l );

                return;

                l = new TreeStructureLevel();
                l.LengthBranchMax = 1.5f;
                l.LengthBranchMin = 0.5f;
                l.DiameterBranchMax = 0.02f;
                l.DiameterBranchMin = 0.05f;
                l.NumBranchMax = 7;
                l.NumBranchMin = 3;
                l.NumSegments = 10;
                l.TaperMax = 0.5f;
                l.TaperMin = 0.2f;
                l.DropAngleBranchesMin = MathHelper.PiOver4;
                l.DropAngleBranchMax = MathHelper.PiOver4;
                l.AxialSplitBranchMax = MathHelper.TwoPi;
                l.AxialSplitBranchMin = 0;
                l.BranchStartRatio = 0.25f;
                l.WobbleDropAngleBranchMax = -0.1f;
                l.WobbleDropAngleBranchMin = 0.1f;


                levels.Add( l );


            }
            else if ( TreeType == "Fat" )
            {
                TreeStructureLevel l;

                l = new TreeStructureLevel();
                l.LengthBranchMax = 14;
                l.LengthBranchMin = 10;
                l.BranchLengthDecrase = 0.8f;
                l.DiameterBranchMax = 0.5f;
                l.DiameterBranchMin = 0.4f;
                l.NumBranchMax = 20;
                l.NumBranchMin = 15;
                l.NumSegments = 20;
                l.TaperMax = 0.1f;
                l.TaperMin = 0.05f;
                l.BranchStartRatio = 0.1f;
                l.BranchEndRatio = 1.0f;
                l.WobbleDropAngleBranchMin = -0.05f;
                l.WobbleDropAngleBranchMax = 0.05f;
                l.WobbleAxialSplitBranchMin = -0.1f;
                l.WobbleAxialSplitBranchMax = 0.1f;


                levels.Add( l );


                l = new TreeStructureLevel();
                l.LengthBranchMax = 12.5f;
                l.LengthBranchMin = 8.5f;
                l.BranchLengthDecrase = 1.0f;
                l.DiameterBranchMax = 0.05f;
                l.DiameterBranchMin = 0.05f;
                l.NumBranchMax = 10;
                l.NumBranchMin = 5;
                l.NumSegments = 10;
                l.TaperMax = 0.5f;
                l.TaperMin = 0.2f;
                l.DropAngleBranchesMin = MathHelper.PiOver4 + 0.0f;
                l.DropAngleBranchMax = MathHelper.PiOver4 + 0.1f;
                l.AxialSplitBranchMax = MathHelper.TwoPi;
                l.AxialSplitBranchMin = 0;
                l.BranchStartRatio = 0.25f;
                l.BranchEndRatio = 1f;
                l.WobbleDropAngleBranchMax = 0.2f;
                l.WobbleDropAngleBranchMin = -0.4f;
                levels.Add( l );

                return;
                l = new TreeStructureLevel();
                l.LengthBranchMax = 4.5f;
                l.LengthBranchMin = 4.0f;
                l.DiameterBranchMax = 0.02f;
                l.DiameterBranchMin = 0.05f;
                l.NumBranchMax = 7;
                l.NumBranchMin = 3;
                l.NumSegments = 10;
                l.TaperMax = 0.5f;
                l.TaperMin = 0.2f;
                l.DropAngleBranchesMin = MathHelper.PiOver4 - 0.2f;
                l.DropAngleBranchMax = MathHelper.PiOver4;
                l.AxialSplitBranchMax = MathHelper.TwoPi;
                l.AxialSplitBranchMin = 0;
                l.BranchStartRatio = 0.25f;
                l.WobbleDropAngleBranchMax = -0.1f;
                l.WobbleDropAngleBranchMin = 0.1f;

                levels.Add( l );


                return;
                l = new TreeStructureLevel();
                l.LengthBranchMax = 1.5f;
                l.LengthBranchMin = 0.5f;
                l.DiameterBranchMax = 0.02f;
                l.DiameterBranchMin = 0.05f;
                l.NumBranchMax = 7;
                l.NumBranchMin = 3;
                l.NumSegments = 10;
                l.TaperMax = 0.5f;
                l.TaperMin = 0.2f;
                l.DropAngleBranchesMin = MathHelper.PiOver4;
                l.DropAngleBranchMax = MathHelper.PiOver4;
                l.AxialSplitBranchMax = MathHelper.TwoPi;
                l.AxialSplitBranchMin = 0;
                l.BranchStartRatio = 0.25f;
                l.WobbleDropAngleBranchMax = -0.1f;
                l.WobbleDropAngleBranchMin = 0.1f;



                levels.Add( l );



                l = new TreeStructureLevel();
                l.LengthBranchMax = 1.5f;
                l.LengthBranchMin = 0.5f;
                l.DiameterBranchMax = 0.02f;
                l.DiameterBranchMin = 0.05f;
                l.NumBranchMax = 7;
                l.NumBranchMin = 3;
                l.NumSegments = 10;
                l.TaperMax = 0.5f;
                l.TaperMin = 0.2f;
                l.DropAngleBranchesMin = MathHelper.PiOver4;
                l.DropAngleBranchMax = MathHelper.PiOver4;
                l.AxialSplitBranchMax = MathHelper.TwoPi;
                l.AxialSplitBranchMin = 0;
                l.BranchStartRatio = 0.25f;
                l.WobbleDropAngleBranchMax = -0.1f;
                l.WobbleDropAngleBranchMin = 0.1f;


                levels.Add( l );


            }
            else if ( TreeType == "Fat2" )
            {
                TreeStructureLevel l;

                l = new TreeStructureLevel();
                l.LengthBranchMax = 14;
                l.LengthBranchMin = 10;
                l.BranchLengthDecrase = 0.8f;
                l.DiameterBranchMax = 0.5f;
                l.DiameterBranchMin = 0.4f;
                l.NumBranchMax = 1;
                l.NumBranchMin = 1;
                l.NumSegments = 20;
                l.TaperMax = 0.1f;
                l.TaperMin = 0.05f;
                l.BranchStartRatio = 0.25f;
                l.BranchEndRatio = 1.0f;
                l.WobbleDropAngleBranchMin = 0;
                l.WobbleDropAngleBranchMax = 0;
                l.WobbleAxialSplitBranchMin = 0;
                l.WobbleAxialSplitBranchMax = 0;


                levels.Add( l );


                l = new TreeStructureLevel();
                l.LengthBranchMax = 12.5f;
                l.LengthBranchMin = 8.5f;
                l.BranchLengthDecrase = 1f;
                l.DiameterBranchMax = 0.05f;
                l.DiameterBranchMin = 0.05f;
                l.NumBranchMax = 1;
                l.NumBranchMin = 1;
                l.NumSegments = 5;
                l.TaperMax = 0.5f;
                l.TaperMin = 0.2f;
                l.DropAngleBranchesMin = MathHelper.PiOver4;
                l.DropAngleBranchMax = MathHelper.PiOver4;
                l.AxialSplitBranchMax = MathHelper.PiOver4;
                l.AxialSplitBranchMin = MathHelper.PiOver4;
                l.BranchStartRatio = 0.25f;
                l.BranchEndRatio = 1f;
                l.WobbleDropAngleBranchMax = MathHelper.PiOver4;
                l.WobbleDropAngleBranchMin = MathHelper.PiOver4;
                l.WobbleAxialSplitBranchMin = MathHelper.PiOver4;
                l.WobbleAxialSplitBranchMax = MathHelper.PiOver4;
                levels.Add( l );

                return;
                l = new TreeStructureLevel();
                l.LengthBranchMax = 4.5f;
                l.LengthBranchMin = 4.0f;
                l.DiameterBranchMax = 0.02f;
                l.DiameterBranchMin = 0.05f;
                l.NumBranchMax = 7;
                l.NumBranchMin = 3;
                l.NumSegments = 5;
                l.TaperMax = 10.5f;
                l.TaperMin = 10.5f;
                l.DropAngleBranchesMin = MathHelper.PiOver4;
                l.DropAngleBranchMax = MathHelper.PiOver4;
                l.AxialSplitBranchMax = MathHelper.PiOver4;
                l.AxialSplitBranchMin = MathHelper.PiOver4;
                l.BranchStartRatio = 0.25f;
                l.WobbleDropAngleBranchMax = MathHelper.PiOver4 * 1.0f;
                l.WobbleDropAngleBranchMin = MathHelper.PiOver4 * 1.0f;

                levels.Add( l );
                return;

                l = new TreeStructureLevel();
                l.LengthBranchMax = 1.5f;
                l.LengthBranchMin = 0.5f;
                l.DiameterBranchMax = 0.02f;
                l.DiameterBranchMin = 0.05f;
                l.NumBranchMax = 7;
                l.NumBranchMin = 3;
                l.NumSegments = 10;
                l.TaperMax = 0.5f;
                l.TaperMin = 0.2f;
                l.DropAngleBranchesMin = MathHelper.PiOver4;
                l.DropAngleBranchMax = MathHelper.PiOver4;
                l.AxialSplitBranchMax = MathHelper.TwoPi;
                l.AxialSplitBranchMin = 0;
                l.BranchStartRatio = 0.25f;
                l.WobbleDropAngleBranchMax = -0.1f;
                l.WobbleDropAngleBranchMin = 0.1f;

                levels.Add( l );

                l = new TreeStructureLevel();
                l.LengthBranchMax = 1.5f;
                l.LengthBranchMin = 0.5f;
                l.DiameterBranchMax = 0.02f;
                l.DiameterBranchMin = 0.05f;
                l.NumBranchMax = 7;
                l.NumBranchMin = 3;
                l.NumSegments = 10;
                l.TaperMax = 0.5f;
                l.TaperMin = 0.2f;
                l.DropAngleBranchesMin = MathHelper.PiOver4;
                l.DropAngleBranchMax = MathHelper.PiOver4;
                l.AxialSplitBranchMax = MathHelper.TwoPi;
                l.AxialSplitBranchMin = 0;
                l.BranchStartRatio = 0.25f;
                l.WobbleDropAngleBranchMax = -0.1f;
                l.WobbleDropAngleBranchMin = 0.1f;

                levels.Add( l );


            }
            else
            {

            }



        }
        public Tree CreateTree( int seed, Vector3 position, int numVertices )
        {
            Tree tree = new Tree( position, numVertices );
            Seeder seeder = new Seeder( seed );



            GenerateBranch( tree.Trunk, 0, seeder );



            return tree;
        }

        public void GenerateBranch( Branch b, int ilevel, Seeder seeder )
        {

            b.seed = seeder.NextInt( 0, 1000000 );
            TreeStructureLevel l = levels[ ilevel ];



            b.maxDiameter = seeder.NextFloat( l.DiameterBranchMin, l.DiameterBranchMax );
            b.minDiameter = b.maxDiameter * seeder.NextFloat( l.TaperMin, l.TaperMax );
            b.NumSegments = l.NumSegments;
            b.AxialSplit = seeder.NextFloat( l.AxialSplitBranchMin, l.AxialSplitBranchMax );
            b.DropAngle = seeder.NextFloat( l.DropAngleBranchesMin, l.DropAngleBranchMax );
            b.WobbleAxialSplitBranchMin = l.WobbleAxialSplitBranchMin;
            b.WobbleAxialSplitBranchMax = l.WobbleAxialSplitBranchMax;
            b.WobbleDropAngleBranchMin = l.WobbleDropAngleBranchMin;
            b.WobbleDropAngleBranchMax = l.WobbleDropAngleBranchMax;

            if ( ilevel == 0 )
            {
                b.length = seeder.NextFloat( l.LengthBranchMin, l.LengthBranchMax );
            }
            else
            {
                //float var = ( b.ParentBranch.NumSegments - b.ParentSegmentIndex ) * ( l.BranchLengthDecrase / ( b.ParentBranch.NumSegments - b.ParentSegmentIndex ) ); //((levels[ilevel - 1].BranchStartRatio *b.ParentBranch.length) / (b.ParentBranch.length / levels[ilevel - 1].NumSegments)))) * b.ParentBranch.ParentSegmentIndex;
                float var = (float)b.ParentSegmentIndex / (float)b.ParentBranch.NumSegments;
                b.length = seeder.NextFloat( l.LengthBranchMin, l.LengthBranchMax );
                b.length -= b.length * var * levels[ ilevel - 1 ].BranchLengthDecrase;
            }

            if ( ilevel >= levels.Count - 1 ) return;
            int numChildren = seeder.NextInt( l.NumBranchMin, l.NumBranchMax );
            for ( int i = 0; i < numChildren; i++ )
            {
                //Branch child = b.CreateChildBranch(seeder.NextInt(1, l.NumSegments));
                Branch child = b.CreateChildBranch( seeder.NextInt( (int)( ( l.BranchStartRatio * b.length ) / ( b.length / l.NumSegments ) ), (int)( ( l.BranchEndRatio * b.length ) / ( b.length / l.NumSegments ) ) ) );


                GenerateBranch( child, ilevel + 1, seeder );




            }
        }

        public static void TestGenerator()
        {
            Tree tree = null;
            XNAGame game = null;
            TreestructureGenerator t = null;

            TestXNAGame.Start( "TestGenerator",
                delegate
                {
                    game = TestXNAGame.Instance;

                    t = new TreestructureGenerator( "hjk" );
                    t.LoadData();
                    tree = t.CreateTree( 4582, new Vector3( 2.0f, 2.0f, 2.0f ), 7 );


                    tree.Trunk.CreateSegments();
                    Tree.CreateVertices( tree.Trunk );
                    tree.CreateMesh( game );
                },
                delegate // 3d render code
                {
                    if ( game.Keyboard.IsKeyDown( Microsoft.Xna.Framework.Input.Keys.G ) )
                    {
                        Random r = new Random();
                        tree = t.CreateTree( r.Next( 0, 10000 ), new Vector3( 2.0f, 2.0f, 2.0f ), 11 );
                        tree.Trunk.CreateSegments();
                        Tree.CreateVertices( tree.Trunk );
                        tree.CreateMesh( game );

                    }
                    //Tree.CreateVertices( tree.Trunk );


                    //Tree.DrawBranch(game, tree.Trunk);
                    //Tree.DrawVertices( game, tree.Trunk );
                    //Tree.DrawSegments( game, tree.Trunk );
                    tree.Render( game );
                } );
        }

       

    }
}
