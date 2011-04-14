using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Common.Core.Collada;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Graphics;
using MHGameWork.TheWizards.Collada;
using MHGameWork.TheWizards.ServerClient.Graphics;
using MHGameWork.TheWizards.ServerClient.Engine;

namespace MHGameWork.TheWizards.Common.Core.Graphics
{
    /// <summary>
    /// TODO: Lighting does not appear to be correct!
    /// TODO: this should support keyframed animation.
    /// NOTE: This requires the XNAGame to be initialized on constructor!
    /// </summary>
    public class SkinnedModel
    {
        public static Color[] BoneColors = new Color[]
				{ Color.DarkBlue, Color.DarkRed, Color.Yellow, Color.White, Color.Teal,
				Color.RosyBrown, Color.Orange, Color.Olive, Color.Maroon, Color.Lime,
				Color.LightBlue, Color.LightGreen, Color.Lavender, Color.Green,
				Color.Firebrick, Color.DarkKhaki, Color.BlueViolet, Color.Beige };

        IXNAGame game;
        public IXNAGame Game { get { return game; } }
        Matrix worldMatrix = Matrix.Identity;

        public List<SkinnedMesh> Meshes = new List<SkinnedMesh>();
        /// <summary>
        /// Cloned by the meshes to reduce memory overhead (and allow parameter sharing?)
        /// </summary>
        public SkinnedShader BaseShader;
        public List<SkinnedBone> Bones = new List<SkinnedBone>();
        public float FrameRate = 1;
        /// <summary>
        /// Was this animation data already constructed last time we called
        /// UpdateAnimation? Will not optimize much if you render several models
        /// of this type (maybe use a instance class that holds this animation
        /// data and just calls this class for rendering to optimize it further),
        /// but if you just render a single model, it gets a lot faster this way.
        /// </summary>
        private int lastAniMatrixNum = -1;
        /// <summary>
        /// Number of values in the animationMatrices in each bone.
        /// TODO: Split the animations up into several states (stay, stay to walk,
        /// walk, fight, etc.), but not required here in this test app yet ^^
        /// TODO: MHGW: This seems to be an unusual name for this variable (more like numFrames or numKeyFrames?)
        /// </summary>
        public int numOfAnimations = 1;

        private VertexDeclaration vertexDeclaration;

        /// <summary>
        /// Value between 0 and 1 defining the start and end of the animation.
        /// </summary>
        private float animationProgress = 0;
        public float AnimationProgress
        {
            get { return animationProgress; }
            set { SetAnimationProgress( value ); }
        }


        public SkinnedModel( IXNAGame _game )
        {
            game = _game;
            BaseShader = new SkinnedShader( game, new EffectPool() );
            vertexDeclaration = AttributeSystem.CreateVertexDeclaration( game.GraphicsDevice, typeof( SkinnedTangentVertex ) );
        }

        public void Render()
        {
            BaseShader.ViewProjection = game.Camera.ViewProjection;
            BaseShader.CameraPosition = game.Camera.ViewInverse.Translation;
            BaseShader.SetBoneMatrices( GetBoneMatrices( worldMatrix ) );

            game.GraphicsDevice.VertexDeclaration = vertexDeclaration;

            for ( int i = 0; i < Meshes.Count; i++ )
            {
                Meshes[ i ].Render();
            }
        }

        public void RenderBonesLines()
        {
            foreach ( SkinnedBone bone in Bones )
            {
                foreach ( SkinnedBone childBone in bone.children )
                    game.LineManager3D.AddLine(
                        ( bone.finalMatrix * worldMatrix ).Translation,
                        ( childBone.finalMatrix * worldMatrix ).Translation,
                        BoneColors[ bone.num % BoneColors.Length ] );

                game.LineManager3D.AddCenteredBox( ( bone.finalMatrix * worldMatrix ).Translation, 0.2f, BoneColors[ bone.num % BoneColors.Length ] );

            } // foreach (bone)

        }



        /// <summary>
        /// Update animation. Will do nothing if animation stayed the same since
        /// last time we called this method.
        /// </summary>
        public void UpdateAnimation( float elapsed )
        {
            // TODO: optimize using a constant for (FrameRate / numOfAnimations)
            AnimationProgress += elapsed * FrameRate / numOfAnimations;


        } // UpdateAnimation()

        private void SetAnimationProgress( float value )
        {
            if ( value < 0 ) value += (int)value + 1;
            animationProgress = value % 1;
            // Add some time to the animation depending on the position.
            int aniMatrixNum = (int)( AnimationProgress * numOfAnimations );
            if ( aniMatrixNum < 0 )
                aniMatrixNum = 0;
            // No need to update if everything stayed the same
            if ( aniMatrixNum == lastAniMatrixNum )
                return;
            lastAniMatrixNum = aniMatrixNum;

            foreach ( SkinnedBone bone in Bones )
            {
                // Just assign the final matrix from the animation matrices.
                bone.finalMatrix = bone.animationMatrices[ aniMatrixNum ];

                // Also use parent matrix if we got one
                // This will always work because all the bones are in order.
                if ( bone.parent != null )
                    bone.finalMatrix *=
                        bone.parent.finalMatrix;

                //bone.finalMatrix = Matrix.Identity;
            } // foreach
        }

