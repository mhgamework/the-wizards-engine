using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TreeGenerator.Clouds
{
    public class BillBoards
    {
        public List<TangentVertex> Vertices = new List<TangentVertex>();
        private VertexDeclaration decl;
        private VertexBuffer vertexBuffer;

        private XNAGame game;
        private BasicShader shader;
        private TWTexture texture;
        public void initialize(XNAGame _game)
        {
            game = _game;
             texture = TWTexture.FromImageFile(game, new GameFile(game.EngineFiles.RootDirectory + @"Clouds\SingleCloud001-5.png"));//, parameters );
             shader = BasicShader.LoadFromFXFile(game, new GameFile(game.EngineFiles.RootDirectory + @"TreeEngine\BillBoardShader.fx"));

             shader.SetTechnique("Billboard");
             shader.SetParameter("world", Matrix.Identity);
             shader.SetParameter("viewProjection", Matrix.Identity);
             shader.SetParameter("view", Matrix.Identity);
             shader.SetParameter("diffuseTexture", texture);


             vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(TangentVertex), Vertices.Count, BufferUsage.None);
             vertexBuffer.SetData(Vertices.ToArray());
             decl = new VertexDeclaration(game.GraphicsDevice, TangentVertex.VertexElements);
        }


        public void render()
        {
            game.GraphicsDevice.RenderState.CullMode = CullMode.None;
            shader.SetParameter("viewProjection", game.Camera.ViewProjection);
            shader.SetParameter("projection", game.Camera.Projection);
            shader.SetParameter("view", game.Camera.View);
            shader.SetParameter("viewInverse", game.Camera.ViewInverse);
            shader.SetParameter("world", Matrix.CreateTranslation(new Vector3(10, 0, 10)));


            shader.RenderMultipass(renderPrimitives);


            //game.LineManager3D.AddLine(Vertices[0].position, game.Camera.View.Right, Color.Green);
            //game.LineManager3D.AddLine(Vertices[0].position, game.Camera.View.Up, Color.Yellow);
            //game.LineManager3D.AddLine(Vertices[0].position, game.Camera.View.Forward, Color.Pink);

            game.LineManager3D.AddLine(Vertices[0].pos, game.Camera.ViewInverse.Right, Color.Green);
            game.LineManager3D.AddLine(Vertices[0].pos, game.Camera.ViewInverse.Up, Color.Yellow);
            game.LineManager3D.AddLine(Vertices[0].pos, game.Camera.ViewInverse.Forward, Color.Pink);
            
            
        }
        private void renderPrimitives()
        {
            //GraphicsDevice device = vertexBuffer.GraphicsDevice;

            game.GraphicsDevice.VertexDeclaration = decl;
            game.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, TangentVertex.SizeInBytes);
            game.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, Vertices.Count);// don't know if this goes faster but just to make sure to not make mistake "TotalVertices"
        }

        public void AddQuad(Vector3 pos)
        {
            Vertices.Add(new TangentVertex(pos, new Vector2(0, 0), new Vector3(-1, 1, 0), new Vector3(2, 2, 0)));
            Vertices.Add(new TangentVertex(pos, new Vector2(0, 1), new Vector3(-1, -1, 0), new Vector3(2,2, 0)));
            Vertices.Add(new TangentVertex(pos, new Vector2(1, 1), new Vector3(1, -1, 0), new Vector3(2, 2, 0)));

            Vertices.Add(new TangentVertex(pos, new Vector2(0, 0), new Vector3(-1, 1, 0), new Vector3(2, 2, 0)));
            Vertices.Add(new TangentVertex(pos, new Vector2(1, 1), new Vector3(1, -1, 0), new Vector3(2, 2, 0)));
            Vertices.Add(new TangentVertex(pos, new Vector2(1, 0), new Vector3(1, 1, 0), new Vector3(2, 2, 0)));

        }
             public struct BillBoardVertex
        {
            public Vector3 position;
            public Vector2 Size;
            public Vector2 QuadPositioning;
            public Vector2 texCoord;

            public BillBoardVertex(Vector3 position,Vector2 Size, Vector2 QuadPositioning,Vector2 texCoord)
            {
                this.position = position;
                this.Size = Size;
                this.QuadPositioning = QuadPositioning;
                this.texCoord = texCoord;

            }

            public static readonly VertexElement[] VertexElements =
     {
         new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
         new VertexElement(0, sizeof(float)*3, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
         new VertexElement(0, sizeof(float)*5, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1),
         new VertexElement(0, sizeof(float)*7, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 2),

     };
            public static int SizeInBytes = sizeof( float ) * ( 3 + 2 +2+2);
        }

             public static void TestCreateVertices()
             {
                 XNAGame game;
                 game = new XNAGame();

                 BillBoards billboards=new BillBoards();

                 game.InitializeEvent +=
                     delegate
                         {
                             billboards.AddQuad(new Vector3(0,0,0));
                         billboards.initialize(game);
                     };
                 game.DrawEvent +=
                     delegate
                         {
                             billboards.render();
                         };
                 game.Run();
             }

    }
}
