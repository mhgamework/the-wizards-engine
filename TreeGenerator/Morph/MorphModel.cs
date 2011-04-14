using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Entity.Editor;
using MHGameWork.TheWizards.Rendering;

namespace TreeGenerator.Morph
{
    public class MorphModel
    {
        private VertexBuffer vb;
        private VertexBuffer vb2;
        private VertexDeclaration decl;
        private int vertexCount;
        private int vertexStride;
        private BasicShader shader;
        private int triangleCount;
        private GraphicsDevice device;
        private XNAGame game;


        private Vector3[] position;
        public Vector3[] GetPosition()
        {
            return position;
        }
        private Vector3[] position2;
        public Vector3[] GetPosition2()
        {
            return position2;
        }
        private Vector3[] normal;
        private Vector3[] normal2;

        private Vector2[] UV;
        private Vector2[] UV2;

        private List<MorphVertex> Vertices = new List<MorphVertex>();
        private List<MorphVertex> Vertices2 = new List<MorphVertex>();
       public EditorMesh MorphMesh= new EditorMesh();

        VertexElement[] elements = new VertexElement[]  
            { 
                new VertexElement( 0, 0, VertexElementFormat.Vector3, 
                    VertexElementMethod.Default, VertexElementUsage.Position, 0), 
             
                new VertexElement( 0, 12, VertexElementFormat.Vector3, 
                    VertexElementMethod.Default, VertexElementUsage.Normal, 0), 
                new VertexElement(0,24,VertexElementFormat.Vector2,VertexElementMethod.Default,VertexElementUsage.TextureCoordinate,0),
             
                new VertexElement( 1, 0, VertexElementFormat.Vector3, 
                    VertexElementMethod.Default, VertexElementUsage.Position, 1),    
              
                new VertexElement( 1,12, VertexElementFormat.Vector3, 
                    VertexElementMethod.Default, VertexElementUsage.Normal, 1), 
                new VertexElement(1,24,VertexElementFormat.Vector2,VertexElementMethod.Default,VertexElementUsage.TextureCoordinate,1),

            };

        public struct MorphVertex
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 TexUV;


            public MorphVertex(Vector3 position, Vector2 texUV, Vector3 normal)
            {
                Position = position;
               
                TexUV = texUV;
                Normal = normal;
               
            }

            public static readonly VertexElement[] VertexElements =
             {
                 new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position,0),
                 new VertexElement(0, 4*(3), VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0),
                 new VertexElement(0, 4*(3+3), VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
             };
            public static int SizeInBytes = sizeof(float) * (3 + 2 + 3);

        }


