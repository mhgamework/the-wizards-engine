using System;
using System.Collections.Generic;
using System.Text;
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

namespace TreeGenerator.help
{
    public class Tree
    {
        public List<TreeStructureLevel> levels;
        private Branch trunk;

        //private double angle; trunk should have this
        //public double Angle
        //{
        //get{return angle;}
        //}

        public Branch Trunk
        {
            get { return trunk; }
            //set { trunk = value; }
        }


        /*List<Branch> branches;
        public List<Branch> Branches
        {

            //check for child branches if their are, add them to the list branches
            get { return branches; }
        }*/

        public int NumVertices;
        public Vector3 Position;

        public VertexBuffer vertexBuffer;
        public VertexDeclaration decl;
        public int vertexCount;
        public int triangleCount;
        public int vertexStride;
        public BasicShader shader;




        public Tree(Vector3 _position, int _numVertices)
        {
            Position = _position;
            NumVertices = _numVertices;

            //branches = new List<Branch>();

            trunk = Branch.CreateTrunk(this);
        }

        public static void CreateVertices(Branch branch)
        {
            branch.CreateVertices();
            for (int i = 0; i < branch.ChildBranches.Count; i++)
            {
                Tree.CreateVertices(branch.ChildBranches[i]);
            }
        }


        public static void DrawBranch(IXNAGame game, Branch branch)
        {
            for (int i = 1; i < branch.NumSegments; i++)
            {
                int seed = i * 1000 + branch.ParentSegmentIndex * 10;
                game.LineManager3D.AddLine(branch.Segments[i - 1].Position, branch.Segments[i].Position, Game1.CreateRandomColor(seed));

            }
            for (int i = 0; i < branch.ChildBranches.Count; i++)
            {
                DrawBranch(game, branch.ChildBranches[i]);
            }

        }


        public static void DrawSegments( XNAGame game, Branch branch )
        {
            for ( int i = 0; i < branch.Segments.Count; i++ )
            {
                for (int j = 2; j < branch.Segments[ i ].Vertices.Count; j++)
                {
                    Vector3 v1 = branch.Segments[ i ].Vertices[ j ].pos;
                    Vector3 v2 = branch.Segments[ i ].Vertices[ j - 1 ].pos;
                    game.LineManager3D.AddLine( v1, v2, Color.Green );

                    v1 = branch.Segments[ i ].Position;
                    v2 = v1 + branch.Segments[ i ].directions.Right * 3 * branch.Segments[i].Diameter;
                    game.LineManager3D.AddLine( v1, v2, Color.Red );


                }
            }
            for ( int i = 0; i < branch.ChildBranches.Count; i++ )
            {
                DrawSegments( game, branch.ChildBranches[ i ] );
            }

        }

        public static void DrawVertices(XNAGame game, Branch branch)
        {
            //for (int i = 1; i < branch.Segments.Count; i++)
            //{
            //    for (int j = 0; j < branch.Segments[1].Vertices.Count; j++)
            //    {
            //        Vector3 v1 = branch.Segments[i - 1].Vertices[j].pos;
            //        Vector3 v2 = branch.Segments[i].Vertices[j].pos;
            //        game.LineManager3D.AddLine(v1, v2, Game1.CreateRandomColor(i + 1000 * j));

            //    }
            //}
            for (int i = 0; i < branch.Segments.Count; i++)
            {
                for (int j = 2; j < branch.Segments[1].Vertices.Count; j++)
                {
                    Vector3 v1 = branch.Segments[i].Vertices[j - 1].pos;
                    Vector3 v2 = branch.Segments[i].Vertices[j].pos;
                    game.LineManager3D.AddLine(v1, v2, Game1.CreateRandomColor(i));

                }
            }
            for (int i = 0; i < branch.Segments.Count; i++)
            {
                for (int j = 2; j < branch.Segments[i].Vertices.Count; j++)
                {
                    Vector3 v1 = branch.Segments[i].Vertices[j].pos;
                    Vector3 v2 = v1 + branch.Segments[i].Vertices[j].tangent;
                    game.LineManager3D.AddLine(v1, v2, Color.Green);

                }
            }
            for (int i = 0; i < branch.Segments.Count; i++)
            {
                for (int j = 2; j < branch.Segments[i].Vertices.Count; j++)
                {
                    Vector3 v1 = branch.Segments[i].Vertices[j].pos;
                    Vector3 v2 = v1 + branch.Segments[i].Vertices[j].normal;
                    game.LineManager3D.AddLine(v1, v2, Color.Red);

                }
            }
            for (int i = 0; i < branch.ChildBranches.Count; i++)
            {
                DrawVertices(game, branch.ChildBranches[i]);
            }

        }


