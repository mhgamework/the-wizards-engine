using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using Microsoft.Xna;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Graphics;

namespace TreeGenerator.Imposter
{

    public class Imposter
    {
        private Texture2D texture;
        public int sizeX, sizeY;
        private RenderTarget2D renderTarget;
        private Vector3[] impostorVerts = new Vector3[8];
        private Vector3 impostorCenter;
        private Matrix worldViewProj;
        private Vector3 normal;
        public Vector3 CameraDirection;

        public bool needsUpdate = true;
        private bool DrawMesh = false;
        private bool DrawImposter = false;

        public List<TangentVertex> MeshVertices = new List<TangentVertex>();
        public TangentVertex[] PlainVertices = new TangentVertex[6];
        public Vector3 MeshPosition = new Vector3(10, 0, 15);
        public Vector3 BoundingBoxPosition;
        ImposterCamera camera = new ImposterCamera();
        IXNAGame game;
        GraphicsDevice device;
        //just for testing
        //TWModel model;
        private Vector3[] boundingBox = new Vector3[8];
        Matrix WorldMatrix = Matrix.Identity;

        //renderdata
        public VertexBuffer vertexBuffer;
        public VertexDeclaration decl;
        public int vertexCount;
        public int triangleCount;
        public int vertexStride;
        public ColladaShader Shader;


        public float MinDistance = 50;
        public float MaxAngleDifference = 1f;
        public float MaxTime = 10;
        private float time = 0;
        public delegate void RenderDelegate(Matrix viewProjection);
        RenderDelegate renderObject;

        Vector3 meshCenter;
        public void Intialize(int width, int height, IXNAGame _game, RenderDelegate _renderObject, Vector3 meshPosition, Vector3[] _boundingBox)
        {
            sizeX = width;
            sizeY = height;
            game = _game;
            device = game.GraphicsDevice;
            MeshPosition = meshPosition;
            boundingBox = _boundingBox;
            renderObject = _renderObject;
            BoundingBoxPosition = new Vector3((boundingBox[1].X + boundingBox[0].X) * 0.5f, boundingBox[0].Y, (boundingBox[2].Z + boundingBox[0].Z) * 0.5f);
            MinDistance = MinDistance * MinDistance;
        }
        public void Intialize(int width, int height, IXNAGame _game)
        {
            sizeX = width;
            sizeY = height;
            game = _game;
            throw new NotImplementedException();
           // renderObject = TestGrassMesh;
            //boundingBox = CreateBoundingBox(model);
            device = game.GraphicsDevice;
            BoundingBoxPosition = new Vector3((boundingBox[1].X + boundingBox[0].X) * 0.5f, boundingBox[0].Y, (boundingBox[2].Z + boundingBox[0].Z) * 0.5f);
            MinDistance = MinDistance * MinDistance;
        }