        public void Initialize(XNAGame _game)
        {
            game = _game;
            MorphMesh = new EditorMesh();
            ObjImporter objImporter = new ObjImporter();

            MorphMesh = objImporter.ImportObjFile(@"C:\Users\Bart\Desktop\OBJ Ketch_1.obj");

            device = game.GraphicsDevice;
            decl = new VertexDeclaration(device,elements);
            vertexStride = MorphVertex.SizeInBytes;
            position=((EditorMeshPart) MorphMesh.CoreData.Parts[0].MeshPart).GeometryData.GetSourceVector3(MeshPartGeometryData.Semantic.Position);
            position2 = ((EditorMeshPart)MorphMesh.CoreData.Parts[1].MeshPart).GeometryData.GetSourceVector3(MeshPartGeometryData.Semantic.Position);
            normal =((EditorMeshPart) MorphMesh.CoreData.Parts[0].MeshPart).GeometryData.GetSourceVector3(MeshPartGeometryData.Semantic.Normal);
            normal2 = ((EditorMeshPart)MorphMesh.CoreData.Parts[1].MeshPart).GeometryData.GetSourceVector3(MeshPartGeometryData.Semantic.Normal);
            UV = ((EditorMeshPart)MorphMesh.CoreData.Parts[0].MeshPart).GeometryData.GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord);
            UV2 = ((EditorMeshPart)MorphMesh.CoreData.Parts[1].MeshPart).GeometryData.GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord);


            for (int i = 0; i < position.Length; i++)
            {
                Vertices.Add(new MorphVertex(position[i], UV[i], normal[i]));
                Vertices2.Add(new MorphVertex(position2[i], UV2[i], normal2[i]));

            }


            triangleCount = position.Length/3;
            
            vb = new VertexBuffer(device, Vertices.Count*MorphVertex.SizeInBytes, BufferUsage.None);
            vb.SetData<MorphVertex>(Vertices.ToArray());
            vb2 = new VertexBuffer(device, Vertices2.Count * MorphVertex.SizeInBytes, BufferUsage.None);
            vb2.SetData<MorphVertex>(Vertices2.ToArray());

            //shader = new BasicShader();
            shader = BasicShader.LoadFromFXFile(game, new GameFile(game.EngineFiles.RootDirectory + @"Morph\MorphShader.fx"));
            Texture2D tex = new Texture2D(game.GraphicsDevice, 1, 1, 0, TextureUsage.None, SurfaceFormat.Rgb32);

            TWTexture texture1 = TWTexture.FromTexture2D(tex);

            shader.SetParameter("diffuseTexture", texture1);
            shader.SetParameter("World", Matrix.Identity);
            shader.SetParameter("Projection", Matrix.Identity);
            shader.SetParameter("LerpValue", 0);
            shader.SetParameter("lightDir", new Vector3(0, 0, 1));





        }
        public void Initialize(XNAGame _game,EditorMesh mesh)
        {
            game = _game;
            MorphMesh = new EditorMesh();
            ObjImporter objImporter = new ObjImporter();

            MorphMesh = mesh;

            device = game.GraphicsDevice;
            decl = new VertexDeclaration(device, elements);
            vertexStride = MorphVertex.SizeInBytes;
            position = ((EditorMeshPart)MorphMesh.CoreData.Parts[0].MeshPart).GeometryData.GetSourceVector3(MeshPartGeometryData.Semantic.Position);
            position2 = ((EditorMeshPart)MorphMesh.CoreData.Parts[1].MeshPart).GeometryData.GetSourceVector3(MeshPartGeometryData.Semantic.Position);
            normal = ((EditorMeshPart)MorphMesh.CoreData.Parts[0].MeshPart).GeometryData.GetSourceVector3(MeshPartGeometryData.Semantic.Normal);
            normal2 = ((EditorMeshPart)MorphMesh.CoreData.Parts[1].MeshPart).GeometryData.GetSourceVector3(MeshPartGeometryData.Semantic.Normal);
            UV = ((EditorMeshPart)MorphMesh.CoreData.Parts[0].MeshPart).GeometryData.GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord);
            UV2 = ((EditorMeshPart)MorphMesh.CoreData.Parts[0].MeshPart).GeometryData.GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord);


            for (int i = 0; i < position.Length; i++)
            {
                Vertices.Add(new MorphVertex(position[i], UV[i], normal[i]));
                Vertices2.Add(new MorphVertex(position2[i], UV2[i], normal2[i]));

            }


            triangleCount = position.Length / 3;

            vb = new VertexBuffer(device, Vertices.Count * MorphVertex.SizeInBytes, BufferUsage.None);
            vb.SetData<MorphVertex>(Vertices.ToArray());
            vb2 = new VertexBuffer(device, Vertices2.Count * MorphVertex.SizeInBytes, BufferUsage.None);
            vb2.SetData<MorphVertex>(Vertices2.ToArray());

            //shader = new BasicShader();
            shader = BasicShader.LoadFromFXFile(game, new GameFile(game.EngineFiles.RootDirectory + @"Morph\MorphShader.fx"));
            Texture2D tex = new Texture2D(game.GraphicsDevice, 1, 1, 0, TextureUsage.None, SurfaceFormat.Rgb32);
            
            TWTexture texture1 =TWTexture.FromTexture2D(tex);

            shader.SetParameter("diffuseTexture", texture1);
            shader.SetParameter("World", Matrix.Identity);
            shader.SetParameter("Projection", Matrix.Identity);
            shader.SetParameter("LerpValue", 0);
            shader.SetParameter("lightDir", new Vector3(0, 0, 1));





        }
        public void Initialize(XNAGame _game, EditorMesh mesh,string TexturePath)
        {
            game = _game;
            MorphMesh = new EditorMesh();
            ObjImporter objImporter = new ObjImporter();

            MorphMesh = mesh;

            device = game.GraphicsDevice;
            decl = new VertexDeclaration(device, elements);
            vertexStride = MorphVertex.SizeInBytes;
            position = ((EditorMeshPart)MorphMesh.CoreData.Parts[0].MeshPart).GeometryData.GetSourceVector3(MeshPartGeometryData.Semantic.Position);
            position2 = ((EditorMeshPart)MorphMesh.CoreData.Parts[1].MeshPart).GeometryData.GetSourceVector3(MeshPartGeometryData.Semantic.Position);
            normal = ((EditorMeshPart)MorphMesh.CoreData.Parts[0].MeshPart).GeometryData.GetSourceVector3(MeshPartGeometryData.Semantic.Normal);
            normal2 = ((EditorMeshPart)MorphMesh.CoreData.Parts[1].MeshPart).GeometryData.GetSourceVector3(MeshPartGeometryData.Semantic.Normal);
            UV = ((EditorMeshPart)MorphMesh.CoreData.Parts[0].MeshPart).GeometryData.GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord);
            UV2 = ((EditorMeshPart)MorphMesh.CoreData.Parts[0].MeshPart).GeometryData.GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord);


            for (int i = 0; i < position.Length; i++)
            {
                Vertices.Add(new MorphVertex(position[i], UV[i], normal[i]));
                Vertices2.Add(new MorphVertex(position2[i], UV2[i], normal2[i]));

            }


            triangleCount = position.Length / 3;

            vb = new VertexBuffer(device, Vertices.Count * MorphVertex.SizeInBytes, BufferUsage.None);
            vb.SetData<MorphVertex>(Vertices.ToArray());
            vb2 = new VertexBuffer(device, Vertices2.Count * MorphVertex.SizeInBytes, BufferUsage.None);
            vb2.SetData<MorphVertex>(Vertices2.ToArray());

            //shader = new BasicShader();
            shader = BasicShader.LoadFromFXFile(game, new GameFile(game.EngineFiles.RootDirectory + @"Morph\MorphShader.fx"));
            TWTexture texture1 = TWTexture.FromImageFile(game, new GameFile(game.EngineFiles.RootDirectory + TexturePath));
            shader.SetParameter("diffuseTexture", texture1);
            shader.SetParameter("World", Matrix.Identity);
            shader.SetParameter("Projection", Matrix.Identity);
            shader.SetParameter("LerpValue", 0);
            shader.SetParameter("lightDir", new Vector3(0, 0, 1));





        }

        private float time = 0;
