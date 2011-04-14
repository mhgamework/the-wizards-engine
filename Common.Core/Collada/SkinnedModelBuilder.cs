using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Common.Core.Graphics;
using System.IO;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient.Collada;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.ServerClient.Engine;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Collada
{
    public class SkinnedModelBuilder
    {
        private ColladaModel colladaModel;
        private SkinnedModel skinnedModel;

        private SkinnedModelBuilder()
        {

        }

        public SkinnedModel CreateSkinnedModel( IXNAGame game, ColladaModel _colladaModel )
        {
            SkinnedModel ret;

            colladaModel = _colladaModel;
            skinnedModel = new SkinnedModel( game );
            ret = skinnedModel;


            skinnedModel.FrameRate = colladaModel.frameRate;
            skinnedModel.numOfAnimations = colladaModel.numOfAnimations;

            LoadMeshes();
            LoadBones();


            // Clean up for possible next creation
            colladaModel = null;
            skinnedModel = null;

            return ret;
        }


        private void LoadBones()
        {

            LoadBone( null, colladaModel.bones[ 0 ] );

        }
        private void LoadBone( SkinnedBone parentBone, ColladaBone colladaBone )
        {
            SkinnedBone bone = new SkinnedBone();
            bone.num = skinnedModel.Bones.Count;
            skinnedModel.Bones.Add( bone );

            bone.parent = parentBone;
            if ( parentBone != null ) parentBone.children.Add( bone );
            bone.initialMatrix = colladaBone.initialMatrix;
            bone.invBoneSkinMatrix = colladaBone.invBoneSkinMatrix;
            bone.animationMatrices = colladaBone.animationMatrices;
            bone.finalMatrix = bone.GetMatrixRecursively();

            for ( int i = 0; i < colladaBone.children.Count; i++ )
            {
                LoadBone( bone, colladaBone.children[ i ] );
            }
        }






        public void LoadMeshes()
        {
            //Dictionary<ColladaMaterial, EditorMaterial> materials = new Dictionary<ColladaMaterial, EditorMaterial>();

            // Load all models from the scene. Each meshPart/primitivelist will become a seperate model
            //TODO: this does not work, now simply load every mesh found
            //LoadFromColladaSceneNode( colladaModel.Scene );
            for ( int i = 0; i < colladaModel.Meshes.Count; i++ )
            {
                ColladaMesh colladaMesh = colladaModel.Meshes[ i ];
                for ( int j = 0; j < colladaMesh.Parts.Count; j++ )
                {
                    SkinnedMesh mesh = new SkinnedMesh( skinnedModel );
                    skinnedModel.Meshes.Add( mesh );
                    LoadMesh( mesh, colladaMesh, colladaMesh.Parts[ j ], Matrix.CreateRotationX( -MathHelper.PiOver2 ) );
                }

            }
        }

        private void LoadFromColladaSceneNode( ColladaSceneNodeBase node )
        {
            if ( node.Type != ColladaSceneNodeBase.NodeType.Node && node.Type != ColladaSceneNodeBase.NodeType.Scene ) return;
            // This is not correct. When using a skin there is an 'instance_controller'
            if ( node.Instance_Geometry != null )
            {
                ColladaMesh mesh = node.Instance_Geometry;

                for ( int iPart = 0; iPart < mesh.Parts.Count; iPart++ )
                {
                    ColladaMesh.PrimitiveList meshPart = mesh.Parts[ iPart ];

                    SkinnedMesh skinnedMesh = new SkinnedMesh( skinnedModel );
                    Matrix objectMatrix = node.GetFullMatrix() * Matrix.CreateRotationX( -MathHelper.PiOver2 );

                    LoadMesh( skinnedMesh, mesh, meshPart, objectMatrix );

                    skinnedModel.Meshes.Add( skinnedMesh );

                    //model.FullData.ObjectMatrix = node.GetFullMatrix();
                    // TODO: this only counts when model is from max!
                    //model.FullData.ObjectMatrix = model.FullData.ObjectMatrix * Matrix.CreateRotationX( -MathHelper.PiOver2 );
                }
            }

            for ( int iNode = 0; iNode < node.Nodes.Count; iNode++ )
            {
                LoadFromColladaSceneNode( node.Nodes[ iNode ] );
            }

        }

        private void LoadMesh( SkinnedMesh mesh, ColladaMesh colladaMesh, ColladaMesh.PrimitiveList primitives, Matrix objectMatrix )
        {
            //EDIT: the vertices now include the objectmatrix, so this is always identity
            objectMatrix = colladaMesh.objectMatrix;
            mesh.Shader.World = Matrix.Identity;

            mesh.Primitives = new Primitives();

            mesh.Primitives.PrimitiveCount = primitives.PrimitiveCount;
            mesh.Primitives.VertexCount = primitives.PrimitiveCount * 3;

            if ( primitives.PrimitiveCount * 3 >= int.MaxValue ) throw new Exception( "Too many vertices" );
            int numVertices = primitives.PrimitiveCount * 3;



            // TODO check if all inputs are available

            /*if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.Position ) ) positions = new Vector3[ numVertices ];
            if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.Normal ) ) normals = new Vector3[ numVertices ];
            if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.Texcoord, 1 ) ) texCoords = new Vector3[ numVertices ];
            if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.TexTangent, 1 ) ) tangents = new Vector3[ numVertices ];*/


            SkinnedTangentVertex[] vertices = new SkinnedTangentVertex[ numVertices ];

            for ( int i = 0; i < numVertices; i++ )
            {
                SkinnedTangentVertex v = new SkinnedTangentVertex();
                v.pos = primitives.GetVector3( ColladaMesh.Input.InputSemantics.Position, i ); //GetPosition( i );
                v.normal = primitives.GetVector3( ColladaMesh.Input.InputSemantics.Normal, i );


                // Texture Coordinates
                Vector3 texcoord = Vector3.Zero;
                if ( primitives.ContainsInput( ColladaMesh.Input.InputSemantics.Texcoord, 1 ) )
                {
                    texcoord = primitives.GetVector3( ColladaMesh.Input.InputSemantics.Texcoord, 1, i );
                    texcoord.Y = 1.0f - texcoord.Y; // V coordinate is inverted in max
                }
                v.uv = new Vector2( texcoord.X, texcoord.Y );

                // Tangent
                Vector3 tangent = Vector3.Zero;
                if ( primitives.ContainsInput( ColladaMesh.Input.InputSemantics.TexTangent, 1 ) )
                {
                    v.tangent = primitives.GetVector3( ColladaMesh.Input.InputSemantics.TexTangent, 1, i );
                }


                // Bone weights and joint indices

                int positionIndex = primitives.GetSourceIndex(
                    primitives.GetInput( ColladaMesh.Input.InputSemantics.Position ), i );

                // WARNING: THIS IS EXTREMELY IMPORTANT AND COSTED MORE THAN 6 HOURS DEBUGGING
                //    Indexes are PREMULTIPLIED BY 3!!!
                v.blendIndices = colladaMesh.vertexSkinJoints[ positionIndex ];
                //v.blendIndices = Vector3.One;
                v.blendIndices = v.blendIndices * 3;
                v.blendWeights = colladaMesh.vertexSkinWeights[ positionIndex ];

                v.pos = Vector3.Transform( v.pos, objectMatrix );
                v.normal = Vector3.Transform( v.normal, objectMatrix );

                vertices[ i ] = v;

            }

            //vertices[ 0 ].pos = Vector3.Transform( Vector3.Zero, Matrix.Invert( objectMatrix ) );
            //vertices[ 0 ].pos = Vector3.Zero;

            mesh.Primitives.InitializeFromVertices( skinnedModel.Game, vertices, SkinnedTangentVertex.SizeInBytes );

            //CalculateBoundingBox();
            //CalculateBoundingSphere();


            //TODO: Material, maybe not really a good way of doing this
            //LoadDataFromColladaMaterial( meshPart.Material );
        }


        /// <summary>
        /// Helper method for storing data. This actually stores shader parameters.
        /// Note: This does not rreaally support material sharing among multiple models
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        private void LoadMeshMaterial( SkinnedMesh mesh, ColladaMaterial mat )
        {
            //MaterialName = mat.Name;

            mesh.Shader.AmbientColor = mat.Ambient.ToVector4();
            mesh.Shader.DiffuseColor = mat.Diffuse.ToVector4();
            mesh.Shader.SpecularColor = mat.Specular.ToVector4();
            mesh.Shader.Shininess = mat.Shininess;

            if ( mat.DiffuseTexture != null )
                ;//mesh.Shader        DiffuseTexture = mat.DiffuseTexture.GetFullFilename();
            mesh.Shader.DiffuseTextureRepeat = new Vector2( mat.DiffuseTextureRepeatU, mat.DiffuseTextureRepeatV );

            if ( mat.NormalTexture != null )
                ;// mesh.Shader NormalTexture = mat.NormalTexture.GetFullFilename();
            mesh.Shader.NormalTextureRepeat = new Vector2( mat.NormalTextureRepeatU, mat.NormalTextureRepeatV );


        }





        public static SkinnedModel LoadSkinnedModel( IXNAGame game, Stream colladaStream )
        {
            ColladaModel colladaModel = ColladaModel.FromStream( colladaStream );

            SkinnedModelBuilder builder = new SkinnedModelBuilder();

            return builder.CreateSkinnedModel( game, colladaModel );


        }
        public static SkinnedModel LoadSkinnedModel( IXNAGame game, ColladaModel model )
        {
            SkinnedModelBuilder builder = new SkinnedModelBuilder();

            return builder.CreateSkinnedModel( game, model );


        }
    }
}