        public void UpdateImposter()
        {
            ICamera beginCamera = game.Camera;

            //for testing
            //model = TWModel.FromColladaModel(game, ColladaModel.LoadWall001());
            //model.WorldMatrix *= Matrix.CreateScale(0.05f);
            //model.WorldMatrix *= Matrix.CreateRotationX(-MathHelper.PiOver2);
            //model.WorldMatrix *= Matrix.CreateTranslation(BoundingBoxPosition);

            //WorldMatrix = model.WorldMatrix;
            Vector3 cameraPosition = game.Camera.ViewInverse.Translation;
            camera = new ImposterCamera();

            //----------------------------------------------------------------------
            // Step 1. Get bounding volume corners and set the camera to look at the
            //         diffuse mesh


            // Get the 8 corners of the bounding box and transform by the world matrix 
            Vector3[] corners = new Vector3[8];
            //boundingBox = CreateBoundingBox(model);
            //I     think this unnecesarry for now because I already moved them
            //Vector3.Transform(boundingBox, ref WorldMatrix, corners);
            corners = boundingBox;
            // Get the transformed center of the mesh 
            // alternatively, we could use mBoundingSphere.Center as sometimes it works better
            meshCenter = ReturnCenter(corners);

            // Set the camera to be at the center of the reflector and to look at the diffuse mesh

            camera.LookAt(cameraPosition, meshCenter);
            CameraDirection = cameraPosition - BoundingBoxPosition;
            //CameraDirection = cameraPosition - meshCenter;
            normal = CameraDirection;
            // just for testing
            //normal = BoundingBoxPosition - game.Camera.ViewProjection.Translation;

            camera.BuildView();


            //----------------------------------------------------------------------
            // Step 2. Project the corners of the bounding volume and construct a 
            //         screen space quad that fits the boundaries of the mesh


            // Now we project the vertices to screen space, so we can find the AABB of the screen
            // space vertices
            Vector3[] screenVerts = new Vector3[8];
            for (int i = 0; i < 8; i++)
            {
                screenVerts[i] = device.Viewport.Project(corners[i], camera.Projection, camera.View, WorldMatrix);

            }

            // compute the screen space AABB
            Vector3 min, max;
            ComputeBoundingBoxFromPoints(screenVerts, out min, out max);

            // construct the quad that will represent our diffuse mesh
            Vector3[] screenQuadVerts = new Vector3[4];
            screenQuadVerts[0] = new Vector3(min.X, min.Y, min.Z);
            screenQuadVerts[1] = new Vector3(max.X, min.Y, min.Z);
            screenQuadVerts[2] = new Vector3(max.X, max.Y, min.Z);
            screenQuadVerts[3] = new Vector3(min.X, max.Y, min.Z);


            //----------------------------------------------------------------------
            // Step 3. Unproject the screen quad vertices to form a 3D quad that 
            //         represents our impostor. We will use this to both draw the 
            //         impostor quad and for intersecting the impostor for reflection.

            //now unproject the screen space quad and save the
            //vertices for when we render the impostor quad
            for (int i = 0; i < 4; i++)
            {
                impostorVerts[i] = device.Viewport.Unproject(screenQuadVerts[i], camera.Projection, camera.View, WorldMatrix);
            }

            //compute the center of the quad
            impostorCenter = Vector3.Zero;
            impostorCenter = impostorVerts[0] + impostorVerts[1] + impostorVerts[2] + impostorVerts[3];
            impostorCenter *= .25f;

            // calculate the width and height of the imposter's vertices
            float width = (impostorVerts[1] - impostorVerts[0]).Length() * 1.2f;
            float height = (impostorVerts[3] - impostorVerts[0]).Length() * 1.2f;

            // We construct an Orthographic projection to get rid of the projection distortion
            // which we don't want for our impostor texture
            camera.Projection = Matrix.CreateOrthographic(width, height, .1f, 100);// I can't set the Projection so if this doesn't work i will try there camera
            camera.BuildView();

            //save the WorldViewProjection matrix so we can use it in the shader
            worldViewProj = camera.ViewProj;

            renderTarget = new RenderTarget2D(device, sizeX, sizeY, 1, SurfaceFormat.Color, RenderTargetUsage.DiscardContents);
            device.SetRenderTarget(0, renderTarget);
            //render

            //TestGrassMesh(camera);
            //model.Render();
            renderObject(camera.ViewProj);


            device.SetRenderTarget(0, null);
            texture = renderTarget.GetTexture();

            //texture = Texture2D.FromFile(device, game.RootDirectory + "Grass//Textures//grass.tga"); the problem is at the texture it self
            //texture.Save(game.EngineFiles.RootDirectory + "texturetest"+MeshPosition.Y.ToString() + game.Elapsed.ToString()+  ".tga", ImageFileFormat.Tga);
            #region setup to render plain
            CreatePlain();
            //set up every thing for rendering the plain
            decl = TangentVertexExtensions.CreateVertexDeclaration(game);
            vertexStride = TangentVertex.SizeInBytes;
            vertexCount = PlainVertices.Length;
            triangleCount = vertexCount / 3;

            vertexBuffer = new VertexBuffer(device, typeof(TangentVertex), vertexCount, BufferUsage.None);
            vertexBuffer.SetData(PlainVertices);

            Shader = new ColladaShader(game, new Microsoft.Xna.Framework.Graphics.EffectPool());

            Shader.Technique = ColladaShader.TechniqueType.Textured;

            Shader.ViewInverse = Matrix.Identity;
            Shader.ViewProjection = Matrix.Identity;
            Shader.LightDirection = Vector3.Normalize(new Vector3(0.6f, 1f, 0.6f));
            Shader.LightColor = new Vector3(1, 1, 1);

            Shader.AmbientColor = new Vector4(1f, 1f, 1f, 1f);
            Shader.DiffuseColor = new Vector4(1f, 1f, 1f, 1f);
            Shader.SpecularColor = new Vector4(0.1f, 0.1f, 0.1f, 0.1f);


            Shader.DiffuseTexture = texture;
            Shader.Technique = ColladaShader.TechniqueType.Textured;
            #endregion

            needsUpdate = false;
        }

