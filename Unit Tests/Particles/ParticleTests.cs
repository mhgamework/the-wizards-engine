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
            data.DiskFilePath = TWDir.GameData.CreateSubdirectory("Core").FullName+ "\\fire.png";
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
            emit = new Emitter(texPool, pool, game, testTexture, 2f, 2f,creater);
            Seeder seed = new Seeder(54);
            game.InitializeEvent += delegate
            {
                texPool.Initialize(game);
                pool.Initialize(game);

                emit.Initialize();
                emit.InitializeRender();


                emit.CreateRenderData();
                emit.SetRenderData();
                //emit.AddParticles(creater,1);
            };
            float  dist = 0;
            game.UpdateEvent += delegate
            {
                // emit.setShader();  
                emit.Update();
                if (dist >500 )
                {
                    dist = 0;
                }
                else
                {
                    dist +=game.Elapsed*5;
                }
                //emit.SetPosition(new Vector3(dist,0,0));
            };
            game.DrawEvent += delegate
            {
                //game.GraphicsDevice.Clear(Color.Black);
                game.GraphicsDevice.RenderState.CullMode = CullMode.None;
                emit.Render(game.SpectaterCamera.ViewProjection, game.SpectaterCamera.ViewInverse);
            };

            game.Run();
        }
         [Test]
        public void FlameTest()
        {
            Emitter emit;
            XNAGame game = new XNAGame();
            FlameParticelCreater creater;
            game.DrawFps = true;

            var pool = new VertexDeclarationPool();
            pool.SetVertexElements<Emitter.ParticleVertex>(Emitter.ParticleVertex.VertexElements);
            var texPool = new TexturePool();
            var testTexture = GetTestTexture();
            creater = new FlameParticelCreater();
            emit = new Emitter(texPool, pool, game, testTexture, 2f, 2f,creater);
            Seeder seed = new Seeder(54);
            game.InitializeEvent += delegate
            {
                texPool.Initialize(game);
                pool.Initialize(game);

                emit.Initialize();
                emit.InitializeRender();


                emit.CreateRenderData();
                emit.SetRenderData();
                //emit.AddParticles(creater,1);
            };
            float  dist = 0;
            game.UpdateEvent += delegate
            {
                // emit.setShader();  
                emit.Update();
               
            };
            game.DrawEvent += delegate
            {
                //game.GraphicsDevice.Clear(Color.Black);
                game.GraphicsDevice.RenderState.CullMode = CullMode.None;
                emit.Render(game.SpectaterCamera.ViewProjection, game.SpectaterCamera.ViewInverse);
            };

            game.Run();
        }
    
    }
}
