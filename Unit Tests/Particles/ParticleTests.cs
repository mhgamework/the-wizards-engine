﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Particles;
using MHGameWork.TheWizards.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NUnit.Framework;

namespace MHGameWork.TheWizards.Tests.Particles
{
    [TestFixture]
    public class ParticleTests
    {
        public static RAMTexture GetTestTexture()
        {
            var tex = new RAMTexture();

            var data = tex.GetCoreData();
            data.StorageType = TextureCoreData.TextureStorageType.Disk;
            data.DiskFilePath = TWDir.GameData.CreateSubdirectory("Core").FullName+ "\\explosion.png";
            /*data.StorageType = TextureCoreData.TextureStorageType.Assembly;
            data.Assembly = Assembly.GetExecutingAssembly();
            data.AssemblyResourceName = "MHGameWork.TheWizards.Tests.OBJParser.Files.maps.BrickRound0030_7_S.jpg";*/
            return tex;
        }
        [Test]
        public void BasicRenderingTest()
        {
            Emitter emit;
            XNAGame game = new XNAGame();

            game.DrawFps = true;

            var pool = new VertexDeclarationPool();
            pool.SetVertexElements<Emitter.ParticleVertex>(Emitter.ParticleVertex.VertexElements);
            var texPool = new TexturePool();
            var testTexture = GetTestTexture();
            SimpleParticleCreater creater=new SimpleParticleCreater();
            emit = new Emitter(texPool, pool, game, testTexture,5,5,creater);
            Seeder seed = new Seeder(54);
            game.InitializeEvent += delegate
                                        {
                                            texPool.Initialize(game);
                                            pool.Initialize(game);


                                            emit.Initialize();
                                            emit.InitializeRender();
                                            
                                            
                                            emit.CreateRenderData();
                                            emit.SetRenderData();
                                            emit.AddParticles(creater,1);
                                        };
            game.UpdateEvent += delegate
                                    {
                                       // emit.setShader();
                                        //emit.SetPosition(seed.NextVector3(new Vector3(-50,70,-50),new Vector3(50,70,50)));
                                        emit.TestUpdate();
                                    };
            game.DrawEvent += delegate
                                  {
                                      game.GraphicsDevice.RenderState.CullMode = CullMode.None;  
                                      emit.Render(game.SpectaterCamera.ViewProjection, game.SpectaterCamera.ViewInverse);
                                  };

            game.Run();
        }
        [Test]
        public void BasicBalTest()
        {
            Emitter emit;
            XNAGame game = new XNAGame();
            BallParticleCreater creater;
            game.DrawFps = true;

            var pool = new VertexDeclarationPool();
            pool.SetVertexElements<Emitter.ParticleVertex>(Emitter.ParticleVertex.VertexElements);
            var texPool = new TexturePool();
            var testTexture = GetTestTexture();
            creater = new BallParticleCreater();
            emit = new Emitter(texPool, pool, game, testTexture, 0.5f, 0.5f,creater);
            Seeder seed = new Seeder(54);
            game.InitializeEvent += delegate
            {
                texPool.Initialize(game);
                pool.Initialize(game);

                emit.Initialize();
                emit.InitializeRender();


                emit.CreateRenderData();
                emit.SetRenderData();
                emit.AddParticles(creater,1);
            };
            double angle = 0;
            game.UpdateEvent += delegate
            {
                // emit.setShader();  
                emit.Update();
                if (angle > MathHelper.TwoPi)
                {
                    angle = 0;
                }
                else
                {
                    angle +=game.Elapsed;
                }
                emit.SetPosition(new Vector3(5 * (float)Math.Cos(angle), 0, 5 *(float) Math.Sin(angle)));
            };
            game.DrawEvent += delegate
            {
                game.GraphicsDevice.RenderState.CullMode = CullMode.None;
                emit.Render(game.SpectaterCamera.ViewProjection, game.SpectaterCamera.ViewInverse);
            };

            game.Run();
        }
    }
}