        public void CreateMesh(IXNAGame game)
        {
            List<TangentVertex> vertices;
            vertices = CreateTriangles(trunk);

            decl = TangentVertexExtensions.CreateVertexDeclaration(game);
            vertexStride = TangentVertex.SizeInBytes;
            vertexCount = vertices.Count;
            triangleCount = vertexCount / 3;

            vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(TangentVertex), vertexCount, BufferUsage.None);
            vertexBuffer.SetData(vertices.ToArray());




            //
            // Load the shader and set the material properties
            //

            shader = BasicShader.LoadFromFXFile(game, new GameFile(game.EngineFiles.RootDirectory + @"Engine\ColladaModel.fx"));

            //shader.SetTechnique( "SpecularPerPixelNormalMapping" );
            shader.SetTechnique("SpecularPerPixel");
            //shader.SetTechnique("SpecularPerPixelColored");


            //TODO: world matrix not correctly implemented
            //TODO: lightdir

            shader.SetParameter("lightDir", Vector3.Normalize(new Vector3(0.6f, 1f, 0.6f)));
            //lightDir.SetValue( -engine.ActiveCamera.CameraDirection );
            //lightDir.SetValue( BasenGame.LightDirection );
            //ColladaMaterial mat = meshPart.Material;

            // Set all material properties

            shader.SetParameter("ambientColor", new Vector4(1f, 1f, 1f, 1f));
            //AmbientColor = setMat.Ambient;
            shader.SetParameter( "diffuseColor", new Vector4( 1f, 1f, 1f, 1f ) );
            shader.SetParameter( "specularColor", new Vector4( 0.1f, 0.1f, 0.1f, 0.1f ) );

            //ret.shader.SetParameter( "shininess", 80f );
            //ret.shader.SetParameter( "shininess", mat.Shininess );


            /*TWTexture texture;
            if (DiffuseTexture != null)
            {
                shader.SetTechnique("SpecularPerPixel");
                texture = TWTexture.FromImageFile(game, new GameFile(DiffuseTexture));
            */
            TWTexture texture = TWTexture.FromImageFile(game, new GameFile(game.EngineFiles.RootDirectory + @"Textures\treebark002.bmp"));
            shader.SetParameter("diffuseTexture", texture);
            shader.SetParameter("diffuseTextureRepeatU", 1.0f);
            shader.SetParameter("diffuseTextureRepeatV", 1.0f);
            //}
            //if (NormalTexture != null)
            //{
            /*shader.SetTechnique("SpecularPerPixelNormalMapping");
            TWTexture texturebump = TWTexture.FromImageFile(game, new GameFile(game.EngineFiles.RootDirectory + @"Textures\birchtree_bump.JPG"));

            shader.SetParameter("normalTexture", texturebump);
            shader.SetParameter("normalTextureRepeatU", 1.0f);
            shader.SetParameter("normalTextureRepeatV", 1.0f);*/

            //}
            //NormalTexture = setMat.normalTexture;

        }

        public void Render(IXNAGame game)
        {
            //game.GraphicsDevice.RenderState.CullMode = CullMode.None;
            shader.World = Matrix.CreateScale(10);
            shader.ViewProjection = game.Camera.ViewProjection;
            shader.RenderMultipass(RenderPrimitives);
        }