public void SetStartLerpValue(float lerpStart)
{
    time = lerpStart;
}
        private int direction = 1;
        public void Update(float speed)
        {
            time += direction*(float)game.Elapsed*speed;
            if (time > 1.0f)
            {
                direction *= -1;
                time = 1.0f;
            }
            if (time < 0f)
            {
                direction *= -1;
                time = 0f;
            }

               
            
            shader.SetParameter("LerpValue", time);
        }
        public void Render()
        {    shader.SetParameter("Projection",game.Camera.Projection);
            shader.SetParameter("View",game.Camera.View);
            shader.SetParameter("ViewInverse", game.Camera.ViewInverse);

            device.RenderState.AlphaTestEnable = true;
            device.RenderState.ReferenceAlpha = 170;
            device.RenderState.AlphaFunction = CompareFunction.GreaterEqual;
            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.DepthBufferWriteEnable = true;
            shader.RenderMultipass(RenderPrimitives);


            device.RenderState.CullMode = CullMode.None;
            device.RenderState.AlphaTestEnable = true;
            device.RenderState.ReferenceAlpha = 170;
            device.RenderState.AlphaFunction = CompareFunction.Less;
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.DepthBufferWriteEnable = false;
            shader.RenderMultipass(RenderPrimitives);

        }
        private void RenderPrimitives()
        {
            device.RenderState.CullMode = CullMode.None;
            device.VertexDeclaration = decl;
            device.Vertices[0].SetSource(vb, 0, vertexStride);
            device.Vertices[1].SetSource(vb2, 0, vertexStride);


            device.DrawPrimitives(PrimitiveType.TriangleList, 0, triangleCount);

        }
        public void SetWorldMatrix(Matrix mat)
        {
            shader.SetParameter("World", mat);
        }

        public static void TestMorphModel()
        {
            XNAGame game;
            game = new XNAGame();
            game.DrawFps = true;
            game.IsFixedTimeStep = false;

            MorphModel morph= new MorphModel();

            game.InitializeEvent +=
                delegate
                    {
                        morph.Initialize(game);
                    };

            game.UpdateEvent+=
                delegate
                    {
                        morph.Update(1);
                    };

            game.DrawEvent+=
                delegate
                    {

                        game.LineManager3D.AddCenteredBox(morph.position[0], 4, Color.Black);
                        game.LineManager3D.AddCenteredBox(morph.position2[0], 4, Color.Black);


                             morph.Render();
                };
            game.Run();
        }

    }
}
