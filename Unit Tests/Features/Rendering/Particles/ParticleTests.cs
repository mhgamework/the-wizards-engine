using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Particles;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Tests.Features.Rendering.Graphics;
using NUnit.Framework;
using SlimDX;
using TexturePool = MHGameWork.TheWizards.Rendering.Deferred.TexturePool;
//using MHGameWork.TheWizards.Tests.Graphics;

namespace MHGameWork.TheWizards.Tests.Features.Rendering.Particles
{
    [TestFixture]
    public class ParticleTests
    {
        public static RAMTexture GetTestTexture()
        {
            var tex = new RAMTexture();

            var data = tex.GetCoreData();
            data.StorageType = TextureCoreData.TextureStorageType.Disk;
            data.DiskFilePath = TWDir.GameData.CreateSubdirectory("Core").FullName + "\\nova.png";
            /*data.StorageType = TextureCoreData.TextureStorageType.Assembly;
            data.Assembly = Assembly.GetExecutingAssembly();
            data.AssemblyResourceName = "MHGameWork.TheWizards.Tests.OBJParser.Files.maps.BrickRound0030_7_S.jpg";*/
            return tex;
        }
        //[Test]
        //public void BasicRenderingTest()
        //{
        //    Emitter emit;
        //    XNAGame game = new XNAGame();

        //    game.DrawFps = true;

        //    var pool = new VertexDeclarationPool();
        //    pool.SetVertexElements<Emitter.ParticleVertex>(Emitter.ParticleVertex.VertexElements);
        //    var texPool = new TexturePool();
        //    var testTexture = GetTestTexture();
        //    SimpleParticleCreater creater = new SimpleParticleCreater();
        //    EmitterParameters param = new EmitterParameters();
        //    param.texture = testTexture;
        //    param.particleCreater = creater;
        //    emit = new Emitter(texPool, pool, game, param);
        //    Seeder seed = new Seeder(54);
        //    game.InitializeEvent += delegate
        //                                {
        //                                    texPool.Initialize(game);
        //                                    pool.Initialize(game);


        //                                    emit.Initialize();
        //                                    emit.InitializeRender();


        //                                    emit.CreateRenderData();
        //                                    emit.SetRenderData();
        //                                    emit.AddParticles(creater, 1);
        //                                };
        //    game.UpdateEvent += delegate
        //                            {
        //                                // emit.setShader();
        //                                //emit.SetPosition(seed.NextVector3(new Vector3(-50,70,-50),new Vector3(50,70,50)));
        //                                emit.TestUpdate();
        //                            };
        //    game.DrawEvent += delegate
        //                          {
        //                              game.GraphicsDevice.RenderState.CullMode = CullMode.None;
        //                              emit.Render(game.SpectaterCamera.ViewProjection, game.SpectaterCamera.ViewInverse);
        //                          };

        //    game.Run();
        //}
        [Test]
        public void BasicBalTest()
        {
            Emitter emit;
            DX11Game game = new DX11Game();
            game.InitDirectX();


            var texPool = new TexturePool(game);
            var testTexture = GetTestTexture();
            BallParticleCreater creater = new BallParticleCreater();
            //SimpleParticleCreater creater = new SimpleParticleCreater();
            EmitterParameters param = new EmitterParameters();
            param.texture = testTexture;
            param.particleCreater = creater;
            emit = new Emitter(texPool, game, param,800,600);//note: again screen size
            //game.Wpf.CreateClassForm(param);
            Seeder seed = new Seeder(54);
            var curve = Curve3DTester.CreateTestCurve();
           
            

            emit.Initialize();
            emit.InitializeRender();


            emit.CreateRenderData();
            emit.SetRenderData();
            emit.SetPosition(Vector3.Zero);
            float dist = 0;
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
                Temp(dist, emit, curve);

                //Draw part

                //game.GraphicsDevice.Clear(Color.Black);
                game.Device.ImmediateContext.Rasterizer.State = game.HelperStates.RasterizerShowAll;
                emit.Render(game.SpectaterCamera.ViewProjection, game.SpectaterCamera.ViewInverse);
                
            };
           
              
           

            game.Run();
        }
        [Test]
        public void FlameTest()
        {
            Emitter emit;
            DX11Game game = new DX11Game();
            game.InitDirectX();
            FlameParticleCreater creater;
            //game.DrawFps = true;

            //var pool = new VertexDeclarationPool();
            //pool.SetVertexElements<Emitter.ParticleVertex>(Emitter.ParticleVertex.VertexElements);
            var texPool = new TexturePool(game);
            var testTexture = GetTestTexture();
            creater = new FlameParticleCreater();
            EmitterParameters param = new EmitterParameters();
            param.EffectName = "calculateFlame";
            param.texture = testTexture;
            param.particleCreater = creater;
            emit = new Emitter(texPool, game, param,800,600);
            Seeder seed = new Seeder(54);

            var curve = Curve3DTester.CreateTestCurve();

            
                //texPool.Initialize(game);
                //pool.Initialize(game);

                emit.Initialize();
                emit.InitializeRender();


                emit.CreateRenderData();
                emit.SetRenderData();
                //emit.AddParticles(creater,1);
            
            float dist = 0;
            game.GameLoopEvent += delegate
            {
                dist += game.Elapsed;
                // emit.setShader();  
                //Temp(dist, emit, curve);

                //setColors(emit);
                emit.Update();
            emit.Render(game.SpectaterCamera.ViewProjection, game.SpectaterCamera.ViewInverse);

            };
           
            game.Run();
        }

