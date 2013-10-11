using System;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Particles;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using SlimDX;
using MathHelper = DirectX11.MathHelper;
using TexturePool = MHGameWork.TheWizards.Rendering.Deferred.TexturePool;
using Vector3 = SlimDX.Vector3;

namespace MHGameWork.TheWizards.SkyMerchant._Tests.Development
{
    [TestFixture]
    public class CloudsTest
    {
        private DX11Game game;

        public CloudsTest()
        {
            game = new DX11Game();
        }


        public static Curve3D CreateTestCurve()
        {
            Curve3D curve = new Curve3D();

            curve.PreLoop = CurveLoopType.Constant;
            curve.PostLoop = CurveLoopType.Cycle;

            curve.AddKey(0, new Microsoft.Xna.Framework.Vector3(2, 2, 2));
            curve.AddKey(1, new Microsoft.Xna.Framework.Vector3(4, 0, 2));
            curve.AddKey(5, new Microsoft.Xna.Framework.Vector3(8, 0, 7));
            curve.AddKey(7, new Microsoft.Xna.Framework.Vector3(2, 1, 5));
            curve.AddKey(9, new Microsoft.Xna.Framework.Vector3(2, 2, 2));

            curve.SetTangents();
            return curve;
        }

        [Test]
        public void SingleCloudTest()
        {
            Emitter emit;
            DX11Game game = new DX11Game();
            game.InitDirectX();


            var texPool = new TexturePool(game);
            var testTexture = getCloudTexture();
            var creater = new CloudCreater();
            //SimpleParticleCreater creater = new SimpleParticleCreater();
            EmitterParameters param = new EmitterParameters();
            param.size = 512;
            param.texture = testTexture;
            param.particleCreater = creater;
            param.AutoSpawnParticles = false;
            param.MaxLifeTime = 2000;
            param.startColor = new Color4(1, 1, 1);
            param.endColor = new Color4(1, 1, 1);
            param.particleHeight = 10;
            param.particleHeightEnd = 10;
            param.particleWidth = 10;
            param.particleWidthEnd = 10;
            param.EffectName = "calculateNone";
            emit = new Emitter(texPool, game, param, 800, 600);//note: again screen size
            //game.Wpf.CreateClassForm(param);
            Seeder seed = new Seeder(54);
            var curve = CreateTestCurve();

            //emit.ParticlesPerSecond = 0;
            

            emit.Initialize();
            emit.InitializeRender();


            emit.CreateRenderData();
            emit.SetRenderData();
            emit.SetPosition(Vector3.Zero);
            float dist = 0;

            emit.AddParticles(creater, 10000);
            
            
            game.GameLoopEvent += delegate
            {
                // emit.setShader();  
                emit.Update();
                if (dist > 100)
                {
                    dist = 0;
                }
                else
                {
                    dist += game.Elapsed * 1;
                }
                //emit.SetPosition(new Vector3(dist, 0, 0));
                //Temp(dist, emit, curve);

                //Draw part

                //game.GraphicsDevice.Clear(Color.Black);
                game.Device.ImmediateContext.Rasterizer.State = game.HelperStates.RasterizerShowAll;
                emit.Render(game.SpectaterCamera.ViewProjection, game.SpectaterCamera.ViewInverse);

            };




            game.Run();
        }


        private ITexture getCloudTexture()
        {
            var ret= new RAMTexture();
            ret.GetCoreData().DiskFilePath = getCloudTexturePath();
            ret.GetCoreData().StorageType = TextureCoreData.TextureStorageType.Disk;
            return ret;
        }
        private string getCloudTexturePath()
        {
            return TWDir.GameData + @"\SkyMerchant\Clouds\cloudTexture.png";
        }

        private class CloudCreater : IParticleCreater
        {
            private Seeder random = new Seeder(0);
            public void GetNewParticleData(out Vector3 position, out Vector3 velocity)
            {
                position = random.NextVector3(-MathHelper.One.xna(), MathHelper.One.xna()).dx() * 200;
                velocity = new Vector3();
            }
        }
    }
}