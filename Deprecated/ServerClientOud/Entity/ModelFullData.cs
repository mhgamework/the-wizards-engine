using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Entity
{
    /// <summary>
    /// This class contains All data in the engine of a specific model. It is not meant for real-time
    /// use but only for editor/preprocessor. It may contain redundant data
    /// </summary>
    public class ModelFullData
    {

        /**
         * 
         * Graphical Data:
         * Mesh
         * 
         **/

        private Microsoft.Xna.Framework.Vector3[] positions;

        public Microsoft.Xna.Framework.Vector3[] Positions
        {
            get { return positions; }
            set { positions = value; }
        }

        private Vector3[] normals;

        public Vector3[] Normals
        {
            get { return normals; }
            set { normals = value; }
        }

        private Vector3[] tangents;

        public Vector3[] Tangents
        {
            get { return tangents; }
            set { tangents = value; }
        }

        private Vector3[] texCoords;

        public Vector3[] TexCoords
        {
            get { return texCoords; }
            set { texCoords = value; }
        }

     
        public int TriangleCount;
        //public int VertexCount;
        //public int VertexStride;
        //public VertexDeclaration VertexDeclaration;






        /**
         * 
         * Graphical Data:
         * Material
         * 
         **/

        public string MaterialName;

        public Color Ambient;
        public Color Diffuse;
        public Color Specular;
        public float Shininess;

        public string DiffuseTexture;
        public float DiffuseTextureRepeatU = 1;
        public float DiffuseTextureRepeatV = 1;
        public string NormalTexture;
        public float NormalTextureRepeatU = 1;
        public float NormalTextureRepeatV = 1;









        /**
         * 
         * Various
         * 
         **/

        private string originalFilePath;

        public string OriginalFilePath
        {
            get { return originalFilePath; }
            set { originalFilePath = value; }
        }








        /**
         * 
         * Physical Data
         * 
         **/

        private Matrix objectMatrix;

        public Matrix ObjectMatrix
        {
            get { return objectMatrix; }
            set { objectMatrix = value; }
        }

        private Matrix localMatrix;
	

        private BoundingBox boundingBox;

        public BoundingBox BoundingBox
        {
            get { return boundingBox; }
            set { boundingBox = value; }
        }

        private BoundingSphere boundingSphere;

        public BoundingSphere BoundingSphere
        {
            get { return boundingSphere; }
            set { boundingSphere = value; }
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

            TriangleCount = meshPart.PrimitiveCount;

            if ( TriangleCount * 3 >= int.MaxValue ) throw new Exception( "Too many vertices" );
            int numVertices = TriangleCount * 3;

            if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.Position ) ) positions = new Vector3[ numVertices ];
            if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.Normal ) ) normals = new Vector3[ numVertices ];
            if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.Texcoord, 1 ) ) texCoords = new Vector3[ numVertices ];
            if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.TexTangent, 1 ) ) tangents = new Vector3[ numVertices ];


            for ( int i = 0; i < numVertices; i++ )
            {

                positions[ i ] = meshPart.GetVector3( ColladaMesh.Input.InputSemantics.Position, i ); //GetPosition( i );
                normals[ i ] = meshPart.GetVector3( ColladaMesh.Input.InputSemantics.Normal, i );


                // Texture Coordinates
                Vector3 texcoord = Vector3.Zero;
                if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.Texcoord, 1 ) )
                {
                    texcoord = meshPart.GetVector3( ColladaMesh.Input.InputSemantics.Texcoord, 1, i );
                    texcoord.Y = 1.0f - texcoord.Y; // V coordinate is inverted in max
                }
                texCoords[ i ] = texcoord;

                // Tangent
                Vector3 tangent = Vector3.Zero;
                if ( meshPart.ContainsInput( ColladaMesh.Input.InputSemantics.TexTangent, 1 ) )
                {
                    tangents[ i ] = meshPart.GetVector3( ColladaMesh.Input.InputSemantics.TexTangent, 1, i );
                }
            }

            CalculateBoundingBox();
            CalculateBoundingSphere();

     
            //TODO: Material, maybe not really a good way of doing this
            LoadDataFromColladaMaterial(meshPart.Material);


        }

        /// <summary>
        /// Helper method for storing data. This actually stores shader parameters.
        /// Note: This does not rreaally support material sharing among multiple models
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        private void LoadDataFromColladaMaterial( ColladaMaterial mat )
        {
            MaterialName = mat.Name;

            Ambient = mat.Ambient;
            Diffuse = mat.Diffuse;
            Specular = mat.Specular;
            Shininess = mat.Shininess;

            if ( mat.DiffuseTexture != null )
                DiffuseTexture = mat.DiffuseTexture.GetFullFilename();
            DiffuseTextureRepeatU = mat.DiffuseTextureRepeatU;
            DiffuseTextureRepeatV = mat.DiffuseTextureRepeatV;

            if ( mat.NormalTexture != null )
                NormalTexture = mat.NormalTexture.GetFullFilename();
            NormalTextureRepeatU = mat.NormalTextureRepeatU;
            NormalTextureRepeatV = mat.NormalTextureRepeatV;


        }

        public void CalculateBoundingBox()
        {
            boundingBox = BoundingBox.CreateFromPoints( positions );
        }
        public void CalculateBoundingSphere()
        {
            boundingSphere = BoundingSphere.CreateFromPoints( positions );
        }

    }
}