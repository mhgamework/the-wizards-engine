using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.ServerClient.Engine;

namespace MHGameWork.TheWizards.ServerClient.XNAGeoMipMap
{
    public class SkyBox001
    {
        private Microsoft.Xna.Framework.Graphics.Model skyboxMesh;
        public Vector3 myPosition;
        public Quaternion myRotation;
        public Vector3 myScale;

        public TextureCube environ;

        ShaderEffect shader;

        GameFileOud modelFile;
        GameFileOud shaderFile;
        GameFileOud textureFile;

        //ContentManager content;
        ServerClientMainOud engine;

        public SkyBox001( ServerClientMainOud nEngine, GameFileOud nModelFile, GameFileOud nShaderFile, GameFileOud nTextureFile )
        //: base( game )
        {
            engine = nEngine;

            modelFile = nModelFile;
            shaderFile = nShaderFile;
            textureFile = nTextureFile;
            //content = new ContentManager( game.Services );


            myPosition = new Vector3( 0, 0, 0 );
            myRotation = new Quaternion( 0, 0, 0, 1 );
            //myScale = new Vector3( 55, 55, 55 );
            myScale = new Vector3( 550, 550, 550 );
        }

        public Engine.LoadingTaskState LoadNormalTask( Engine.LoadingTaskType taskType )
        {
            if ( modelFile.State != GameFileOud.GameFileState.UpToDate )
            {
                modelFile.SynchronizeAsync();

                return LoadingTaskState.Subtasking;
            }
            if ( shaderFile.State != GameFileOud.GameFileState.UpToDate )
            {
                modelFile.SynchronizeAsync();

                return LoadingTaskState.Subtasking;
            }
            if ( textureFile.State != GameFileOud.GameFileState.UpToDate )
            {
                modelFile.SynchronizeAsync();

                return LoadingTaskState.Subtasking;
            }

            skyboxMesh = engine.XNAGame.Content.Load<Microsoft.Xna.Framework.Graphics.Model>( modelFile.GetFullFilename() );
            //TODO: shader = new ShaderEffect( engine, shaderFile.GetFullFilename() );
            environ = TextureCube.FromFile( engine.XNAGame.GraphicsDevice, textureFile.GetFullFilename() );

            return LoadingTaskState.Completed;
        }

        //protected void LoadGraphicsContent( bool loadAllContent )
        //{
        //    if ( loadAllContent )
        //    {
        //        //skyboxMesh = content.Load<Model>( modelAsset );
        //        //shader = content.Load<Effect>( shaderAsset );
        //        //environ = content.Load<TextureCube>( textureAsset );
        //    }
        //    //base.LoadGraphicsContent( loadAllContent );
        //}

        //public void Draw( GameTime gameTime )
        //{
        //    //Matrix World = Matrix.CreateScale( myScale ) *
        //    //                Matrix.CreateFromQuaternion( myRotation ) *
        //    //                Matrix.CreateTranslation( Camera.myPosition );

        //    //shader.Parameters[ "World" ].SetValue( World );
        //    //shader.Parameters[ "View" ].SetValue( Camera.myView );
        //    //shader.Parameters[ "Projection" ].SetValue( Camera.myProjection );
        //    //shader.Parameters[ "surfaceTexture" ].SetValue( environ );

        //    //shader.Parameters[ "EyePosition" ].SetValue( Camera.myPosition );

        //    //for ( int pass = 0; pass < shader.CurrentTechnique.Passes.Count; pass++ )
        //    //{
        //    //    for ( int msh = 0; msh < skyboxMesh.Meshes.Count; msh++ )
        //    //    {
        //    //        ModelMesh mesh = skyboxMesh.Meshes[ msh ];
        //    //        for ( int prt = 0; prt < mesh.MeshParts.Count; prt++ )
        //    //            mesh.MeshParts[ prt ].Effect = shader;
        //    //        mesh.Draw();
        //    //    }
        //    //}

        //    //base.Draw( gameTime );
        //}

        public void Render()
        {
            if ( shader == null ) return;

            Matrix World = Matrix.CreateScale( myScale ) *
                            Matrix.CreateFromQuaternion( myRotation )
                //*Matrix.CreateTranslation( engine.ActiveCamera.CameraPosition );
                            * Matrix.CreateTranslation( engine.ActiveCamera.CameraPosition.X, 0, engine.ActiveCamera.CameraPosition.Z );

            shader.Effect.Parameters[ "World" ].SetValue( World );
            shader.Effect.Parameters[ "View" ].SetValue( engine.ActiveCamera.CameraInfo.ViewMatrix );
            shader.Effect.Parameters[ "Projection" ].SetValue( engine.ActiveCamera.CameraInfo.ProjectionMatrix );
            shader.Effect.Parameters[ "surfaceTexture" ].SetValue( environ );

            shader.Effect.Parameters[ "EyePosition" ].SetValue( new Vector3( engine.ActiveCamera.CameraPosition.X, 0, engine.ActiveCamera.CameraPosition.Z ) );

            for ( int pass = 0; pass < shader.Effect.CurrentTechnique.Passes.Count; pass++ )
            {
                for ( int msh = 0; msh < skyboxMesh.Meshes.Count; msh++ )
                {
                    ModelMesh mesh = skyboxMesh.Meshes[ msh ];
                    for ( int prt = 0; prt < mesh.MeshParts.Count; prt++ )
                        mesh.MeshParts[ prt ].Effect = shader.Effect;
                    mesh.Draw();
                }
            }

        }

        public static void TestRenderSkybox001()
        {
            TestServerClientMain main = null;

            SkyBox001 sky = null;
            TestServerClientMain.Start( "TestRenderSkybox001",
                delegate
                {
                    main = TestServerClientMain.Instance;

                    GameFileOud model = new GameFileOud( main.XNAGame.Content.RootDirectory + @"\Content\skybox" );
                    GameFileOud shader = new GameFileOud( main.XNAGame.Content.RootDirectory + @"\Content\Skybox001.fx" );
                    GameFileOud texture = new GameFileOud( main.XNAGame.Content.RootDirectory + @"\Content\Skybox001.dds" );


                    sky = new SkyBox001( main, model, shader, texture );

                    main.LoadingManager.AddLoadTaskAdvanced( sky.LoadNormalTask, LoadingTaskType.Normal );

                },
                delegate
                {
                    main.LoadingManager.ProcessNextTask( LoadingTaskType.Normal );

                    sky.Render();

                } );
        }
    }
}
