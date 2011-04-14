using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.ServerClient.Editor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Entities
{
    /// <summary>
    /// This class contains All data in the engine of a specific model. It is not meant for real-time
    /// use but only for editor/preprocessor. It may contain redundant data
    /// </summary>
    public class ModelFullData
    {

        public ModelFullData()
        {
            geometry = new ModelGeometryData();
        }

        /**
         * 
         * Graphical Data:
         * Mesh
         * 
         **/

        private ModelGeometryData geometry;

        public ModelGeometryData Geometry
        {
            [DebuggerStepThrough]
            get { return geometry; }
            [DebuggerStepThrough]
            set { geometry = value; }
        }


        public Microsoft.Xna.Framework.Vector3[] Positions
        {
            get { return geometry.Positions; }
            set { geometry.Positions = value; }
        }


        public Vector3[] Normals
        {
            get { return geometry.Normals; }
            set { geometry.Normals = value; }
        }


        public Vector3[] Tangents
        {
            get { return geometry.Tangents; }
            set { geometry.Tangents = value; }
        }


        public Vector3[] TexCoords
        {
            get { return geometry.Positions; }
            set { geometry.TexCoords = value; }
        }



        public int TriangleCount
        {
            get { return geometry.TriangleCount; }
            set { geometry.TriangleCount = value; }
        }

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



   




        public void CalculateBoundingBox()
        {
            boundingBox = BoundingBox.CreateFromPoints( geometry.Positions );
        }
        public void CalculateBoundingSphere()
        {
            boundingSphere = BoundingSphere.CreateFromPoints( geometry.Positions );
        }

    }
}