        public void Update()
        {
            float distance;
            distance = Vector3.DistanceSquared(BoundingBoxPosition, game.Camera.ViewProjection.Translation);//can make this faster by amking it squared
            float angleDifference = AngleBetweenTwoV3(normal, BoundingBoxPosition - game.Camera.ViewInverse.Translation);//AngleBetweenTwoV3(normal, BoundingBoxPosition - game.Camera.ViewProjection.Translation);
            //if (game.Elapsed!=null)
            //{
            //    time += game.Elapsed * 0.001f;//GameTime.ElapsedGameTime.Milliseconds*0.001f;
            //}

            if (distance > MinDistance)
            {
                DrawImposter = true;
                DrawMesh = false;
            }
            else
            {
                DrawImposter = false;
                DrawMesh = true;
            }
            if (MinDistance + 10 > distance && distance > MinDistance - 10)
            {
                DrawImposter = true;
                DrawMesh = true;
            }
            //float MaxAngleDiffer = MaxAngleDifference + (distance / 500);
            if (DrawImposter)
            {
                if (angleDifference > MaxAngleDifference)//needs to add the distance in the equation: the further away the bigger the angle differce can be
                {
                    needsUpdate = true;
                }
                //if (time>MaxTime)
                //{
                //    needsUpdate=true;
                //    time=0;
                //}

            }
        }
        float updateImposterCount = 0;
        public void Render()
        {
            //should perhaps come somewhere else
            if (needsUpdate)
            {

                UpdateImposter();
            }

            //device.Clear(Color.CornflowerBlue);
            if (DrawImposter)
            {
                device.RenderState.CullMode = CullMode.None;
                device.RenderState.AlphaTestEnable = true;
                device.RenderState.ReferenceAlpha = 1;
                device.RenderState.AlphaFunction = CompareFunction.GreaterEqual;
                Shader.ViewProjection = game.Camera.ViewProjection;
                Shader.ViewInverse = game.Camera.ViewInverse;
                Shader.World = Matrix.Identity;
                Shader.Shader.RenderMultipass(RenderPrimitives);
            }
            if (DrawMesh)
            {
                device.RenderState.CullMode = CullMode.None;
                device.RenderState.AlphaTestEnable = true;
                device.RenderState.ReferenceAlpha = 1;
                device.RenderState.AlphaFunction = CompareFunction.GreaterEqual;
                device.RenderState.CullMode = CullMode.None;
                renderObject(game.Camera.ViewProjection);
            }

            for (int i = 0; i < boundingBox.Length - 1; i++)
            {
                game.LineManager3D.AddLine(boundingBox[i], boundingBox[i + 1], Color.Yellow);
            }
            for (int i = 0; i < MeshVertices.Count - 1; i++)
            {
                game.LineManager3D.AddLine(MeshVertices[i].pos, MeshVertices[i + 1].pos, Color.Red);
            }
            for (int i = 0; i < PlainVertices.Length - 1; i++)
            {
                game.LineManager3D.AddLine(PlainVertices[i].pos, PlainVertices[i + 1].pos, Color.Pink);
            }
            game.LineManager3D.AddCenteredBox(BoundingBoxPosition, 0.25f, Color.Red);
        }

        private void RenderPrimitives()
        {
            GraphicsDevice device = vertexBuffer.GraphicsDevice;
            device.Vertices[0].SetSource(vertexBuffer, 0, vertexStride);
            device.VertexDeclaration = decl;
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, triangleCount);

        }

