using System;
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
            data.DiskFilePath = TestFiles.BrickRoundJPG;
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
            pool.SetVertexElements<VertexPositionTexture>(VertexPositionTexture.VertexElements);
            var texPool = new TexturePool();
            var testTexture = GetTestTexture(); 

            emit = new Emitter(texPool, pool, game, testTexture,5,5);

            game.InitializeEvent += delegate
                                        {
                                            texPool.Initialize(game);
                                            pool.Initialize(game);


                                            emit.Initialize(5000);
                                          
                                            emit.InitializeRender();
                                            emit.AddParticle(new Vector3(0, 0, 0));
                                            
                                            emit.CreateRenderData();
                                            emit.SetRenderData();
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