        /// <summary>
        /// Get bone matrices for the shader. We have to apply the invBoneSkinMatrix
        /// to each final matrix, which is the recursively created matrix from
        /// all the animation data (see UpdateAnimation).
        /// </summary>
        /// <returns></returns>
        private Matrix[] GetBoneMatrices( Matrix renderMatrix )
        {
            // Update the animation data in case it is not up to date anymore.
            //UpdateAnimation( renderMatrix );

            // And get all bone matrices, we support max. 80 (see shader).
            // TODO: massive memory allocation??
            Matrix[] matrices = new Matrix[ Math.Min( 80, Bones.Count ) ];
            for ( int num = 0; num < matrices.Length; num++ )
            {
                // The matrices are constructed from the invBoneSkinMatrix and
                // the finalMatrix, which holds the recursively added animation matrices
                // and finally we add the render matrix too here.
                matrices[ num ] =
                     Bones[ num ].invBoneSkinMatrix * Bones[ num ].finalMatrix * renderMatrix;
                //matrices[ num ] = Bones[ 1 ].invBoneSkinMatrix * Bones[ 1 ].finalMatrix;
                //BaseShader.Shader.SetParameter( "tempSkinMatrix", Matrix.Transpose( matrices[ num ] ) );

                //matrices[ num ] = Matrix.Identity;
                //bones[ num ].invBoneSkinMatrix * renderMatrix;
            }

            return matrices;
        } // GetBoneMatrices()


        public Matrix WorldMatrix
        {
            get { return WorldMatrix; }
            set
            {
                worldMatrix = value;
                /*for ( int i = 0; i < Meshes.Count; i++ )
                {
                    Meshes[ i ].SetWorldMatrix( value );
                }
                BaseShader.World = value;*/
            }
        }

        public static void TestRenderBones()
        {
            TestXNAGame main = null;


            SkinnedModel model = null;
            ColladaModel col = null;

            TestXNAGame.Start( "TestShowBones",
                delegate
                {
                    main = TestXNAGame.Instance;


                    //col = ColladaModel.FromFile( new GameFile( main.RootDirectory + "/Content/Goblin.DAE" ) );
                    //col = ColladaModel.LoadSimpleCharacterAnim001();
                    col = ColladaModel.LoadSimpleBones001();

                    model = SkinnedModelBuilder.LoadSkinnedModel( main, col );
                    //model.WorldMatrix = Matrix.CreateFromYawPitchRoll( -MathHelper.PiOver2, -MathHelper.PiOver2, 0 );
                },
                delegate
                {
                    model.UpdateAnimation( main.Elapsed );
                    model.RenderBonesLines();

                } );
        }


        public static void TestRenderAnimated()
        {
            TestXNAGame main = null;
            SkinnedModel model = null;

            ColladaModel col = null;


            TestXNAGame.Start( "TestRenderAnimated",
                delegate
                {
                    main = TestXNAGame.Instance;

                    //col = ColladaModel.FromFile( new GameFile( main.RootDirectory + "/Content/Goblin.DAE" ) );
                    //col = ColladaModel.FromFile( new GameFile( main.RootDirectory + "/DebugFiles/VerySimpleBones001(disabled).DAE" ) );
                    //col = ColladaModel.FromFile( new GameFile( main.RootDirectory + "/Content/TriangleBones002.DAE" ) );
                    col = ColladaModel.LoadSimpleCharacterAnim001();
                    //col = ColladaModel.LoadSimpleBones001();
                    //col = ColladaModel.LoadTriangleBones001();

                    model = SkinnedModelBuilder.LoadSkinnedModel( main, col );
                    model.WorldMatrix = Matrix.CreateFromYawPitchRoll( -MathHelper.PiOver2, -MathHelper.PiOver2, 0 );
                    //model.WorldMatrix = Matrix.Identity;
                },
                delegate
                {
                    main.GraphicsDevice.RenderState.CullMode = CullMode.None;
                    if ( main.Keyboard.IsKeyPressed( Microsoft.Xna.Framework.Input.Keys.R ) )
                    {
                        model = SkinnedModelBuilder.LoadSkinnedModel( main, col );
                        model.WorldMatrix = Matrix.Identity;// Matrix.CreateFromYawPitchRoll( -MathHelper.PiOver2, -MathHelper.PiOver2, 0 );
                    }
                    model.UpdateAnimation( main.Elapsed );
                    model.Render();
                    model.RenderBonesLines();
                } );
        }



    }
}
