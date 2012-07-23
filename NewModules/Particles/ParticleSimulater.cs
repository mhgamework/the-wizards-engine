using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using MapFlags = SlimDX.DXGI.MapFlags;


namespace MHGameWork.TheWizards.Particles
{
    public class ParticleSimulater
    {
        private RenderTargetView positionTarget;
        private RenderTargetView positionTarget2;
        private RenderTargetView velocityTarget;
        private RenderTargetView velocityTarget2;


        private DirectX11.Graphics.FullScreenQuad quad;
        Viewport newPort = new Viewport();

        private Texture2D positionTex;
        private Texture2D position2Tex;
        private Texture2D velocityTex;
        private Texture2D velocity2Tex;



        private Texture2D staticInfo;
        private BasicShader shader;
        private DX11Game game;
        private readonly int size;
        private readonly string simulation;
       
        private bool fase = true;
        private DeviceContext context;
        private ShaderResourceView positionRv;
        private ShaderResourceView position2Rv;
        private ShaderResourceView velocityRv;
        private ShaderResourceView velocity2Rv;
        private InputLayout layout;

        public ParticleSimulater(DX11Game game, int size, string simulation)
        {
            this.game = game;
            this.size = size;
            this.simulation = simulation;
            context = game.Device.ImmediateContext;

            // positionTex = positionTarget.GetTexture();
            // position2Tex = positionTarget2.GetTexture();
            // velocityTex = velocityTarget.GetTexture();
            // velocity2Tex = velocityTarget2.GetTexture();

            //don't know what the best settings are for the viewport
            newPort.Height = size;
            newPort.Width = size;

        }
        private Stream generateIncludeCallback()
        {
            var code = "float3 calculateAcceleration(float3 oldVelocity, float3 oldPosition){return " + simulation + "(oldVelocity,oldPosition); }";

            var byteArray = Encoding.ASCII.GetBytes(code);
            return new MemoryStream(byteArray);
        }
        public void Initialize()
        {
            //quad = new FullScreenQuad(game.GraphicsDevice);
            quad = new FullScreenQuad(game.Device);
            shader = BasicShader.LoadAutoreload(game,
                                                new System.IO.FileInfo(
                                                    CompiledShaderCache.Current.RootShaderPath + "Particles\\BasicParticleAnimation.fx"), null, new[] { new ShaderMacro("CALCULATE_ACCELERATION",simulation) });
            shader.SetTechnique("particleSimulation");
           // shader.AddCustomIncludeHandler("generated.fx",generateIncludeCallback);
            //shader.InitFromEmbeddedFile(game, Assembly.GetExecutingAssembly(), "MHGameWork.TheWizards.Particles.Files.BasicParticleAnimation.fx", "..\\..\\NewModules\\Particles\\Files\\BasicParticleAnimation.fx", new EffectPool());

            layout = FullScreenQuad.CreateInputLayout(game.Device, shader.GetCurrentPass(0));

            positionTex = new Texture2D(game.Device, new Texture2DDescription()
                                                         {
                                                             ArraySize = 1,
                                                             CpuAccessFlags = CpuAccessFlags.None,
                                                             BindFlags =
                                                                 BindFlags.RenderTarget |BindFlags.ShaderResource,
                                                             Format = Format.R16G16B16A16_Float,
                Height = size,
                Width = size,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription =
                    new SlimDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default //TODO: render the new position to the texture


            });
            position2Tex = new Texture2D(game.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                CpuAccessFlags = CpuAccessFlags.None,
                BindFlags =
                    BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.R16G16B16A16_Float,
                Height = size,
                Width = size,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription =
                    new SlimDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default


            });
            velocityTex = new Texture2D(game.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                CpuAccessFlags = CpuAccessFlags.None,
                BindFlags =
                    BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.R16G16B16A16_Float,
                Height = size,
                Width = size,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription =
                    new SlimDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default


            });
            velocity2Tex = new Texture2D(game.Device, new Texture2DDescription()
            {
                ArraySize = 1,
                CpuAccessFlags = CpuAccessFlags.None,
                BindFlags =
                    BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.R16G16B16A16_Float,
                Height = size,
                Width = size,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription =
                    new SlimDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default


            });
            positionRv = new ShaderResourceView(game.Device, positionTex);
            position2Rv = new ShaderResourceView(game.Device, position2Tex);
            velocityRv = new ShaderResourceView(game.Device, velocityTex);
            velocity2Rv = new ShaderResourceView(game.Device, velocity2Tex);