        public void RenderPrimitives()
        {
            GraphicsDevice device = vertexBuffer.GraphicsDevice;
            device.Vertices[0].SetSource(vertexBuffer, 0, vertexStride);
            device.VertexDeclaration = decl;
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, triangleCount);
            
        }

        public List<TangentVertex> CreateTriangles(Branch branch)
        {
            List<TangentVertex> vertices = new List<TangentVertex>();
            createTriangles(branch, vertices);

            return vertices;
        }

        private static void createTriangles(Branch branch, List<TangentVertex> vertices)
        {
            TangentVertex v1, v2, v3, v4;
            for (int i = 1; i < branch.Segments.Count; i++)
            {
                Segment s1 = branch.Segments[i - 1];
                Segment s2 = branch.Segments[i];
                for (int j = 1; j < s1.Vertices.Count; j++)
                {
                    // V1--V2
                    // |  / |
                    // | /  |
                    // V3--V4
                    v1 = s2.vertices[j];
                    v3 = s1.vertices[j];
                    if (j == s1.vertices.Count - 1)
                    {
                        v2 = s2.vertices[1];
                        v4 = s1.vertices[1];
                    }
                    else
                    {
                        v2 = s2.vertices[j + 1];
                        v4 = s1.vertices[j + 1];
                    }

                    //vertices.Add(v2);
                    //vertices.Add(v1);
                    //vertices.Add(v3);

                    //vertices.Add(v2);
                    //vertices.Add(v3);
                    //vertices.Add(v4);

                    // Why O Why do we have to put these clockwise.

                    vertices.Add(v3);
                    vertices.Add(v1);
                    vertices.Add(v2);

                    vertices.Add(v3);
                    vertices.Add(v2);
                    vertices.Add(v4);



                }
            }

            for (int i = 0; i < branch.ChildBranches.Count; i++)
            {
                createTriangles(branch.ChildBranches[i], vertices);

            }
        }

        public static void TestTreeStructure()
        {
            Tree tree = null;
            XNAGame game = null;


            TestXNAGame.Start("TestTreeStructure",
                delegate
                {
                    game = TestXNAGame.Instance;
                    tree = new Tree(new Vector3(2.0f, 2.0f, 2.0f), 11);
                    tree.Trunk.length = 3;
                    tree.Trunk.maxDiameter = 0.3f;
                    tree.Trunk.minDiameter = 0.2f;
                    tree.Trunk.NumSegments = 3;
                    tree.trunk.DropAngle = 0;
                    tree.trunk.AxialSplit = 0;
                    Branch b = tree.Trunk.CreateChildBranch(2);
                    b.minDiameter = 0.1f;
                    b.maxDiameter = 0.2f;
                    b.NumSegments = 5;
                    b.length = 1;
                    b.DropAngle = MathHelper.PiOver4;
                    b.AxialSplit = MathHelper.Pi;

                    b.WobbleDropAngleBranchMin = MathHelper.PiOver4;
                    b.WobbleDropAngleBranchMax = MathHelper.PiOver4;

                    tree.Trunk.CreateSegments();
                    Tree.CreateVertices(tree.trunk);

                },
                delegate // 3d render code
                {

                    DrawVertices(game, tree.trunk);
                    //DrawBranch(game, tree.trunk);


                });
        }

        public static void TestRenderTree()
        {
            Tree tree = null;
            XNAGame game = null;


            TestXNAGame.Start("TestTreeStructure",
                delegate
                {
                    game = TestXNAGame.Instance;
                    tree = new Tree(new Vector3(2.0f, 2.0f, 2.0f), 11);
                    tree.Trunk.length = 3;
                    tree.Trunk.maxDiameter = 0.3f;
                    tree.Trunk.minDiameter = 0.2f;
                    tree.Trunk.NumSegments = 3;
                    tree.trunk.DropAngle = 0;
                    tree.trunk.AxialSplit = 0;
                    Branch b = tree.Trunk.CreateChildBranch(2);
                    b.minDiameter = 0.1f;
                    b.maxDiameter = 0.2f;
                    b.NumSegments = 5;
                    b.length = 1;
                    b.DropAngle = MathHelper.PiOver4;
                    b.AxialSplit = MathHelper.Pi;

                    b.WobbleDropAngleBranchMin = MathHelper.PiOver4;
                    b.WobbleDropAngleBranchMax = MathHelper.PiOver4;

                    tree.Trunk.CreateSegments();
                    Tree.CreateVertices(tree.trunk);
                    tree.CreateMesh(game);

                },
                delegate // 3d render code
                {
                    tree.Render(game);
                    DrawVertices(game, tree.trunk);
                    //DrawBranch(game, tree.trunk);


                });
        }
       
    }
}
