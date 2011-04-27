using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;

namespace MHGameWork.TheWizards.Particles
{
    public class ParticleSimulater
    {
        private RenderTarget2D positionTarget;
        private RenderTarget2D positionTarget2;
        private RenderTarget2D velocityTarget;
        private RenderTarget2D velocityTarget2;


        private FullScreenQuad quad;
        Viewport newPort = new Viewport();

        private Texture2D positionTex;
        private Texture2D position2Tex;

        private Texture2D velocityTex;
        private Texture2D velocity2Tex;

        private Texture2D staticInfo;
        private BasicShader shader;
        private IXNAGame game;
        private readonly int size;
        private int vertexStride = TangentVertex.SizeInBytes;
        private bool fase = true;
        public ParticleSimulater(IXNAGame game, int size)
        {
            this.game = game;
            this.size = size;
            positionTarget = new RenderTarget2D(game.GraphicsDevice, size, size, 0, SurfaceFormat.HalfVector4);
            positionTarget2 = new RenderTarget2D(game.GraphicsDevice, size, size, 0, SurfaceFormat.HalfVector4);
            velocityTarget = new RenderTarget2D(game.GraphicsDevice, size, size, 0, SurfaceFormat.HalfVector4);
            velocityTarget2 = new RenderTarget2D(game.GraphicsDevice, size, size, 0, SurfaceFormat.HalfVector4);



            // positionTex = positionTarget.GetTexture();
            // position2Tex = positionTarget2.GetTexture();
            // velocityTex = velocityTarget.GetTexture();
            // velocity2Tex = velocityTarget2.GetTexture();

            //don't know what the best settings are for the viewport
            newPort.Height = size;
            newPort.Width = size;

        }

        public void Initialize()
        {
            quad = new FullScreenQuad(game.GraphicsDevice);
            shader = BasicShader.LoadFromEmbeddedFile(game, Assembly.GetExecutingAssembly(), "MHGameWork.TheWizards.Particles.Files.BasicParticleAnimation.fx", "..\\..\\NewModules\\Particles\\Files\\BasicParticleAnimation.fx", new EffectPool());
            shader.SetTechnique("particleSimulation");
            shader.SetParameter("size", size);

            clearRenderTarget(positionTarget);
            clearRenderTarget(positionTarget2);
            clearRenderTarget(velocityTarget);
            clearRenderTarget(velocityTarget2);
        }
        private void clearRenderTarget(RenderTarget2D target)
        {
            game.GraphicsDevice.SetRenderTarget(0, target);
            game.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 1.0f, 0);
            game.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
            game.GraphicsDevice.SetRenderTarget(0, null);
        }
        public Texture2D getOldPosition()
        {
            if (fase)
            {
                return positionTarget.GetTexture();
            }
            else
            {
                return positionTarget2.GetTexture(); ;
            }

        }
        private Texture2D getNewPosition()
        {
            if (fase)
            {
                return position2Tex;
            }
            else
            {
                return positionTex;
            }

        }
        private Texture2D getOldVelocity()
        {
            if (fase)
            {
                return velocityTarget.GetTexture();
            }
            else
            {
                return velocityTarget2.GetTexture();
            }

        }
        private Texture2D getNewVelocity()
        {
            if (fase)
            {
                return velocity2Tex;
            }
            else
            {
                return velocityTex;
            }

        }

        private RenderTarget2D getNewRenderTargetPosition()
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
        private RenderTarget2D getNewRenderTargetVelocity()
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
            HalfVector4[] vec = new HalfVector4[1];
            vec[0] = new HalfVector4(position.X, position.Y, position.Z, 1);
           getOldPosition().SetData<HalfVector4>(0, new Rectangle(index % size, (int)(index / size), 1, 1), vec, 0, 1, SetDataOptions.None);

            vec[0] = new HalfVector4(velocity.X, velocity.Y, velocity.Z, 0);
            getOldVelocity().SetData<HalfVector4>(0, new Rectangle(index % size, (int)(index / size), 1, 1), vec, 0, 1, SetDataOptions.None);
        }
        public void AddNewParticle(Vector4[] positions, Vector4[] velocities, int start)
        {

            getOldPosition().SetData<Vector4>(positions, start, positions.Length, SetDataOptions.None);

            getOldPosition().SetData<Vector4>(velocities, start, velocities.Length, SetDataOptions.None);
        }

        public void RenderUpdate(float elapsed)
        {
            shader.SetParameter("elapsed", elapsed);
            shader.SetParameter("oldPosition", getOldPosition());
            shader.SetParameter("oldVelocity", getOldVelocity());
           
            Viewport oldPort = game.GraphicsDevice.Viewport;
            game.GraphicsDevice.Viewport = newPort;


            game.GraphicsDevice.SetRenderTarget(0, getNewRenderTargetPosition());
            game.GraphicsDevice.SetRenderTarget(1, getNewRenderTargetVelocity());
            shader.RenderMultipass(quad.Draw);
            game.GraphicsDevice.SetRenderTarget(0, null);
            game.GraphicsDevice.SetRenderTarget(1, null);
            shader.SetParameter("oldPosition", (Texture2D)null);
            shader.SetParameter("oldVelocity", (Texture2D)null);
            game.GraphicsDevice.Viewport = oldPort;

             SwitchTextures();
            var g = (XNAGame)game;
            
            g.SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.SaveState);
            g.SpriteBatch.Draw(getOldPosition(), Vector2.Zero, Color.White);
            g.SpriteBatch.Draw(getOldVelocity(), new Vector2(150, 0), Color.White);
            g.SpriteBatch.End();

           


        }


    }
}