            positionTarget = new RenderTargetView(game.Device, positionTex);
            positionTarget2 = new RenderTargetView(game.Device, position2Tex);
            velocityTarget = new RenderTargetView(game.Device, velocityTex);
            velocityTarget2 = new RenderTargetView(game.Device, velocity2Tex);

            
        }
        private void clearRenderTarget(RenderTargetView target)
        {

            context.ClearRenderTargetView(target, new Color4(1, 0, 0, 0));
            /*game.GraphicsDevice.SetRenderTarget(0, target);
            game.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 1.0f, 0);
            game.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
            game.GraphicsDevice.SetRenderTarget(0, null);*/
        }
        public RenderTargetView getOldPosition()
        {
            
            if (fase)
            {
                return positionTarget;
            }
            else
            {
                return positionTarget2; ;
            }

        }
        private RenderTargetView getNewPosition()
        {
            if (fase)
            {
                return positionTarget2;
            }
            else
            {
                return positionTarget;
            }

        }
        private RenderTargetView getOldVelocity()
        {
            if (fase)
            {
                return velocityTarget;
            }
            else
            {
                return velocityTarget2;
            }

        }
        private RenderTargetView getNewVelocity()
        {
            if (fase)
            {
                return velocityTarget2;
            }
            else
            {
                return velocityTarget;
            }

        }


        public ShaderResourceView getOldPositionSRV()
        {

            if (fase)
            {
                return positionRv;
            }
            else
            {
                return position2Rv; ;
            }

        }
        private ShaderResourceView getNewPositionSRV()
        {
            if (fase)
            {
                return position2Rv;
            }
            else
            {
                return positionRv;
            }

        }
        private ShaderResourceView getOldVelocitySRV()
        {
            if (fase)
            {
                return velocityRv;
            }
            else
            {
                return velocity2Rv;
            }

        }
        private ShaderResourceView getNewVelocitySRV()
        {
            if (fase)
            {
                return velocity2Rv;
            }
            else
            {
                return velocityRv;
            }

        }
        private RenderTargetView getNewRenderTargetPosition()
        {
            if (fase)
            {
                return positionTarget2;
            }
            else
            {
                return positionTarget;
            }
        }
        private RenderTargetView getNewRenderTargetVelocity()
        {
            if (fase)
            {
                return velocityTarget2;
            }
            else
            {
                return velocityTarget;
            }
        }
        private void SwitchTextures()
        {
            fase = !fase;
        }


        public void AddNewParticle(Vector3 position, Vector3 velocity, int index)
        {
            /*HalfVector4[] vec = new HalfVector4[1];
            vec[0] = new HalfVector4(position.X, position.Y, position.Z, 1);
            getOldPosition().SetData<HalfVector4>(0, new Rectangle(index % size, (int)(index / size), 1, 1), vec, 0, 1, SetDataOptions.None);*/

           /* var boxp = context.MapSubresource(getOldPosition(), 0, size*size*3*2, MapMode.ReadWrite,
                                              SlimDX.Direct3D11.MapFlags.None);
            boxp.Data.Position = boxp.RowPitch * (int)(index / size) + index % size;
            boxp.Data.Write(position);

            context.UnmapSubresource(getOldPosition(), 0);*/
            context.OutputMerger.SetTargets(getOldPosition());
            context.Rasterizer.SetViewports(new Viewport(index%size, (int) (index/size), 1, 1));
            game.TextureRenderer.DrawColor(new Color4(position), Vector2.Zero, new Vector2(1,1));
            //context.ClearRenderTargetView(getOldPosition(), new Color4(position));
            

            //vec[0] = new HalfVector4(velocity.X, velocity.Y, velocity.Z, 0);
            //getOldVelocity().SetData<HalfVector4>(0, new Rectangle(index % size, (int)(index / size), 1, 1), vec, 0, 1, SetDataOptions.None);

           /* var boxv = context.MapSubresource(getOldVelocity(), 0, size * size * 3 * 2, MapMode.ReadWrite, SlimDX.Direct3D11.MapFlags.None);
            boxv.Data.Position = boxv.RowPitch * (int)(index / size) + index % size;
            boxv.Data.Write(position);*/
            context.OutputMerger.SetTargets(getOldVelocity());
            context.Rasterizer.SetViewports(new Viewport(index % size, (int)(index / size), 1, 1));
            game.TextureRenderer.DrawColor(new Color4(velocity), Vector2.Zero, new Vector2(1, 1));


            
        }

        /*public void AddNewParticle(Vector4[] positions, Vector4[] velocities, int start)
        {

            getOldPosition().SetData<Vector4>(positions, start, positions.Length, SetDataOptions.None);

            getOldPosition().SetData<Vector4>(velocities, start, velocities.Length, SetDataOptions.None);
        }*/

        public void RenderUpdate(float elapsed, Vector3 position)
        {
            context.ClearState();
            
            //game.GraphicsDevice.RenderState.AlphaBlendEnable = false;

            shader.SetTechnique("particleSimulation");
            shader.Effect.GetVariableByName("size").AsScalar().Set( size);
            shader.Effect.GetVariableByName("elapsed").AsScalar().Set(elapsed);
            shader.Effect.GetVariableByName("center").AsVector().Set( position);
            shader.Effect.GetVariableByName("oldPositionTex").AsResource().SetResource(getOldPositionSRV());
            shader.Effect.GetVariableByName("oldVelocityTex").AsResource().SetResource(getOldVelocitySRV());
            
            //Viewport oldPort = game.GraphicsDevice.Viewport;
            //game.GraphicsDevice.Viewport = newPort;
            context.Rasterizer.SetViewports(newPort,newPort);
            context.OutputMerger.SetTargets(getNewRenderTargetPosition(), getNewRenderTargetVelocity());
            //game.GraphicsDevice.SetRenderTarget(0, getNewRenderTargetPosition());
            //game.GraphicsDevice.SetRenderTarget(1, getNewRenderTargetVelocity());
            shader.Apply();
            quad.Draw(layout);

            context.OutputMerger.SetTargets(new RenderTargetView[2] { null, null });
            /*game.GraphicsDevice.SetRenderTarget(0, null);
            game.GraphicsDevice.SetRenderTarget(1, null);*/
            shader.Effect.GetVariableByName("oldPositionTex").AsResource().SetResource(null);
            shader.Effect.GetVariableByName("oldVelocityTex").AsResource().SetResource(null);
            shader.Apply();
            //game.GraphicsDevice.Viewport = oldPort;

            SwitchTextures();
            /*var g = (XNAGame)game;

            g.SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Texture, SaveStateMode.SaveState);
            g.SpriteBatch.Draw(getOldPosition(), Vector2.Zero, Color.White);
            g.SpriteBatch.Draw(getOldVelocity(), new Vector2(150, 0), Color.White);
            g.SpriteBatch.End();*/
            game.SetBackbuffer();
            game.TextureRenderer.Draw(getOldPositionSRV(), Vector2.Zero, new Vector2(150, 150));
            game.TextureRenderer.Draw(getOldVelocitySRV(), new Vector2(150, 0), new Vector2(150, 150));


        }



    }
}