        //public Vector3[] CreateBoundingBox(TWModel model)
        //{
        //    Vector3[] boundingbox = new Vector3[8];
        //    //for (int i = 0; i < model.Parts.Count; i++)
        //    //{

        //    //}
        //    //lets say i can get this
        //    //wall
        //    /*boundingbox[3] = new Vector3(8, 0, 19);
        //    boundingbox[2] = new Vector3(11, 0, 19);
        //    boundingbox[4] = new Vector3(8, 5, 19);
        //    boundingbox[5] = new Vector3(11, 5, 19);
        //    boundingbox[0] = new Vector3(8, 0, 11);
        //    boundingbox[1] = new Vector3(11, 0, 11);
        //    boundingbox[7] = new Vector3(8, 5, 11);
        //    boundingbox[6] = new Vector3(11, 5, 11);*/

        //    //grass
        //    boundingbox[0] = new Vector3(9, 0, 14);
        //    boundingbox[1] = new Vector3(11, 0, 14);
        //    boundingbox[2] = new Vector3(11, 0, 16);
        //    boundingbox[3] = new Vector3(9, 0, 16);
        //    boundingbox[4] = new Vector3(9, 2, 16);
        //    boundingbox[5] = new Vector3(11, 2, 16);
        //    boundingbox[6] = new Vector3(11, 2, 14);
        //    boundingbox[7] = new Vector3(9, 2, 14);



        //    return boundingbox;
        //}

        protected void ComputeBoundingBoxFromPoints(Vector3[] vertices, out Vector3 min, out Vector3 max)
        {
            min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < vertices.Length; i++)
            {
                min = Vector3.Min(min, vertices[i]);
                max = Vector3.Max(max, vertices[i]);
            }
        }

        public Vector3 ReturnCenter(Vector3[] vertices)
        {
            Vector3 center;
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < vertices.Length; i++)
            {
                min = Vector3.Min(min, vertices[i]);
                max = Vector3.Max(max, vertices[i]);
            }

            center = (min + max) * 0.5f;
            return center;
        }

        public float AngleBetweenTwoV3(Vector3 v1, Vector3 v2)
        {
            v1.Normalize();
            v2.Normalize();
            double Angle = (float)Math.Acos(Vector3.Dot(v1, v2));
            return (float)Angle;
        }

        public void CreatePlain()
        {
            Vector3 tangent = Vector3.Up;

            //triangle 1
            PlainVertices[0] = new TangentVertex(impostorVerts[0], 0, 0, normal, tangent);
            PlainVertices[1] = new TangentVertex(impostorVerts[3], 0, 1, normal, tangent);
            PlainVertices[2] = new TangentVertex(impostorVerts[2], 1, 1, normal, tangent);
            //triangle 2
            PlainVertices[3] = new TangentVertex(impostorVerts[0], 0, 0, normal, tangent);
            PlainVertices[4] = new TangentVertex(impostorVerts[1], 1, 0, normal, tangent);
            PlainVertices[5] = new TangentVertex(impostorVerts[2], 1, 1, normal, tangent);
        }


        

        public static void TestImposter()
        {
            XNAGame game;
            game = new XNAGame();
            Imposter imp = new Imposter();

            game.InitializeEvent +=
                delegate
                {
                    imp.Intialize(500, 500, game);
                    imp.Update();


                };
            game.UpdateEvent +=
                delegate
                {
                    if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.U))
                    {
                        imp.needsUpdate = true;
                    }
                    imp.Update();
                };
            game.DrawEvent +=
                delegate
                {
                    for (int i = 0; i < imp.boundingBox.Length - 1; i++)
                    {
                        game.LineManager3D.AddLine(imp.boundingBox[i], imp.boundingBox[i + 1], Color.Yellow);
                    }
                    for (int i = 0; i < imp.PlainVertices.Length; i += 3)
                    {
                        game.LineManager3D.AddTriangle(imp.PlainVertices[i].pos, imp.PlainVertices[i + 1].pos, imp.PlainVertices[i + 2].pos, Color.DarkBlue);
                    }

                    //imp.TestGrassMesh();
                    imp.Render();
                    // imp.UpdateImposter(game);
                };
            game.Run();
        }

    }
     
}