        //[Test]
        //public void SparkleTest()
        //{
        //    Emitter emit;
        //    XNAGame game = new XNAGame();
        //    SparkelParticleCreater creater;
        //    game.DrawFps = true;

        //    var pool = new VertexDeclarationPool();
        //    pool.SetVertexElements<Emitter.ParticleVertex>(Emitter.ParticleVertex.VertexElements);
        //    var texPool = new TexturePool();
        //    var testTexture = GetTestTexture();
        //    creater = new SparkelParticleCreater();
        //    EmitterParameters param = new EmitterParameters();
        //    param.EffectName = "CalculateSpark";
        //    param.texture = testTexture;
        //    param.particleCreater = creater;
        //    emit = new Emitter(texPool, pool, game, param);

        //    param.Directional = true;
        //    Seeder seed = new Seeder(54);
        //    param.UvStart = new Vector2(0, 0.5f);
        //    param.UvSize = new Vector2(0.5f, 0.1f);
        //    param.startColor = Color.LightYellow;
        //    param.endColor = Color.OrangeRed;
        //    game.Wpf.CreateClassForm(param);

        //    game.InitializeEvent += delegate
        //    {
        //        texPool.Initialize(game);
        //        pool.Initialize(game);

        //        emit.Initialize();
        //        emit.InitializeRender();


        //        emit.CreateRenderData();
        //        emit.SetRenderData();
               
        //    };
        //    float dist = 0;
        //    game.UpdateEvent += delegate
        //    {
                
        //        emit.Update();

        //    };
        //    game.DrawEvent += delegate
        //    {
        //        //game.GraphicsDevice.Clear(Color.Black);
        //        game.GraphicsDevice.RenderState.CullMode = CullMode.None;
        //        emit.Render(game.SpectaterCamera.ViewProjection, game.SpectaterCamera.ViewInverse);
              
        //    };

        //    game.Run();
        //}
        ////private void setColors(Emitter emit)
        ////{
        ////    //emit.StartColor = new Color(20,20,200);
        ////    emit.EndColor = new Color(0,0,0);
        ////}


        //[Test]
        //public void BasicEffectTest()
        //{
            
        //    XNAGame game = new XNAGame();
        //    SparkelParticleCreater creater;
        //    game.DrawFps = true;

        //    var pool = new VertexDeclarationPool();
        //    pool.SetVertexElements<Emitter.ParticleVertex>(Emitter.ParticleVertex.VertexElements);
        //    var texPool = new TexturePool();
        //    var testTexture = GetTestTexture();
        //    creater = new SparkelParticleCreater();
        //    EmitterParameters param = new EmitterParameters();
        //    param.EffectName = "CalculateSpark";
        //    param.texture = testTexture;
        //    param.particleCreater = creater;
        //    param.Directional = true;
        //    Seeder seed = new Seeder(54);
        //    param.UvStart = new Vector2(0, 0.5f);
        //    param.UvSize = new Vector2(0.5f, 0.1f);
        //    param.startColor = Color.LightYellow;
        //    param.endColor = Color.OrangeRed;
        //    param.Continueous = false;
            

        //    ParticleEffect effect = new ParticleEffect(game, pool, texPool);game.Wpf.CreateClassForm(effect);
        //    effect.AddEmitter(param);
        //    game.InitializeEvent += delegate
        //    {
        //        texPool.Initialize(game);
        //        pool.Initialize(game);
        //        effect.Initialize();

        //    };
        //    float dist = 0;
        //    game.UpdateEvent += delegate
        //                            {
        //                                if(game.Keyboard.IsKeyPressed(Keys.T))
        //                                {
        //                                    effect.Trigger();
        //                                }
        //                                effect.Update();

        //                            };
        //    game.DrawEvent += delegate
        //    {
        //       game.GraphicsDevice.Clear(Color.Black);
        //       game.GraphicsDevice.RenderState.CullMode = CullMode.None;
        //       effect.Render(game.SpectaterCamera.ViewProjection, game.SpectaterCamera.ViewInverse);

        //    };

        //    game.Run();
        //}
        private void Temp(float dist, Emitter emit, Curve3D curve)
        {
            emit.SetPosition(curve.Evaluate(dist*3).dx()*2);
        }
    }
}
