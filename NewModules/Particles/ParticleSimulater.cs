using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Particles
{
    public class ParticleSimulater
    {
        private RenderTarget2D target;
        private FullScreenQuad quad;
        private Texture2D positionTex;
        private Texture2D position2Tex;

        private Texture2D velocityTex;
        private Texture2D velocity2Tex;

        private Texture2D staticInfo;
        private BasicShader shader;
        private IXNAGame game;
        private readonly int size;
        private int vertexStride=TangentVertex.SizeInBytes;
        private bool fase = true;
        public ParticleSimulater(IXNAGame game,int size)
        {
            this.game = game;
            this.size = size;
            positionTex = new Texture2D(game.GraphicsDevice, size, size, 0, TextureUsage.None, SurfaceFormat.HalfVector4);
            position2Tex = new Texture2D(game.GraphicsDevice, size, size, 0, TextureUsage.None, SurfaceFormat.HalfVector4);
            velocityTex = new Texture2D(game.GraphicsDevice, size, size, 0, TextureUsage.None, SurfaceFormat.HalfVector4);
            velocity2Tex = new Texture2D(game.GraphicsDevice, size, size, 0, TextureUsage.None, SurfaceFormat.HalfVector4);

        }

        public void Initialize()
        {
            quad = new FullScreenQuad(game.GraphicsDevice);
           
        }
        private  Texture2D getOldPosition()
        {
            if(fase)
            {
                return positionTex;
            }else
            {
                return position2Tex;
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
                return velocityTex;
            }
            else
            {
                return velocity2Tex;
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
        private void SwitchTextures()
        {
             fase = !fase;
        }
        public void AddNewParticle(Vector3 position, Vector3 velocity,int index)
        {
            Vector4[] vec = new Vector4[1];
            vec[0] = new Vector4(position, 1);
            getOldPosition().SetData<Vector4>(vec, index, 1, SetDataOptions.None);

            vec[0] = new Vector4(velocity, 1);
            getOldPosition().SetData<Vector4>(vec, index, 1, SetDataOptions.None);
        }
       
        public void RenderUpdate(float elapsed)
        {
                shader.SetParameter("elapsed", elapsed);
                shader.SetParameter("OldPostion", getOldPosition());
                shader.SetParameter("OldVelocity", getOldVelocity());

            Viewport oldPort = game.GraphicsDevice.Viewport;
            var newPort = new Viewport();
            newPort.
            game.GraphicsDevice.Viewport.Height = size;
            game.GraphicsDevice.Viewport.Width = size;
            game.GraphicsDevice.SetRenderTarget(0, target);
            game.GraphicsDevice.SetRenderTarget(1, target);
            shader.RenderMultipass(quad.Draw);
            getNewPosition() = target.GetTexture();


        }
        
       
    }
}
