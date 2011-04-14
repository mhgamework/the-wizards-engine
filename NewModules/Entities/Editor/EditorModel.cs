using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Entity.Editor;
using MHGameWork.TheWizards.ServerClient;
using MHGameWork.TheWizards.ServerClient.Collada;
using MHGameWork.TheWizards.ServerClient.Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Entities.Editor
{
    public class EditorModel
    {

        private ModelFullData fullData;

        public ModelFullData FullData
        {
            get { return fullData; }
            set { fullData = value; }
        }

        private EditorModelRenderData renderData;
        /// <summary>
        /// TODO: Should not be in this class
        /// </summary>
        public EditorModelRenderData RenderData
        {
            get { return renderData; }
            set { renderData = value; }
        }



        public EditorModel()
        {
            fullData = new ModelFullData();
        }


        /// <summary>
        /// Returns an array of EditorModels containing all of the parts in the collada file
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="colladaFile"></param>
        /// <returns></returns>
        public static List<EditorModel> FromColladaModel( EditorObject obj, ColladaModel colladaModel )
        {
            List<EditorModel> list = new List<EditorModel>();
            Dictionary<ColladaMaterial, EditorMaterial> materials = new Dictionary<ColladaMaterial, EditorMaterial>();


            // Load Materials ( Shaders + parameters )
            /*for ( int iMat = 0; iMat < colladaModel.materials.Count; iMat++ )
            {
                EditorMaterialCollada mat = EditorMaterialCollada.FromColladaMaterial( colladaModel.materials[ iMat ] );
                materials[ colladaModel.materials[ iMat ] ] = mat;
            }*/

            // Load all models from the scene. Each meshPart/primitivelist will become a seperate model
            LoadFromColladaSceneNode( colladaModel.Scene, list );

            /*//Add Parts
            for ( int iNode = 0; iNode < colladaScene.Nodes.Count; iNode++ )
            {
                LoadFromColladaSceneNode( colladaScene.Nodes[ iNode ] );
            
            }*/

            /*// Calculate the bouding box
            if ( model.parts.Count > 0 )
            {
                BoundingBox bb = model.parts[ 0 ].BoundingBox;
                for ( int i = 1; i < model.parts.Count; i++ )
                {
                    bb = BoundingBox.CreateMerged( bb, model.parts[ i ].BoundingBox );
                }
                model.boundingBox = bb;

            }*/



            // Can be disposed now, was only needed for loading the correct materials in the modelparts.
            //model.tempMaterials.Clear();
            //model.tempMaterials = null;

            return list;

        }

        private static void LoadFromColladaSceneNode( ColladaSceneNodeBase node, List<EditorModel> models )
        {
            if ( node.Type != ColladaSceneNodeBase.NodeType.Node && node.Type != ColladaSceneNodeBase.NodeType.Scene ) return;
            if ( node.Instance_Geometry != null )
            {
                ColladaMesh mesh = node.Instance_Geometry;

                for ( int iPart = 0; iPart < mesh.Parts.Count; iPart++ )
                {
                    ColladaMesh.PrimitiveList meshPart = mesh.Parts[ iPart ];

                    EditorModel model = new EditorModel();

                    model.LoadDataFromColladaMeshPart( model, meshPart );

                    models.Add( model );


                    model.FullData.ObjectMatrix = node.GetFullMatrix();
                    // TODO: this only counts when model is from max!
                    model.FullData.ObjectMatrix = model.FullData.ObjectMatrix * Matrix.CreateRotationX( -MathHelper.PiOver2 );
                }
            }

            for ( int iNode = 0; iNode < node.Nodes.Count; iNode++ )
            {
                LoadFromColladaSceneNode( node.Nodes[ iNode ], models );
            }

        }


        /// <summary>
        /// Checks whether a ray intersects a model. 
        /// Returns the distance along the ray to the point of intersection, or null
        /// if there is no intersection.
        /// </summary>
        public float? RayIntersectsModel( Ray ray, Matrix modelWorldMatrix, out Vector3 vertex1, out Vector3 vertex2, out Vector3 vertex3 )
        {

            if ( fullData == null ) throw new Exception( "EditorFullModelData not loaded!" );

            vertex1 = vertex2 = vertex3 = Vector3.Zero;

            // The input ray is in world space, but our model data is stored in object
            // space. We would normally have to transform all the model data by the
            // modelTransform matrix, moving it into world space before we test it
            // against the ray. That transform can be slow if there are a lot of
            // triangles in the model, however, so instead we do the opposite.
            // Transforming our ray by the inverse modelTransform moves it into object
            // space, where we can test it directly against our model data. Since there
            // is only one ray but typically many triangles, doing things this way
            // around can be much faster.

            Matrix modelMatrix = fullData.ObjectMatrix * modelWorldMatrix;
            Matrix inverseTransform = Matrix.Invert( modelMatrix );

            ray.Position = Vector3.Transform( ray.Position, inverseTransform );
            ray.Direction = Vector3.TransformNormal( ray.Direction, inverseTransform );


            //ray.Direction.Normalize();


            //Little trick: the ray direction is not normalized, but the algorithm seems to work using that value, but the bounding check doesn't
            Ray ray2 = ray;
            ray2.Direction.Normalize();

            if ( !fullData.BoundingSphere.Intersects( ray2 ).HasValue )
            {
                // If the ray does not intersect the bounding sphere, we cannot
                // possibly have picked this model, so there is no need to even
                // bother looking at the individual triangle data.
                //insideBoundingSphere = false;

                return null;
            }


            // The bounding sphere test passed, so we need to do a full
            // triangle picking test.
            //insideBoundingSphere = true;

            // Keep track of the closest triangle we found so far,
            // so we can always return the closest one.
            float? closestIntersection = null;

            // Loop over the vertex data, 3 at a time (3 vertices = 1 triangle).
            Vector3[] vertices = fullData.Positions;

            for ( int i = 0; i < vertices.Length; i += 3 )
            {
                // Perform a ray to triangle intersection test.
                float? intersection;

                ServerClient.MathExtra.Functions.RayIntersectsTriangle( ref ray,
                                                                        ref vertices[ i ],
                                                                        ref vertices[ i + 1 ],
                                                                        ref vertices[ i + 2 ],
                                                                        out intersection );


                // Does the ray intersect this triangle?
                if ( intersection != null )
                {
                    // If so, is it closer than any other previous triangle?
                    if ( ( closestIntersection == null ) ||
                         ( intersection < closestIntersection ) )
                    {
                        // Store the distance to this triangle.
                        closestIntersection = intersection;

                        // Transform the three vertex positions into world space,
                        // and store them into the output vertex parameters.
                        Vector3.Transform( ref vertices[ i ],
                                           ref modelMatrix, out vertex1 );

                        Vector3.Transform( ref vertices[ i + 1 ],
                                           ref modelMatrix, out vertex2 );

                        Vector3.Transform( ref vertices[ i + 2 ],
                                           ref modelMatrix, out vertex3 );
                    }
                }
            }

            return closestIntersection;
        }


        /// <summary>
        /// Helper method for loading as much data from a colladaMeshPart (a collada primitivelist) as possible and storing it.
        /// This does not fill all the data in this class, other data can be filled directly or using possible other helper methods.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="meshPart"></param>
        /// <returns></returns>
        public void LoadDataFromColladaMeshPart( EditorModel model, ColladaMesh.PrimitiveList meshPart )
        {

            fullData.TriangleCount = meshPart.PrimitiveCount;

            if ( fullData.TriangleCount * 3 >= int.MaxValue ) throw new Exception( "Too many vertices" );
            int numVertices = fullData.TriangleCount * 3;

            if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.Position ) ) fullData.Positions = new Vector3[ numVertices ];
            if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.Normal ) ) fullData.Normals = new Vector3[ numVertices ];
            if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.Texcoord, 1 ) ) fullData.TexCoords = new Vector3[ numVertices ];
            if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.TexTangent, 1 ) ) fullData.Tangents = new Vector3[ numVertices ];


            for ( int i = 0; i < numVertices; i++ )
            {

                fullData.Positions[ i ] = meshPart.GetVector3( ColladaMesh.Input.InputSemantics.Position, i ); //GetPosition( i );
                fullData.Normals[ i ] = meshPart.GetVector3( ColladaMesh.Input.InputSemantics.Normal, i );


                // Texture Coordinates
                Vector3 texcoord = Vector3.Zero;
                if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.Texcoord, 1 ) )
                {
                    texcoord = meshPart.GetVector3( ColladaMesh.Input.InputSemantics.Texcoord, 1, i );
                    texcoord.Y = 1.0f - texcoord.Y; // V coordinate is inverted in max
                }
                fullData.TexCoords[ i ] = texcoord;

                // Tangent
                Vector3 tangent = Vector3.Zero;
                if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.TexTangent, 1 ) )
                {
                    fullData.Tangents[ i ] = meshPart.GetVector3( ColladaMesh.Input.InputSemantics.TexTangent, 1, i );
                }
            }

            fullData.CalculateBoundingBox();
            fullData.CalculateBoundingSphere();


            //TODO: Material, maybe not really a good way of doing this
            LoadDataFromColladaMaterial( meshPart.Material );


        }

        /// <summary>
        /// Helper method for storing data. This actually stores shader parameters.
        /// Note: This does not rreaally support material sharing among multiple models
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        private void LoadDataFromColladaMaterial( ColladaMaterial mat )
        {
            fullData.MaterialName = mat.Name;

            fullData.Ambient = mat.Ambient;
            fullData.Diffuse = mat.Diffuse;
            fullData.Specular = mat.Specular;
            fullData.Shininess = mat.Shininess;

            if ( mat.DiffuseTexture != null )
                fullData.DiffuseTexture = mat.DiffuseTexture.GetFullFilename();
            fullData.DiffuseTextureRepeatU = mat.DiffuseTextureRepeatU;
            fullData.DiffuseTextureRepeatV = mat.DiffuseTextureRepeatV;

            if ( mat.NormalTexture != null )
                fullData.NormalTexture = mat.NormalTexture.GetFullFilename();
            fullData.NormalTextureRepeatU = mat.NormalTextureRepeatU;
            fullData.NormalTextureRepeatV = mat.NormalTextureRepeatV;


        }



    }
}