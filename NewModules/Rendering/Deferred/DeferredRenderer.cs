using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Common.Core;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.OBJParser;
using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    /// <summary>
    /// This class is the core class for a Deferred Shading implementation
    /// Sources:
    /// http://game-developers.org/node/177
    /// </summary>
    public class DeferredRenderer : IXNAObject
    {

        private RenderTarget2D diffuseRT;    //this Render Target will hold color and Specular Intensity
        private RenderTarget2D normalRT; //this Render Target will hold normals and Specular Power
        private RenderTarget2D depthRT; //finally, this one will hold the depth

        private IXNAGame game;

        public DeferredRenderer(RAMMesh merchantsHouseMesh, string woodPlanksBarePath)
        {
            this.merchantsHouseMesh = merchantsHouseMesh;
            this.woodPlanksBarePath = woodPlanksBarePath;
        }

        private GraphicsDevice GraphicsDevice { get { return game.GraphicsDevice; } }

        private BasicShader clearShader;
        private BasicShader deferredShader;
        private BasicShader renderGBufferShader;
        private BasicShader directionalLightShader;
        private BasicShader finalCombineEffect;
        private BasicShader pointLightShader;
        private BasicShader spotLightShader;

        private FullScreenQuad fullScreenQuad;
        private VertexDeclaration tangentVertexDeclaration;
        private RAMMesh mesh;
        private TexturePool texturePool;
        private MeshPartPool meshPartPool;
        private Texture2D boxTexture;
        private Vector2 halfPixel;
        private RenderTarget2D lightRT;
        private float totalSeconds;
        private Texture2D checkerTexture;
        private BasicShader shadowMapShader;
        private RenderTarget2D shadowMapRT;
        private DepthStencilBuffer shadowMapDS;

        private int shadowMapSize = 2048;

        Vector3 tempMovePos = Vector3.Up * 5;
        private BasicShader ambientOcclusionShader;
        private Texture2D randomNormalsTexture;
        private RenderTarget2D ambientOcclusionRT;
        private BasicShader blurShader;
        private RenderTarget2D blurRT;
        private BasicShader downsampleShader;
        public DeferredOutputMode OutputMode { get; set; }

        private Point downsampleSize;
        private Vector2 downsampleHalfPixel;
        private RenderTarget2D downsampleRT;
        private RenderTarget2D downsampleBlurYRT;
        private RenderTarget2D deferredFinalRT;
        private RenderTarget2D currentFrameLuminance;
        private RenderTarget2D currentFrameAdaptedLuminance;
        private RenderTarget2D lastFrameAdaptedLuminance;
        private RenderTarget2D[] luminanceChain;
        private RenderTarget2D downscaleRT;
        private RenderTarget2D downscale1RT;
        private RenderTarget2D downscale2RT;
        private RenderTarget2D downscale3RT;
        private RAMMesh merchantsHouseMesh;
        private string woodPlanksBarePath;

        public enum DeferredOutputMode
        {
            None = 0,
            LightAccumulation,
            Diffuse,
            Normal,
            SpecularExponent,
            SpecularPower,
            Depth,
            ShadowMap,
            AmbientOcclusion,
            BlurX,
            BlurY,
            Downsample,
            DownsampleBlurX,
            DownsampleBlurY,
            Final,
            ToneMapped
        }

        public void Initialize(IXNAGame _game)
        {
            game = _game;
            //get the sizes of the backbuffer, in order to have matching render targets   
            int backBufferWidth = _game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int backBufferHeight = _game.GraphicsDevice.PresentationParameters.BackBufferHeight;


            initializeDeferredRendering(_game, backBufferWidth, backBufferHeight);

            initializeDeferredShading(_game, backBufferWidth, backBufferHeight);


            ambientOcclusionRT = new RenderTarget2D(_game.GraphicsDevice, backBufferWidth,
                                                        backBufferHeight, 1, SurfaceFormat.Color); //TODO: use better channel here
            blurRT = new RenderTarget2D(_game.GraphicsDevice, backBufferWidth,
                                                        backBufferHeight, 1, SurfaceFormat.Color); //TODO: use better channel here




            downsampleSize = new Point(backBufferWidth / 2, backBufferHeight / 2);
            downsampleHalfPixel.X = 0.5f / downsampleSize.X;
            downsampleHalfPixel.Y = 0.5f / downsampleSize.Y;
            downsampleRT = new RenderTarget2D(_game.GraphicsDevice, downsampleSize.X,
                                                        downsampleSize.Y, 1, SurfaceFormat.Color);
            downsampleBlurYRT = new RenderTarget2D(_game.GraphicsDevice, downsampleSize.X,
                                            downsampleSize.Y, 1, SurfaceFormat.Color);







            deferredShader = loadShader("DeferredShader.fx");





            ambientOcclusionShader = loadShader("AmbientOcclusion.fx");

            blurShader = loadShader("Blur.fx");

            downsampleShader = loadShader("Downsample.fx");


            fullScreenQuad = new FullScreenQuad(GraphicsDevice);
            tangentVertexDeclaration = TangentVertex.CreateVertexDeclaration(game);


            mesh = merchantsHouseMesh;
            texturePool = new TexturePool();

            texturePool.Initialize(game);
            meshPartPool = new MeshPartPool();
            meshPartPool.Initialize(game);

            boxTexture = Texture2D.FromFile(GraphicsDevice, new FileStream(woodPlanksBarePath, FileMode.Open, FileAccess.Read, FileShare.Read));
            halfPixel.X = 0.5f / GraphicsDevice.PresentationParameters.BackBufferWidth;
            halfPixel.Y = 0.5f / GraphicsDevice.PresentationParameters.BackBufferHeight;



            checkerTexture = Texture2D.FromFile(game.GraphicsDevice,
                                               EmbeddedFile.GetStream(
                                                   "MHGameWork.TheWizards.Rendering.Files.Checker.png", "Checker.png"));


            randomNormalsTexture = Texture2D.FromFile(game.GraphicsDevice,
                                               EmbeddedFile.GetStream(
                                                   "MHGameWork.TheWizards.Rendering.Deferred.Files.RandomNormals.png", "RandomNormals.png"));

            initializeToneMap();
        }

        private void initializeDeferredShading(IXNAGame _game, int backBufferWidth, int backBufferHeight)
        {
            shadowMapRT = new RenderTarget2D(_game.GraphicsDevice, shadowMapSize,
                                             shadowMapSize, 1, SurfaceFormat.Single);
            shadowMapDS = new DepthStencilBuffer(GraphicsDevice,
                                                 shadowMapSize,
                                                 shadowMapSize,
                                                 GraphicsDevice.DepthStencilBuffer.Format);

            lightRT = new RenderTarget2D(GraphicsDevice, backBufferWidth,
                                         backBufferHeight, 1, SurfaceFormat.HalfVector4, RenderTargetUsage.PreserveContents);

            deferredFinalRT = new RenderTarget2D(GraphicsDevice, backBufferWidth,
                                                  backBufferHeight, 1, SurfaceFormat.HalfVector4, RenderTargetUsage.PreserveContents);


            directionalLightShader = loadShader("DirectionalLight.fx");
            directionalLightShader.SetTechnique("Technique0");

            finalCombineEffect = loadShader("CombineFinal.fx");
            finalCombineEffect.SetTechnique("Technique1");


            pointLightShader = loadShader("PointLight.fx");
            pointLightShader.SetTechnique("Technique1");


            spotLightShader = loadShader("SpotLight.fx");
            spotLightShader.SetTechnique("Technique1");

            shadowMapShader = loadShader("ShadowMap.fx");
        }

        private void initializeDeferredRendering(IXNAGame _game, int backBufferWidth, int backBufferHeight)
        {
            diffuseRT = new RenderTarget2D(_game.GraphicsDevice, backBufferWidth,
                                           backBufferHeight, 1, SurfaceFormat.Color);
            normalRT = new RenderTarget2D(_game.GraphicsDevice, backBufferWidth,
                                          backBufferHeight, 1, SurfaceFormat.Color);
            depthRT = new RenderTarget2D(_game.GraphicsDevice, backBufferWidth,
                                         backBufferHeight, 1, SurfaceFormat.Single);


            clearShader = loadShader("ClearGBuffer.fx");
            clearShader.SetTechnique("Technique1");
            renderGBufferShader = loadShader("RenderGBuffer.fx");
            renderGBufferShader.SetTechnique("Technique1");
        }

        private void initializeToneMap()
        {
            // Initialize our buffers
            int width = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int height = GraphicsDevice.PresentationParameters.BackBufferHeight;

            currentFrameLuminance = new RenderTarget2D(GraphicsDevice, 1, 1, 1, SurfaceFormat.Single, RenderTargetUsage.DiscardContents);
            currentFrameAdaptedLuminance = new RenderTarget2D(GraphicsDevice, 1, 1, 1, SurfaceFormat.Single, RenderTargetUsage.DiscardContents);
            lastFrameAdaptedLuminance = new RenderTarget2D(GraphicsDevice, 1, 1, 1, SurfaceFormat.Single, RenderTargetUsage.DiscardContents);
            downscaleRT = new RenderTarget2D(GraphicsDevice, width / 16, height / 16, 1, SurfaceFormat.HalfVector4, RenderTargetUsage.DiscardContents);

            GraphicsDevice.SetRenderTarget(0, lastFrameAdaptedLuminance);
            GraphicsDevice.Clear(Color.White);
            GraphicsDevice.SetRenderTarget(0, null);



            // We need a luminance chain
            int chainLength = 1;
            int startSize = (int)MathHelper.Min(width / 16, height / 16);
            int size = 16;
            for (size = 16; size < startSize; size *= 4)
                chainLength++;

            luminanceChain = new RenderTarget2D[chainLength];
            size /= 4;
            for (int i = 0; i < chainLength; i++)
            {
                luminanceChain[i] = new RenderTarget2D(GraphicsDevice, size, size, 1, SurfaceFormat.Single);
                size /= 4;
            }


            downscale1RT = new RenderTarget2D(GraphicsDevice, width / 2, height / 2, 1, SurfaceFormat.HalfVector4, RenderTargetUsage.DiscardContents);
            downscale2RT = new RenderTarget2D(GraphicsDevice, width / 2, height / 2, 1, SurfaceFormat.HalfVector4, RenderTargetUsage.DiscardContents);
            downscale3RT = new RenderTarget2D(GraphicsDevice, width / 2, height / 2, 1, SurfaceFormat.HalfVector4, RenderTargetUsage.DiscardContents);



        }

        private BasicShader loadShader(string filename)
        {
            return BasicShader.LoadFromEmbeddedFile(game, typeof(DeferredRenderer).Assembly, "MHGameWork.TheWizards.Rendering.Deferred.Files." + filename,
                                                                      "..\\..\\NewModules\\Rendering\\Deferred\\Files\\" + filename, null);
        }


        public void Render(IXNAGame _game)
        {

            GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            GraphicsDevice.RenderState.DepthBufferEnable = true;

            SetGBuffer();
            ClearGBuffer();
            DrawScene();
            ResolveGBuffer();



            GraphicsDevice.Clear(ClearOptions.Target, Color.White, 1.0f, 0);
            GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            deferredShader.Effect.Parameters["colorMap"].SetValue(diffuseRT.GetTexture());
            deferredShader.Effect.Parameters["normalMap"].SetValue(normalRT.GetTexture());
            deferredShader.Effect.Parameters["depthMap"].SetValue(depthRT.GetTexture());

            GraphicsDevice.RenderState.AlphaBlendEnable = false;
            if (OutputMode == DeferredOutputMode.Diffuse)
            {
                deferredShader.SetTechnique("Diffuse");
                deferredShader.RenderMultipass(fullScreenQuad.Draw);
            }
            else if (OutputMode == DeferredOutputMode.Normal)
            {
                deferredShader.SetTechnique("Normal");
                deferredShader.RenderMultipass(fullScreenQuad.Draw);
            }
            else if (OutputMode == DeferredOutputMode.Depth)
            {
                deferredShader.SetTechnique("Depth");
                deferredShader.RenderMultipass(fullScreenQuad.Draw);
            }
            else if (OutputMode == DeferredOutputMode.AmbientOcclusion)
            {
                DrawAmbientOcclusion();
            }
            else if (OutputMode == DeferredOutputMode.BlurX || OutputMode == DeferredOutputMode.BlurY)
            {
                DrawAmbientOcclusion();
                BlurAmbientOcclusion();
            }
            /*else if (OutputMode == DeferredOutputMode.Downsample || OutputMode == DeferredOutputMode.DownsampleBlurX || OutputMode == DeferredOutputMode.DownsampleBlurY)
            {
                DrawAmbientOcclusion();
                DrawLights();
                drawHDRDownsampled();
            }*/
            else if (OutputMode == DeferredOutputMode.LightAccumulation || OutputMode == DeferredOutputMode.ShadowMap)
            {
                DrawLights();
            }
            else
            {
                DrawAmbientOcclusion();
                DrawLights();
                //drawHDRDownsampled();

                DrawCombineFinal();

                GenerateDownscaleTargetHW(deferredFinalRT, downscaleRT);

                CalculateAverageLuminance(downscaleRT, game.Elapsed, false);

                if (OutputMode == DeferredOutputMode.ToneMapped)
                {
                    DrawToneMapped();
                }

            }



        }

        /// <summary>
        /// Downscales the source to 1/16th size, using hardware filtering
        /// </summary>
        /// <param name="source">The source to be downscaled</param>
        /// <param name="result">The RT in which to store the result</param>
        protected void GenerateDownscaleTargetHW(RenderTarget2D source, RenderTarget2D result)
        {
            GraphicsDevice.SetRenderTarget(0, downscale1RT);
            GraphicsDevice.Clear(Color.Orange);
            finalCombineEffect.SetTechnique("ScaleHW");
            finalCombineEffect.SetParameter("linearTex0", source.GetTexture());
            finalCombineEffect.RenderMultipass(fullScreenQuad.Draw);

            GraphicsDevice.SetRenderTarget(0, downscale2RT);
            GraphicsDevice.Clear(Color.Orange);
            finalCombineEffect.SetParameter("linearTex0", downscale1RT.GetTexture());
            finalCombineEffect.RenderMultipass(fullScreenQuad.Draw);

            GraphicsDevice.SetRenderTarget(0, downscale3RT);
            GraphicsDevice.Clear(Color.Orange);
            finalCombineEffect.SetParameter("linearTex0", downscale2RT.GetTexture());
            finalCombineEffect.RenderMultipass(fullScreenQuad.Draw);

            GraphicsDevice.SetRenderTarget(0, result);
            GraphicsDevice.Clear(Color.Orange);
            finalCombineEffect.SetParameter("linearTex0", downscale3RT.GetTexture());
            finalCombineEffect.RenderMultipass(fullScreenQuad.Draw);

            GraphicsDevice.SetRenderTarget(0, null);

            if (OutputMode == DeferredOutputMode.Downsample)
            {
                ((XNAGame)game).SpriteBatch.Begin(SpriteBlendMode.None);
                ((XNAGame)game).SpriteBatch.Draw(downscale1RT.GetTexture(), new Vector2(0, 0), Color.White);
                ((XNAGame)game).SpriteBatch.Draw(downscale2RT.GetTexture(), new Vector2(downscale1RT.Width, 0), Color.White);
                ((XNAGame)game).SpriteBatch.Draw(downscale3RT.GetTexture(), new Vector2(0, downscale1RT.Height), Color.White);
                ((XNAGame)game).SpriteBatch.Draw(result.GetTexture(), new Vector2(downscale3RT.Width, downscale1RT.Height), Color.White);
                ((XNAGame)game).SpriteBatch.End();
            }


        }

        private void drawHDRDownsampled()
        {
            GraphicsDevice.SetRenderTarget(0, downsampleRT);

            GraphicsDevice.Clear(Color.TransparentBlack);

            downsampleShader.SetParameter("lightMap", lightRT.GetTexture());
            downsampleShader.SetParameter("diffuseMap", diffuseRT.GetTexture());
            downsampleShader.SetParameter("halfPixel", downsampleHalfPixel);
            downsampleShader.SetTechnique("Technique1");

            downsampleShader.RenderMultipass(fullScreenQuad.Draw);
            GraphicsDevice.SetRenderTarget(0, null);

            if (OutputMode == DeferredOutputMode.Downsample)
            {
                ((XNAGame)game).SpriteBatch.Begin(SpriteBlendMode.None);
                ((XNAGame)game).SpriteBatch.Draw(downsampleRT.GetTexture(), new Vector2(0, 0), Color.White);
                ((XNAGame)game).SpriteBatch.End();
                return;
            }

            GraphicsDevice.SetRenderTarget(0, downsampleBlurYRT);
            blurShader.SetParameter("blurMap", downsampleRT.GetTexture());
            blurShader.SetParameter("BlurOffset", new Vector2(downsampleHalfPixel.X, 0));
            blurShader.SetParameter("halfPixel", downsampleHalfPixel);
            blurShader.SetTechnique("GaussionBlur");

            blurShader.RenderMultipass(fullScreenQuad.Draw);

            GraphicsDevice.SetRenderTarget(0, null);

            if (OutputMode == DeferredOutputMode.DownsampleBlurX)
            {
                ((XNAGame)game).SpriteBatch.Begin(SpriteBlendMode.None);
                ((XNAGame)game).SpriteBatch.Draw(downsampleBlurYRT.GetTexture(), new Vector2(0, 0), Color.White);
                ((XNAGame)game).SpriteBatch.End();
                return;
            }

            GraphicsDevice.SetRenderTarget(0, downsampleRT);

            blurShader.SetParameter("blurMap", downsampleBlurYRT.GetTexture());
            blurShader.SetParameter("BlurOffset", new Vector2(0, downsampleHalfPixel.Y));
            blurShader.SetParameter("halfPixel", downsampleHalfPixel);
            blurShader.SetTechnique("GaussionBlur");

            blurShader.RenderMultipass(fullScreenQuad.Draw);

            GraphicsDevice.SetRenderTarget(0, null);

            if (OutputMode == DeferredOutputMode.DownsampleBlurY)
            {
                ((XNAGame)game).SpriteBatch.Begin(SpriteBlendMode.None);
                ((XNAGame)game).SpriteBatch.Draw(downsampleRT.GetTexture(), new Vector2(0, 0), Color.White);
                ((XNAGame)game).SpriteBatch.End();
                return;
            }

        }

        private void drawRTTexture(RenderTarget2D renderTarget)
        {
            ((XNAGame)game).SpriteBatch.Begin(SpriteBlendMode.None);
            ((XNAGame)game).SpriteBatch.Draw(renderTarget.GetTexture(), new Vector2(0, 0), Color.White);
            ((XNAGame)game).SpriteBatch.End();
        }

        private void DrawLights()
        {

            if (OutputMode != DeferredOutputMode.LightAccumulation && OutputMode != DeferredOutputMode.ShadowMap)
                GraphicsDevice.SetRenderTarget(0, lightRT);

            //clear all components to 0
            GraphicsDevice.Clear(Color.TransparentBlack);
            setRenderStatesLightAccumulation();


            //draw some lights
            //DrawDirectionalLight(Vector3.Normalize(new Vector3(-1, -2, 1)), Color.White);
            //DrawDirectionalLight(new Vector3(0, -1, 0), Color.White);
            //DrawDirectionalLight(new Vector3(-1, 0, 0), Color.Crimson);
            //DrawDirectionalLight(new Vector3(1, 0, 0), Color.SkyBlue);
            //DrawDirectionalLight(new Vector3(0, -1f, 1), Color.DimGray);
            //DrawPointLight(new Vector3(1 * (float)Math.Sin(totalSeconds), 1, 1 * (float)Math.Cos(totalSeconds)), Color.White, 2, 4);
            /*DrawPointLight(new Vector3(-106, 10, -2)
                + new Vector3(10 * (float)Math.Sin(totalSeconds), 1, 10 * (float)Math.Cos(totalSeconds))
                , Color.White, 30, 4);*/

            DrawSpotLight(new Vector3(-111.1687f, 9.255297f, -6.912942f), Color.White, 20, 1, new Vector3(0.7912793f, -0.2672373f, 0.5499648f), MathHelper.ToRadians(30), 1);

            DrawSpotLight(tempMovePos, Color.White, 6, 1, Vector3.Down, MathHelper.ToRadians(30), 1);
            Vector3 target = new Vector3(-106, 0, -2);
            //Vector3 pos = new Vector3(-95, 20, -6);
            Vector3 pos = new Vector3(-106, 30, -2)
                          + new Vector3(30 * (float)Math.Sin(totalSeconds * 0.3f), 0, 30 * (float)Math.Cos(totalSeconds * 0.3f));
            if (game.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
            {
                GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;

            }
            else
            {
                GraphicsDevice.RenderState.FillMode = FillMode.Solid;

            }
            var dir = Vector3.Normalize(target - pos);

            DrawSpotLight(pos, Color.White, 200, 1, dir, MathHelper.ToRadians(20), 1);



            clearRenderStatesLightAccumulation();

            if (OutputMode != DeferredOutputMode.LightAccumulation && OutputMode != DeferredOutputMode.ShadowMap)
                GraphicsDevice.SetRenderTarget(0, null);

        }

        private void clearRenderStatesLightAccumulation()
        {
            GraphicsDevice.RenderState.AlphaBlendEnable = false;
            GraphicsDevice.RenderState.DestinationBlend = Blend.Zero;
        }

        private void setRenderStatesLightAccumulation()
        {
            GraphicsDevice.RenderState.AlphaBlendEnable = true;
            //use additive blending, and make sure the blending factors are as we need them
            GraphicsDevice.RenderState.AlphaBlendOperation = BlendFunction.Add;
            GraphicsDevice.RenderState.SourceBlend = Blend.One;
            GraphicsDevice.RenderState.DestinationBlend = Blend.One;
            //use the same operation on the alpha channel
            GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = false;
        }

        private void DrawCombineFinal()
        {
            GraphicsDevice.SetRenderTarget(0, deferredFinalRT);
            GraphicsDevice.Clear(Color.TransparentBlack);

            //set the effect parameters
            finalCombineEffect.Effect.Parameters["colorMap"].SetValue(diffuseRT.GetTexture());
            finalCombineEffect.Effect.Parameters["lightMap"].SetValue(lightRT.GetTexture());
            finalCombineEffect.Effect.Parameters["ambientOcclusionMap"].SetValue(ambientOcclusionRT.GetTexture());
            //finalCombineEffect.Effect.Parameters["hdrMap"].SetValue(downsampleRT.GetTexture());
            finalCombineEffect.Effect.Parameters["halfPixel"].SetValue(halfPixel);
            finalCombineEffect.SetTechnique("Technique1");

            finalCombineEffect.RenderMultipass(fullScreenQuad.Draw);

            GraphicsDevice.SetRenderTarget(0, null);

            if (OutputMode == DeferredOutputMode.Final)
                drawRTTexture(deferredFinalRT);

        }

        private void DrawToneMapped()
        {
            GraphicsDevice.Clear(Color.TransparentBlack);

            //set the effect parameters
            finalCombineEffect.Effect.Parameters["finalMap"].SetValue(deferredFinalRT.GetTexture());
            finalCombineEffect.Effect.Parameters["halfPixel"].SetValue(halfPixel);
            finalCombineEffect.SetTechnique("TechniqueToneMap");
            finalCombineEffect.RenderMultipass(fullScreenQuad.Draw);
        }

        private void DrawScene()
        {
            //drawCone();
            drawBox();

            drawMesh(mesh);
        }
        private void DrawSceneShadowMap()
        {
            //drawCone();
            drawBoxShadowMap();

            drawMeshShadowMap(mesh);
        }

        private void drawBoxShadowMap()
        {
            TangentVertex[] vertices;
            short[] sIndices;

            BoxMesh.CreateUnitBoxVerticesAndIndices(out vertices, out sIndices);
            int[] indices = new int[sIndices.Length];
            for (int i = 0; i < sIndices.Length; i++)
            {
                indices[i] = sIndices[i];
            }


            GraphicsDevice.VertexDeclaration = tangentVertexDeclaration;


            shadowMapShader.SetParameter("g_matWorld", Matrix.Identity);

            shadowMapShader.RenderMultipass(delegate
            {
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, sIndices,
                                                         0, sIndices.Length / 3);
            });
        }

        private void drawMeshShadowMap(RAMMesh mesh)
        {
            GraphicsDevice.RenderState.CullMode = CullMode.None;
            GraphicsDevice.VertexDeclaration = tangentVertexDeclaration;
            var data = mesh.GetCoreData();
            for (int i = 0; i < data.Parts.Count; i++)
            {
                var part = data.Parts[i];
                shadowMapShader.SetParameter("g_matWorld", part.ObjectMatrix);

                GraphicsDevice.Vertices[0].SetSource(meshPartPool.GetVertexBuffer(part.MeshPart), 0,
                                                     TangentVertex.SizeInBytes);

                GraphicsDevice.Indices = meshPartPool.GetIndexBuffer(part.MeshPart);

                int numVertices =
                    part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position).Length;


                shadowMapShader.RenderMultipass(delegate
                {
                    GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numVertices / 3);

                });



            }
        }
        private void drawBox()
        {
            TangentVertex[] vertices;
            short[] sIndices;

            BoxMesh.CreateUnitBoxVerticesAndIndices(out vertices, out sIndices);
            int[] indices = new int[sIndices.Length];
            for (int i = 0; i < sIndices.Length; i++)
            {
                indices[i] = sIndices[i];
            }


            GraphicsDevice.VertexDeclaration = tangentVertexDeclaration;


            renderGBufferShader.SetParameter("World", Matrix.Identity);
            renderGBufferShader.SetParameter("View", game.Camera.View);
            renderGBufferShader.SetParameter("Projection", game.Camera.Projection);
            renderGBufferShader.SetParameter("Texture", boxTexture);

            renderGBufferShader.RenderMultipass(delegate
                                              {
                                                  GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, sIndices,
                                                                                           0, sIndices.Length / 3);
                                              });
        }
        private void drawMesh(RAMMesh mesh)
        {
            GraphicsDevice.RenderState.CullMode = CullMode.None;
            GraphicsDevice.VertexDeclaration = tangentVertexDeclaration;
            var data = mesh.GetCoreData();
            for (int i = 0; i < data.Parts.Count; i++)
            {
                var part = data.Parts[i];
                renderGBufferShader.SetParameter("World", part.ObjectMatrix);
                if (part.MeshMaterial.DiffuseMap == null)
                    renderGBufferShader.SetParameter("Texture", checkerTexture);
                else
                    renderGBufferShader.SetParameter("Texture", texturePool.LoadTexture(part.MeshMaterial.DiffuseMap));

                GraphicsDevice.Vertices[0].SetSource(meshPartPool.GetVertexBuffer(part.MeshPart), 0,
                                                     TangentVertex.SizeInBytes);

                GraphicsDevice.Indices = meshPartPool.GetIndexBuffer(part.MeshPart);

                int numVertices =
                    part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position).Length;


                renderGBufferShader.RenderMultipass(delegate
                {
                    GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numVertices / 3);

                });



            }
        }
        /// <summary>
        /// Calculates the average luminance of the scene
        /// </summary>
        /// <param name="downscaleBuffer">The scene texure, downscaled to 1/16th size</param>
        /// <param name="dt">The time delta</param>
        /// <param name="encoded">If true, the image is encoded in LogLuv format</param>
        protected void CalculateAverageLuminance(RenderTarget2D downscaleBuffer, float dt, bool encoded)
        {
            // Calculate the initial luminance
            /*if (encoded)
                HDREffect.CurrentTechnique = HDREffect.Techniques["LuminanceEncode"];
            else*/

            finalCombineEffect.SetTechnique("Luminance");
            GraphicsDevice.SetRenderTarget(0, luminanceChain[0]);
            finalCombineEffect.SetParameter("pointTex0", downscaleBuffer.GetTexture());
            finalCombineEffect.SetParameter("halfPixel", calculateHalfPixelFromRT(downscaleBuffer));
            finalCombineEffect.RenderMultipass(fullScreenQuad.Draw);

            // Repeatedly downscale            
            finalCombineEffect.SetTechnique("Downscale4");

            for (int i = 1; i < luminanceChain.Length; i++)
            {
                GraphicsDevice.SetRenderTarget(0, luminanceChain[i]);
                finalCombineEffect.SetParameter("pointTex0", luminanceChain[i - 1].GetTexture());
                finalCombineEffect.SetParameter("halfPixel", calculateHalfPixelFromRT(luminanceChain[i - 1]));
                finalCombineEffect.RenderMultipass(fullScreenQuad.Draw);

            }

            // Final downscale            
            finalCombineEffect.SetTechnique("Downscale4Luminance");
            GraphicsDevice.SetRenderTarget(0, currentFrameLuminance);
            finalCombineEffect.SetParameter("pointTex0", luminanceChain[luminanceChain.Length - 1].GetTexture());
            finalCombineEffect.SetParameter("halfPixel", calculateHalfPixelFromRT(luminanceChain[luminanceChain.Length - 1]));

            finalCombineEffect.RenderMultipass(fullScreenQuad.Draw);

            GraphicsDevice.SetRenderTarget(0, null);

            // Adapt the luminance, to simulate slowly adjust exposure
            /*HDREffect.Parameters["g_fDT"].SetValue(dt);
            HDREffect.CurrentTechnique = HDREffect.Techniques["CalcAdaptedLuminance"];
            RenderTarget2D[] sources = new RenderTarget2D[2];
            sources[0] = currentFrameLuminance;
            sources[1] = lastFrameAdaptedLuminance;
            PostProcess(sources, currentFrameAdaptedLuminance, HDREffect);*/

            //if (OutputMode == DeferredOutputMode.Downsample)
            {
                ((XNAGame)game).SpriteBatch.Begin(SpriteBlendMode.None);
                int offset = 0;
                for (int i = 0; i < luminanceChain.Length; i++)
                {
                    var rt = luminanceChain[i];
                    ((XNAGame)game).SpriteBatch.Draw(rt.GetTexture(), new Vector2(offset, 0), Color.White);
                    offset += rt.Width;
                }

                ((XNAGame)game).SpriteBatch.Draw(currentFrameLuminance.GetTexture(), new Rectangle(0, luminanceChain[0].Height, 64, 64), Color.White);
                ((XNAGame)game).SpriteBatch.End();
            }

        }

        private Vector2 calculateHalfPixelFromRT(RenderTarget2D renderTarget)
        {
            return new Vector2(0.5f / renderTarget.Width, 0.5f / renderTarget.Height);
        }


        private void drawConePrimitives(int segments)
        {
            Vector3 forward = Vector3.Forward;
            Vector3 targetPlaneX = Vector3.Right;
            Vector3 targetPlaneY = Vector3.Up;

            var vertices = new TangentVertex[segments + 1];

            vertices[0] = new TangentVertex(Vector3.Zero, Vector2.Zero, -forward, Vector3.Zero);
            for (int i = 1; i < vertices.Length; i++)
            {
                float iAngle = MathHelper.TwoPi * (i - 1) / segments;
                var pos = forward + (float)Math.Cos(iAngle) * targetPlaneX + (float)Math.Sin(iAngle) * targetPlaneY;
                var normal = (float)-Math.Sin(iAngle) * targetPlaneX + (float)Math.Cos(iAngle) * targetPlaneY;
                vertices[i] = new TangentVertex(pos, Vector2.Zero, normal, Vector3.Zero);
            }

            int[] indices = new int[segments * 3 * 2];
            for (int i = 0; i < segments; i++)
            {
                indices[i * 3 + 0] = 0;
                indices[i * 3 + 1] = i + 1;
                indices[i * 3 + 2] = (i + 1) % segments + 1;

                indices[segments * 3 + i * 3 + 0] = 1;
                indices[segments * 3 + i * 3 + 1] = (i + 1) % segments + 1;
                indices[segments * 3 + i * 3 + 2] = i + 1;

            }


            GraphicsDevice.VertexDeclaration = tangentVertexDeclaration;


            GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices,
                                                     0, indices.Length / 3);
        }

        private void drawSpherePrimitives()
        {
            TangentVertex[] vertices;
            short[] sIndices;

            SphereMesh.CreateUnitSphereVerticesAndIndices(20, out vertices, out sIndices);
            int[] indices = new int[sIndices.Length];
            for (int i = 0; i < sIndices.Length; i++)
            {
                indices[i] = sIndices[i];
            }


            GraphicsDevice.VertexDeclaration = tangentVertexDeclaration;

            GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, sIndices,
                                                     0, sIndices.Length / 3);
        }



        private void ClearGBuffer()
        {
            clearShader.RenderMultipass(delegate
                                            {
                                                fullScreenQuad.DrawOld(GraphicsDevice);
                                            });
        }
        private void DrawDirectionalLight(Vector3 lightDirection, Color color)
        {
            //set all parameters
            directionalLightShader.Effect.Parameters["colorMap"].SetValue(diffuseRT.GetTexture());
            directionalLightShader.Effect.Parameters["normalMap"].SetValue(normalRT.GetTexture());
            directionalLightShader.Effect.Parameters["depthMap"].SetValue(depthRT.GetTexture());
            directionalLightShader.Effect.Parameters["lightDirection"].SetValue(lightDirection);
            directionalLightShader.Effect.Parameters["Color"].SetValue(color.ToVector3());
            directionalLightShader.Effect.Parameters["cameraPosition"].SetValue(game.Camera.ViewInverse.Translation);
            directionalLightShader.Effect.Parameters["InvertViewProjection"].SetValue(Matrix.Invert(game.Camera.ViewProjection));
            directionalLightShader.Effect.Parameters["halfPixel"].SetValue(halfPixel);
            directionalLightShader.Effect.Begin();
            directionalLightShader.Effect.Techniques[0].Passes[0].Begin();
            //draw a full-screen quad
            fullScreenQuad.DrawOld(GraphicsDevice);
            directionalLightShader.Effect.Techniques[0].Passes[0].End();
            directionalLightShader.Effect.End();
        }
        private void DrawPointLight(Vector3 lightPosition, Color color, float lightRadius, float lightIntensity)
        {
            //set the G-Buffer parameters
            pointLightShader.Effect.Parameters["colorMap"].SetValue(diffuseRT.GetTexture());
            pointLightShader.Effect.Parameters["normalMap"].SetValue(normalRT.GetTexture());
            pointLightShader.Effect.Parameters["depthMap"].SetValue(depthRT.GetTexture());
            //compute the light world matrix
            //scale according to light radius, and translate it to light position
            Matrix sphereWorldMatrix = Matrix.CreateScale(lightRadius) * Matrix.CreateTranslation(lightPosition);
            pointLightShader.Effect.Parameters["World"].SetValue(sphereWorldMatrix);
            pointLightShader.Effect.Parameters["View"].SetValue(game.Camera.View);
            pointLightShader.Effect.Parameters["Projection"].SetValue(game.Camera.Projection);
            //light position
            pointLightShader.Effect.Parameters["lightPosition"].SetValue(lightPosition);
            //set the color, radius and Intensity
            pointLightShader.Effect.Parameters["Color"].SetValue(color.ToVector3());
            pointLightShader.Effect.Parameters["lightRadius"].SetValue(lightRadius);
            pointLightShader.Effect.Parameters["lightIntensity"].SetValue(lightIntensity);
            //parameters for specular computations
            pointLightShader.Effect.Parameters["cameraPosition"].SetValue(game.Camera.ViewInverse.Translation);
            pointLightShader.Effect.Parameters["InvertViewProjection"].SetValue(Matrix.Invert(game.Camera.View * game.Camera.Projection));
            //size of a halfpixel, for texture coordinates alignment
            pointLightShader.Effect.Parameters["halfPixel"].SetValue(halfPixel);

            //calculate the distance between the camera and light center
            float cameraToCenter = Vector3.Distance(game.Camera.ViewInverse.Translation, lightPosition);
            //if we are inside the light volume, draw the sphere's inside face
            if (cameraToCenter < lightRadius)
                GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
            else
                GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

            pointLightShader.Effect.Begin();
            pointLightShader.Effect.Techniques[0].Passes[0].Begin();
            drawSpherePrimitives();
            pointLightShader.Effect.Techniques[0].Passes[0].End();
            pointLightShader.Effect.End();
            GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

        }
        private void DrawSpotLight(Vector3 lightPosition, Color color, float lightRadius, float lightIntensity, Vector3 spotDirection, float spotLightAngle, float spotDecayExponent)
        {

            float spotLightAngleCosine = (float)Math.Cos(spotLightAngle);
            Vector3 forward = Vector3.Forward;
            Vector3 targetPlaneX = Vector3.Right;
            Vector3 targetPlaneY = Vector3.Up;

            var world = Matrix.Identity;

            world *= Matrix.CreateScale(new Vector3(lightRadius, lightRadius, lightRadius));
            world *= Matrix.CreateScale((float)Math.Tan(spotLightAngle) * (targetPlaneX + targetPlaneY) + forward);
            Vector3 up = Vector3.Up;
            if (Math.Abs(Vector3.Dot(up, spotDirection)) > 0.999)
                up = Vector3.Right;

            world *= Matrix.CreateWorld(Vector3.Zero, -spotDirection, up);
            world *= Matrix.CreateTranslation(lightPosition);


            clearRenderStatesLightAccumulation();

            var LightViewProjection = drawSpotLightShadowMap(lightPosition, color, lightRadius, lightIntensity, spotDirection, spotLightAngle, spotDecayExponent, world);

            setRenderStatesLightAccumulation();

            if (OutputMode == DeferredOutputMode.ShadowMap)
                return;


            //set the G-Buffer parameters
            spotLightShader.Effect.Parameters["colorMap"].SetValue(diffuseRT.GetTexture());
            spotLightShader.Effect.Parameters["normalMap"].SetValue(normalRT.GetTexture());
            spotLightShader.Effect.Parameters["depthMap"].SetValue(depthRT.GetTexture());
            spotLightShader.Effect.Parameters["shadowMap"].SetValue(shadowMapRT.GetTexture());
            //compute the light world matrix
            //scale according to light radius, and translate it to light position

            spotLightShader.Effect.Parameters["World"].SetValue(world);
            spotLightShader.Effect.Parameters["View"].SetValue(game.Camera.View);
            spotLightShader.Effect.Parameters["Projection"].SetValue(game.Camera.Projection);



            spotLightShader.Effect.Parameters["LightViewProjection"].SetValue(LightViewProjection);

            //light position
            spotLightShader.Effect.Parameters["lightPosition"].SetValue(lightPosition);
            //set the color, radius and Intensity
            spotLightShader.Effect.Parameters["Color"].SetValue(color.ToVector3());
            spotLightShader.Effect.Parameters["lightRadius"].SetValue(lightRadius);
            spotLightShader.Effect.Parameters["lightIntensity"].SetValue(lightIntensity);
            spotLightShader.Effect.Parameters["spotDirection"].SetValue(spotDirection);
            spotLightShader.Effect.Parameters["spotLightAngleCosine"].SetValue(spotLightAngleCosine);
            spotLightShader.Effect.Parameters["spotDecayExponent"].SetValue(spotDecayExponent);
            //parameters for specular computations
            spotLightShader.Effect.Parameters["cameraPosition"].SetValue(game.Camera.ViewInverse.Translation);
            spotLightShader.Effect.Parameters["InvertViewProjection"].SetValue(Matrix.Invert(game.Camera.View * game.Camera.Projection));
            //size of a halfpixel, for texture coordinates alignment
            spotLightShader.Effect.Parameters["halfPixel"].SetValue(halfPixel);
            spotLightShader.Effect.Parameters["g_vShadowMapSize"].SetValue(new Vector2(shadowMapSize, shadowMapSize));

            //calculate the distance between the camera and light center
            float cameraToCenter = Vector3.Distance(game.Camera.ViewInverse.Translation, lightPosition);
            //if we are inside the light volume, draw the sphere's inside face
            if (cameraToCenter < lightRadius)
                GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
            else
                GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;



            spotLightShader.Effect.Parameters["World"].SetValue(Matrix.Identity);
            spotLightShader.Effect.Parameters["View"].SetValue(Matrix.Identity);
            spotLightShader.Effect.Parameters["Projection"].SetValue(Matrix.Identity);
            GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

            spotLightShader.Effect.Begin();
            spotLightShader.Effect.Techniques[0].Passes[0].Begin();
            //drawConePrimitives(40);
            fullScreenQuad.DrawOld(GraphicsDevice);
            spotLightShader.Effect.Techniques[0].Passes[0].End();
            spotLightShader.Effect.End();
            GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

        }

        private Matrix drawSpotLightShadowMap(Vector3 lightPosition, Color color, float lightRadius, float lightIntensity,
                                            Vector3 spotDirection, float spotLightAngle, float spotDecayExponent, Matrix world)
        {
            Matrix view = Matrix.Invert(world);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1, 0.01f, 1);

            DepthStencilBuffer oldDS = GraphicsDevice.DepthStencilBuffer;

            if (OutputMode != DeferredOutputMode.ShadowMap)
            {
                GraphicsDevice.SetRenderTarget(0, shadowMapRT);
                GraphicsDevice.DepthStencilBuffer = shadowMapDS;
            }

            GraphicsDevice.Clear(Color.White);
            GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1, 0);

            shadowMapShader.Effect.Parameters["g_matViewProj"].SetValue(view * projection);
            shadowMapShader.SetTechnique("GenerateShadowMap");
            DrawSceneShadowMap();

            if (OutputMode != DeferredOutputMode.ShadowMap)
            {
                if (OutputMode != DeferredOutputMode.LightAccumulation)
                    GraphicsDevice.SetRenderTarget(0, lightRT);
                else
                    GraphicsDevice.SetRenderTarget(0, null);
                GraphicsDevice.DepthStencilBuffer = oldDS;
            }
            game.LineManager3D.AddViewFrustum(new BoundingFrustum(view * projection), Color.Red);

            return view * projection;
        }

        private void DrawAmbientOcclusion()
        {
            if (OutputMode != DeferredOutputMode.AmbientOcclusion)
                GraphicsDevice.SetRenderTarget(0, ambientOcclusionRT);



            //set all parameters
            ambientOcclusionShader.Effect.Parameters["rnm"].SetValue(randomNormalsTexture);
            ambientOcclusionShader.Effect.Parameters["normalMap"].SetValue(normalRT.GetTexture());
            ambientOcclusionShader.Effect.Parameters["depthMap"].SetValue(depthRT.GetTexture());

            ambientOcclusionShader.Effect.Parameters["halfPixel"].SetValue(halfPixel);

            ambientOcclusionShader.Effect.Begin();
            ambientOcclusionShader.Effect.Techniques[0].Passes[0].Begin();
            //draw a full-screen quad
            fullScreenQuad.DrawOld(GraphicsDevice);
            ambientOcclusionShader.Effect.Techniques[0].Passes[0].End();
            ambientOcclusionShader.Effect.End();


            if (OutputMode != DeferredOutputMode.AmbientOcclusion)
                GraphicsDevice.SetRenderTarget(0, null);
        }

        private void BlurAmbientOcclusion()
        {
            if (OutputMode != DeferredOutputMode.BlurX)
                GraphicsDevice.SetRenderTarget(0, blurRT);
            blurShader.SetParameter("blurMap", ambientOcclusionRT.GetTexture());
            blurShader.SetParameter("BlurOffset", new Vector2(halfPixel.X, 0));
            blurShader.SetParameter("halfPixel", halfPixel);
            blurShader.SetTechnique("GaussionBlur");

            blurShader.RenderMultipass(fullScreenQuad.Draw);

            if (OutputMode == DeferredOutputMode.BlurX) return;
            GraphicsDevice.SetRenderTarget(0, null);

            blurShader.SetParameter("blurMap", blurRT.GetTexture());
            blurShader.SetParameter("BlurOffset", new Vector2(0, halfPixel.Y));
            blurShader.SetParameter("halfPixel", halfPixel);
            blurShader.SetTechnique("GaussionBlur");

            blurShader.RenderMultipass(fullScreenQuad.Draw);
        }

        public void Update(IXNAGame _game)
        {
            totalSeconds += game.Elapsed;


            if (game.Keyboard.IsKeyDown(Keys.Up))
                tempMovePos += Vector3.Up * game.Elapsed;
            if (game.Keyboard.IsKeyDown(Keys.Down))
                tempMovePos += -Vector3.Up * game.Elapsed;
        }

        private void SetGBuffer()
        {
            GraphicsDevice.SetRenderTarget(0, diffuseRT);
            GraphicsDevice.SetRenderTarget(1, normalRT);
            GraphicsDevice.SetRenderTarget(2, depthRT);
        }
        private void ResolveGBuffer()
        {
            GraphicsDevice.SetRenderTarget(0, null);
            GraphicsDevice.SetRenderTarget(1, null);
            GraphicsDevice.SetRenderTarget(2, null);
        }
    }
}
