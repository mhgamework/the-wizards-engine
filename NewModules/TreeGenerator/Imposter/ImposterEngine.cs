using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.ServerClient.CascadedShadowMaps;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Common.Core.Collada;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Graphics;

namespace TreeGenerator.Imposter
{
    public class ImposterEngine
    {
        public List<TangentVertex> Vertices = new List<TangentVertex>();
        public VertexBuffer vertexBuffer;
        public VertexDeclaration decl;
        public int vertexCount;
        public int triangleCount;
        public int vertexStride;
        public ColladaShader Shader;
        IXNAGame game;
        GraphicsDevice device;

        public List<ImposterStruct> Imposters = new List<ImposterStruct>();
        private DepthStencilBuffer depthBuffer;
        SpriteBatch batch;
        public RenderTarget2D RenderTarget;
        public Texture2D textureOne;
        public TWTexture textureTwo = new TWTexture();
        public int size = 128; //128/2;
        FullScreenQuad quad;

        public int ImpostersTextureSize = 128 * 8;//128 * 4;

        

        public ImposterEngine(IXNAGame _game)
        {
            game = _game;
            device = game.GraphicsDevice;
        }

        public void Initialze()
        {
            device = game.GraphicsDevice;
            textureOne = new Texture2D(device, ImpostersTextureSize, ImpostersTextureSize, 1, TextureUsage.None, SurfaceFormat.Color);
            Shader = new ColladaShader(game, null);
            //RenderTarget = new RenderTarget2D(device, ImpostersTextureSize, ImpostersTextureSize, 1, SurfaceFormat.Color, RenderTargetUsage.DiscardContents);
            batch = new SpriteBatch(device);
            depthBuffer = new DepthStencilBuffer(device, ImpostersTextureSize, ImpostersTextureSize, device.DepthStencilBuffer.Format);
            RenderTarget = new RenderTarget2D(device, ImpostersTextureSize, ImpostersTextureSize, 1, SurfaceFormat.Color, RenderTargetUsage.DiscardContents);

            textureTwo = TWTexture.FromImageFile(game, new GameFile(game.EngineFiles.RootDirectory + @"Textures\rockbump.jpg"));

            Shader = new ColladaShader(game, new Microsoft.Xna.Framework.Graphics.EffectPool());

            Shader.Technique = ColladaShader.TechniqueType.Textured;

            Shader.ViewInverse = Matrix.Identity;
            Shader.ViewProjection = Matrix.Identity;
            Shader.LightDirection = Vector3.Normalize(new Vector3(0.6f, 1f, 0.6f));
            Shader.LightColor = new Vector3(1, 1, 1);

            Shader.AmbientColor = new Vector4(1f, 1f, 1f, 1f);
            Shader.DiffuseColor = new Vector4(1f, 1f, 1f, 1f);
            Shader.SpecularColor = new Vector4(0.1f, 0.1f, 0.1f, 0.1f);


            //Shader.DiffuseTexture = textureOne;
            //Shader.DiffuseTexture = textureTwo.XnaTexture;//this is because I haven't made any smooth transition shader were we use 2 textures
            Shader.Technique = ColladaShader.TechniqueType.Textured;
        }

        
        public void Update()
        {
            if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.U))
            {
                for (int i = 0; i < Imposters.Count; i++)
                {
                    Imposters[i].NeedsUpdate = true;
                }
            }
            if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.I))
            {
                Imposters[0].NeedsUpdate = true;

            }
            if (game.Keyboard.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.O))
            {
                Imposters[1].NeedsUpdate = true;

            }

            /* What must be evaluated in the update methode?
             * the further away the less often it needs to be evaluted
             * 
             * 
             * 
             * 
            */
           
               
                for (int i = 0; i < Imposters.Count; i++)
                {
                    Imposters[i].Distance = Vector3.Distance(Imposters[i].BoundingBoxPosition, game.Camera.ViewInverse.Translation);
                    if (Imposters[i].Distance > Imposters[i].MinDistance)
                    {
                        if (Imposters[i].DrawMesh)
                        {
                            Imposters[i].NeedsUpdate = true;
                        }
                        Imposters[i].DrawMesh = false;
                        Imposters[i].DrawImposter = true;
                    }
                    else
                    {
                        Imposters[i].DrawMesh = true;
                        HideImposterPlain(Imposters[i].Index);
                        Imposters[i].DrawImposter = false;
                    }
                    if (Imposters[i].DrawImposter)
                    {
                        Imposters[i].Time += game.Elapsed;
                        if (Imposters[i].Time > Imposters[i].MaxTime)
                        {
                                Imposters[i].Time = 0;
                            if (CheckAngle(Imposters[i]))
                            {
                                Imposters[i].NeedsUpdate = true;
                            }
                            if (Imposters[i].Distance>200)
                            {
                                Imposters[i].DiscartRenderData = true;
                                Imposters[i].RenderDataGone=true;
                            }
                            if (Imposters[i].RenderDataGone && Imposters[i].Distance < 190)
                            {
                                Imposters[i].LoadRenderData = true;
                            }

                        }
                    }
                

            }
            //for (int i = 0; i < Imposters.Count; i++)
            //{
            //    if (Imposters[i].NeedsUpdate)
            //    {
            //        UpdateImposter(i);
            //        Imposters[i].NeedsUpdate = false;
            //    }
            //}
            NewUpdateImposter();
        }

        public void Render()
        {
            //SetupForRender();
            RenderImposters();

            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.CullMode = CullMode.None;
            device.RenderState.AlphaTestEnable = true;
            device.RenderState.ReferenceAlpha = 1;
            device.RenderState.AlphaFunction = CompareFunction.GreaterEqual;

            Shader.ViewProjection = game.Camera.ViewProjection;
            Shader.ViewInverse = game.Camera.ViewInverse;
            Shader.World = Matrix.Identity;

            Shader.Shader.RenderMultipass(RenderPrimitives);
            /*for (int i = 0; i < Vertices.Count; i += 3)
            {
                game.LineManager3D.AddTriangle(Vertices[i].pos, Vertices[i + 1].pos, Vertices[i + 2].pos, new Color(new Vector3(i, i, i)));
            }*/
            for (int i = 0; i < Imposters.Count; i++)
            {
                if (Imposters[i].DrawMesh)
                {
                    Imposters[i].RenderObject(game.Camera.ViewProjection);
                }
            }

            batch.Begin();
            batch.Draw(textureOne, new Rectangle(10, 10, 128, 128), Color.White);

            batch.End();



        }
        private void RenderPrimitives()
        {
            //GraphicsDevice device = vertexBuffer.GraphicsDevice;
            device.Vertices[0].SetSource(vertexBuffer, 0, vertexStride);
            device.VertexDeclaration = decl;
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, triangleCount);

        }
        private void RenderPrimitiveScreenQuad()
        {
            //GraphicsDevice device = vertexBuffer.GraphicsDevice;
            device.Vertices[0].SetSource(quad.VertexBuffer, 0, vertexStride);
            device.VertexDeclaration = quad.VertexDeclaration;
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, 2);

        }
        private void DrawScreenQuad()
        {
            quad = new FullScreenQuad(device);

            ColladaShader shade = new ColladaShader(game, null);
            shade.DiffuseTexture = textureOne;
            shade.Shader.RenderMultipass(RenderPrimitiveScreenQuad);
            quad.Draw();

        }



        public delegate void RenderDelegate(Matrix viewProjection);

        public void AddRenderObject(RenderDelegate render, Vector3[] boundingBox)
        {
            int xCo = 0, yCo = 0;

            //NOTE: Bart, Dit heb ik ook gefixt

            int numImpostersY = ImpostersTextureSize / size;
            yCo = Imposters.Count/numImpostersY;
            xCo = Imposters.Count - yCo * numImpostersY;
            

            /*int texPerLength = ImpostersTextureSize / size;
            if ((Imposters.Count - (texPerLength * yCo) / texPerLength) < texPerLength)
            {
                xCo = Imposters.Count;


            }
            else { yCo++; }*/

            ImposterStruct imp = new ImposterStruct(xCo, yCo, Imposters.Count, render, boundingBox,size);
            imp.Distance = (Vector3.Distance(imp.BoundingBoxPosition, game.Camera.ViewInverse.Translation));
            Imposters.Add(imp);
            for (int i = 0; i < 6; i++)
            {
                Vertices.Add(new TangentVertex());
            }
            UpdateImposter(Imposters.Count - 1);
        }

        public void UpdateImposter(int index)
        {
            ImposterStruct imp = Imposters[index];
            MHGameWork.TheWizards.ServerClient.ICamera beginCamera = game.Camera;
            Vector3[] impostorVerts = new Vector3[8];

            Vector3 cameraPosition = game.Camera.ViewInverse.Translation;
            ImposterCamera camera = new ImposterCamera();

            //----------------------------------------------------------------------
            // Step 1. Get bounding volume corners and set the camera to look at the
            //         diffuse mesh


            // Get the 8 corners of the bounding box and transform by the world matrix 
            Vector3[] corners = new Vector3[8];
            //boundingBox = CreateBoundingBox(model);
            //I     think this unnecesarry for now because I already moved them
            //Vector3.Transform(boundingBox, ref WorldMatrix, corners);
            corners = imp.BoundingBox;
            // Get the transformed center of the mesh 
            // alternatively, we could use mBoundingSphere.Center as sometimes it works better


            // Set the camera to be at the center of the reflector and to look at the diffuse mesh

            camera.LookAt(cameraPosition, imp.MeshCenter);
            Vector3 CameraDirection = cameraPosition - imp.BoundingBoxPosition;
            //CameraDirection = cameraPosition - meshCenter;
            imp.Normal = CameraDirection;
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
                screenVerts[i] = device.Viewport.Project(corners[i], camera.Projection, camera.View, Matrix.Identity);

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
                impostorVerts[i] = device.Viewport.Unproject(screenQuadVerts[i], camera.Projection, camera.View, Matrix.Identity);
            }

            //compute the center of the quad
            Vector3 impostorCenter = Vector3.Zero;
            impostorCenter = impostorVerts[0] + impostorVerts[1] + impostorVerts[2] + impostorVerts[3];
            impostorCenter *= .25f;

            // calculate the width and height of the imposter's vertices
            float width = (impostorVerts[1] - impostorVerts[0]).Length() * 1.2f;
            float height = (impostorVerts[3] - impostorVerts[0]).Length() * 1.2f;

            // We construct an Orthographic projection to get rid of the projection distortion
            // which we don't want for our impostor texture
            camera.Projection = Matrix.CreateOrthographic(width, height, .1f, 1000);// I can't set the Projection so if this doesn't work i will try there camera
            camera.BuildView();

            //save the WorldViewProjection matrix so we can use it in the shader
            Matrix worldViewProj = camera.ViewProj;


            //renderTarget = new RenderTarget2D(device, sizeX, sizeY, 1, SurfaceFormat.Color, RenderTargetUsage.DiscardContents);
            Viewport oldViewPort = device.Viewport;
            DepthStencilBuffer oldDepthBuffer = device.DepthStencilBuffer;
            Viewport p = new Viewport();

            p.X = imp.XCoord * size;
            p.Y = imp.YCoord * size;
            p.Width = size;
            p.Height = size;
            p.MaxDepth = oldViewPort.MaxDepth;
            p.MinDepth = oldViewPort.MinDepth;

            device.DepthStencilBuffer = depthBuffer;


            device.SetRenderTarget(0, RenderTarget);
            device.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.Orange, 1f, 0);

            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.AlphaTestEnable = false;
            batch.Begin(SpriteBlendMode.None); //SpriteBlendMode.AlphaBlend);
            batch.Draw(textureOne, new Vector2(0, 0), new Color(255, 255, 255, 255));
            batch.End();

            //DrawScreenQuad();


            device.Viewport = p;
            device.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, new Color(0, 0, 0, 0), 1f, 0);// something needs to be fixed over here
            imp.RenderObject(camera.ViewProj);


            device.Viewport = oldViewPort;
            device.SetRenderTarget(0, null);
            textureOne = RenderTarget.GetTexture();

            device.DepthStencilBuffer = oldDepthBuffer;

            //texture = Texture2D.FromFile(device, game.RootDirectory + "Grass//Textures//grass.tga"); the problem is at the texture it self
            //textureOne.Save(game.EngineFiles.RootDirectory + "texturetest"+".tga", ImageFileFormat.Tga);

            CreatePlain(imp, impostorVerts);
            Imposters[index].NeedsUpdate = false;
            SetupForRender();
        }

        public void NewUpdateImposter()
        {
            for (int i = 0; i < Imposters.Count; i++)
            {
                if (Imposters[i].NeedsUpdate)
                {



                    //MHGameWork.TheWizards.ServerClient.ICamera beginCamera = game.Camera;
                    Vector3[] impostorVerts = new Vector3[8];

                    Vector3 cameraPosition = game.Camera.ViewInverse.Translation;
                    ImposterCamera camera = new ImposterCamera();

                    //----------------------------------------------------------------------
                    // Step 1. Get bounding volume corners and set the camera to look at the
                    //         diffuse mesh


                    // Get the 8 corners of the bounding box and transform by the world matrix 
                    Vector3[] corners = new Vector3[8];
                    //boundingBox = CreateBoundingBox(model);
                    //I     think this unnecesarry for now because I already moved them
                    //Vector3.Transform(boundingBox, ref WorldMatrix, corners);
                    corners = Imposters[i].BoundingBox;
                    // Get the transformed center of the mesh 
                    // alternatively, we could use mBoundingSphere.Center as sometimes it works better


                    // Set the camera to be at the center of the reflector and to look at the diffuse mesh

                    camera.LookAt(cameraPosition, Imposters[i].MeshCenter);
                    Vector3 CameraDirection = cameraPosition - Imposters[i].BoundingBoxPosition;
                    //CameraDirection = cameraPosition - meshCenter;
                    Imposters[i].Normal = CameraDirection;
                    // just for testing
                    //normal = BoundingBoxPosition - game.Camera.ViewProjection.Translation;

                    camera.BuildView();


                    //----------------------------------------------------------------------
                    // Step 2. Project the corners of the bounding volume and construct a 
                    //         screen space quad that fits the boundaries of the mesh


                    // Now we project the vertices to screen space, so we can find the AABB of the screen
                    // space vertices
                    Vector3[] screenVerts = new Vector3[8];
                    for (int j = 0; j < 8; j++)
                    {
                        screenVerts[j] = device.Viewport.Project(corners[j], camera.Projection, camera.View, Matrix.Identity);

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
                    for (int j = 0; j < 4; j++)
                    {
                        impostorVerts[j] = device.Viewport.Unproject(screenQuadVerts[j], camera.Projection, camera.View, Matrix.Identity);
                    }

                    //compute the center of the quad
                    Vector3 impostorCenter = Vector3.Zero;
                    impostorCenter = impostorVerts[0] + impostorVerts[1] + impostorVerts[2] + impostorVerts[3];
                    impostorCenter *= .25f;

                    // calculate the width and height of the imposter's vertices
                    float width = (impostorVerts[1] - impostorVerts[0]).Length() * 1.2f;
                    float height = (impostorVerts[3] - impostorVerts[0]).Length() * 1.2f;

                    // We construct an Orthographic projection to get rid of the projection distortion
                    // which we don't want for our impostor texture
                    camera.Projection = Matrix.CreateOrthographic(width, height, .1f, 1000);// I can't set the Projection so if this doesn't work i will try there camera
                    camera.BuildView();
                    Imposters[i].Cam = camera;
                    //save the WorldViewProjection matrix so we can use it in the shader
                    //Matrix worldViewProj = camera.ViewProj;

                
                //renderTarget = new RenderTarget2D(device, sizeX, sizeY, 1, SurfaceFormat.Color, RenderTargetUsage.DiscardContents);

                 CreatePlain(Imposters[i], impostorVerts);
                Imposters[i].NeedsUpdate = false;
                Imposters[i].NeedsImposterRender = true;
                }
            }
           SetupForRender();
                
            
        }

        public void RenderImposters()
        {
            Viewport oldViewPort = device.Viewport;
            DepthStencilBuffer oldDepthBuffer = device.DepthStencilBuffer;
            

            device.DepthStencilBuffer = depthBuffer;


            device.SetRenderTarget(0, RenderTarget);
            device.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.Orange, 1f, 0);


            device.RenderState.AlphaBlendEnable = false;
            device.RenderState.AlphaTestEnable = false;
            batch.Begin(SpriteBlendMode.None); //SpriteBlendMode.AlphaBlend);
            batch.Draw(textureOne, new Vector2(0, 0), new Color(255, 255, 255, 255));
            batch.End();

            //DrawScreenQuad();

            for (int i = 0; i < Imposters.Count; i++)
            {

                if(Imposters[i].NeedsImposterRender)
                {
                    device.Viewport = Imposters[i].VPort;
                    device.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, new Color(0, 0, 0, 0), 1f, 0);// something needs to be fixed over here
                    Imposters[i].RenderObject(Imposters[i].Cam.ViewProj);


               


                Imposters[i].NeedsImposterRender = false;
                //texture = Texture2D.FromFile(device, game.RootDirectory + "Grass//Textures//grass.tga"); the problem is at the texture it self
                }//textureOne.Save(game.EngineFiles.RootDirectory + "texturetest"+".tga", ImageFileFormat.Tga);
            } 
            device.Viewport = oldViewPort;
            device.SetRenderTarget(0, null);
            textureOne = RenderTarget.GetTexture();
            device.DepthStencilBuffer = oldDepthBuffer;
            device.Clear(ClearOptions.DepthBuffer | ClearOptions.Target, Color.CornflowerBlue, 1f, 0);
            


        }
        public void CreatePlain(ImposterStruct imp, Vector3[] impostorVerts)
        {
            Vector3 tangent = Vector3.Up;
            Vector2 uvStart, uvEnd;

            float tempSize = 1 / ((float)ImpostersTextureSize / size);

            // NOTE: Bart, dit was wel een heel raar stukje, ik heb veranderd door beneden. Als wat ik gedaan heb toch niet juist was
            //   dan zet je dit maar terug aan

            /*if (imp.XCoord == 0)
            {
                if (imp.YCoord == 0)
                {
                    uvStart = new Vector2(0, 0);
                    uvEnd = new Vector2(tempSize, tempSize);
                }
                else
                {
                    uvStart = new Vector2(0, 1f / imp.YCoord);
                    uvEnd = new Vector2(tempSize, (1f / imp.YCoord) + tempSize);
                }
            }
            else
            {
                if (imp.YCoord == 0)
                {
                    uvStart = new Vector2(1f / imp.XCoord, 0);
                    uvEnd = new Vector2((1f / imp.XCoord) + tempSize, tempSize);
                }
                else
                {
                    uvStart = new Vector2(1f / imp.XCoord, 1f / imp.YCoord);
                    uvEnd = new Vector2((1f / imp.XCoord) + tempSize, (1f / imp.YCoord) + tempSize);
                }
            }*/

            uvStart = new Vector2(imp.XCoord * tempSize, imp.YCoord * tempSize);
            uvEnd = new Vector2((imp.XCoord + 1) * tempSize, (imp.YCoord + 1) * tempSize);


            //32 this depends on how many pixels you give for each imposter
            //reminder if this doesn't work perhaps you still have to put an f after it for float
            //triangle 1
            Vertices[imp.Index * 6 + 0] = new TangentVertex(impostorVerts[0], uvStart.X, uvStart.Y, imp.Normal, tangent);
            Vertices[imp.Index * 6 + 1] = new TangentVertex(impostorVerts[3], uvStart.X, uvEnd.Y, imp.Normal, tangent);
            Vertices[imp.Index * 6 + 2] = new TangentVertex(impostorVerts[2], uvEnd.X, uvEnd.Y, imp.Normal, tangent);
            //triangle 2
            Vertices[imp.Index * 6 + 3] = new TangentVertex(impostorVerts[0], uvStart.X, uvStart.Y, imp.Normal, tangent);
            Vertices[imp.Index * 6 + 4] = new TangentVertex(impostorVerts[1], uvEnd.X, uvStart.Y, imp.Normal, tangent);
            Vertices[imp.Index * 6 + 5] = new TangentVertex(impostorVerts[2], uvEnd.X, uvEnd.Y, imp.Normal, tangent);
        }

        public void HideImposterPlain(int index)
        {
            Vertices[index * 6 + 0] = new TangentVertex(Vector3.Zero, 0, 0,Vector3.Zero, Vector3.Zero);
            Vertices[index * 6 + 1] = new TangentVertex(Vector3.Zero, 0, 0, Vector3.Zero, Vector3.Zero);
            Vertices[index * 6 + 2] = new TangentVertex(Vector3.Zero, 0, 0, Vector3.Zero, Vector3.Zero);
            //triangle 2
            Vertices[index * 6 + 3] = new TangentVertex(Vector3.Zero, 0, 0, Vector3.Zero, Vector3.Zero);
            Vertices[index * 6 + 4] = new TangentVertex(Vector3.Zero, 0, 0, Vector3.Zero, Vector3.Zero);
            Vertices[index * 6 + 5] = new TangentVertex(Vector3.Zero, 0, 0, Vector3.Zero, Vector3.Zero);
        }

        public void SetupForRender()
        {
            //set up every thing for rendering the plain
            if (vertexCount != Vertices.Count)
            {
                decl = TangentVertex.CreateVertexDeclaration(game);
                vertexStride = TangentVertex.SizeInBytes;
                vertexCount = Vertices.Count;
                triangleCount = vertexCount / 3;

                vertexBuffer = new DynamicVertexBuffer(device, typeof(TangentVertex), vertexCount, BufferUsage.None);
                Shader.DiffuseTexture = textureOne;
            }
            vertexBuffer.SetData(Vertices.ToArray());
            
           

        }

        public bool CheckAngle(ImposterStruct imp)
        {

            float angle = MathHelper.ToDegrees(AngleBetweenTwoV3(imp.BoundingBoxPosition, game.Camera.ViewInverse.Translation));

            if (angle > imp.MaxAngleDifference)
            {
                return true;
            }
            return false;
        }

        public float AngleBetweenTwoV3(Vector3 v1, Vector3 v2)
        {
            v1.Normalize();
            v2.Normalize();
            double Angle = (float)Math.Acos(Vector3.Dot(v1, v2));
            return (float)Angle;
        }// in radians
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


        public static void TestImposterEngine()
        {
            XNAGame game;
            game = new XNAGame();
            TreeEngine.EngineTreeType tree = new TreeGenerator.TreeEngine.EngineTreeType();

            TreeEngine.EngineTreeRenderData data = new TreeGenerator.TreeEngine.EngineTreeRenderData(game);
            TreeEngine.EngineTreeRenderDataGenerater gen = new TreeGenerator.TreeEngine.EngineTreeRenderDataGenerater(21);
            TreeStructure treeStruct = new TreeStructure();

            TreeRenderManager bodyManager = new TreeRenderManager();
            TreeRenderManager leafManager = new TreeRenderManager();
            gen.GetRenderData(treeStruct, game,0);
            data = gen.TreeRenderData;


            ImposterEngine engine = new ImposterEngine(game);
            game.InitializeEvent +=
               delegate
               {

                   data.Initialize();
                   engine.Initialze();
                   //engine.AddRenderObject(data.draw ,data.boundingBox);
               };
            game.UpdateEvent +=
                delegate
                {
                    engine.Update();
                };

            game.DrawEvent +=
                delegate
                {

                    engine.Render();
                    //data.draw();
                };
            game.Run();
        }
    }

    public class ImposterStruct // don't know or I should make this an struct or an class
    {
        public ImposterEngine.RenderDelegate RenderObject;

        private bool needsUpdate = true;

        public bool NeedsUpdate
        {
            get { return needsUpdate; }
            set
            {
                needsUpdate = value;
                if (needsUpdate == true && value == false)
                {
                    time = 0;
                }
            }
        }
        public bool DrawMesh = false;
        public bool DrawImposter = false;

        public float MinDistance = 40;
        public float MaxAngleDifference = 10f;
        public float MaxAngleDiffernceAtMinDistance = 5f;// in degrees not in radians like in the other imposter disgn
        public float MaxTime = 1f;
        public float MaxtimeAtMinDist = 0.1f;
        private float time = 0;
        public float Time
        {
            get { return time; }
            set { time = value; }
        }

        public int XCoord, YCoord, Index;

        public Vector3[] BoundingBox = new Vector3[8];
        public Vector3 MeshCenter;
        public Vector3 BoundingBoxPosition;
        private Vector3 normal;

        public Vector3 Normal
        {
            get { return normal; }
            set { normal = value; normal.Normalize(); }
        }
        private float distance;


        public float Distance
        {
            get { return distance; }
            set
            {
                distance = value;
                MaxTime = MaxtimeAtMinDist + (value - MinDistance) * 0.01f;
                MaxAngleDifference = MaxAngleDifference + (Distance - MinDistance) * 0.1f;// not sure perp


            }
        }

        // just for the new UpdateImposterClass
        public ImposterCamera Cam = new ImposterCamera();
        public Viewport VPort = new Viewport();
        public bool NeedsImposterRender = false;

        public bool DiscartRenderData = false;
        public bool LoadRenderData = false;
        public bool RenderDataGone = false;
        public ImposterStruct(int xCo, int yCo, int index, ImposterEngine.RenderDelegate _renderObject, Vector3[] boundingBox,int size)
        {
            XCoord = xCo;
            YCoord = yCo;
            Index = index;
            RenderObject = _renderObject;
            BoundingBox = boundingBox;
            Cam = new ImposterCamera();
            VPort = new Viewport();
            VPort.X = XCoord * size;
            VPort.Y = YCoord * size;
            VPort.Width = size;
            VPort.Height = size;
            VPort.MaxDepth = 1.0f;
            VPort.MinDepth = 0.0f;
            BoundingBoxPosition = new Vector3((boundingBox[1].X + boundingBox[0].X) * 0.5f, boundingBox[0].Y, (boundingBox[2].Z + boundingBox[0].Z) * 0.5f);
            CalculateCenter();
        }

        public void CalculateCenter()
        {

            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < BoundingBox.Length; i++)
            {
                min = Vector3.Min(min, BoundingBox[i]);
                max = Vector3.Max(max, BoundingBox[i]);
            }

            MeshCenter = (min + max) * 0.5f;

        }
    }



}
