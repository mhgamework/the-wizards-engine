// Project: SkinningWithColladaModelsInXna, File: TangentVertex.cs
// Namespace: SkinningWithColladaModelsInXna.Graphics, Class: 
// Path: C:\code\SkinningWithColladaModelsInXna\Graphics, Author: Abi
// Code lines: 195, Size of file: 5,52 KB
// Creation date: 30.08.2006 11:38
// Last modified: 02.09.2006 06:18
// Generated with Commenter by abi.exDream.com

#region Using directives

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace MHGameWork.TheWizards.Graphics.Xna.Graphics.TODO
{
    /// <summary>
    /// TangentVertex, extracted from Abi.Graphic engine for NormalMapCompressor.
    /// More information can be found at:
    /// http://exdream.dyn.ee/blog/PermaLink.aspx?guid=cd2c85b3-13e6-48cd-953e-f7e3bb79fbc5
    /// <para/>
    /// Tangent vertex format for shader vertex format used all over the place.
    /// DirectX9 or XNA does not provide this crazy format ^^ It contains:
    /// Position, Normal vector, texture coords, tangent vector.
    /// </summary>
    public struct TangentVertex
    {
        #region Variables
        /// <summary>
        /// Position
        /// </summary>
        public Vector3 pos;
        /// <summary>
        /// Texture coordinates
        /// </summary>
        public Vector2 uv;
        /// <summary>
        /// Normal
        /// </summary>
        public Vector3 normal;
        /// <summary>
        /// Tangent
        /// </summary>
        public Vector3 tangent;

        /// <summary>
        /// Stride size, in XNA called SizeInBytes. I'm just conforming with that.
        /// Btw: How is this supposed to work without using unsafe AND
        /// without using System.Runtime.InteropServices.Marshal.SizeOf?
        /// </summary>
        public static int SizeInBytes
        {
            get
            {
                // 4 bytes per float:
                // 3 floats pos, 2 floats uv, 3 floats normal and 3 float tangent.
                return 4 * ( 3 + 2 + 3 + 3 );
            } // get
        } // StrideSize

        /// <summary>
        /// U texture coordinate
        /// </summary>
        /// <returns>Float</returns>
        public float U
        {
            get
            {
                return uv.X;
            } // get
        } // U

        /// <summary>
        /// V texture coordinate
        /// </summary>
        /// <returns>Float</returns>
        public float V
        {
            get
            {
                return uv.Y;
            } // get
        } // V
        #endregion

        #region Constructor
        /// <summary>
        /// Create tangent vertex
        /// </summary>
        /// <param name="setPos">Set position</param>
        /// <param name="setU">Set u texture coordinate</param>
        /// <param name="setV">Set v texture coordinate</param>
        /// <param name="setNormal">Set normal</param>
        /// <param name="setTangent">Set tangent</param>
        public TangentVertex(
            Vector3 setPos,
            float setU, float setV,
            Vector3 setNormal,
            Vector3 setTangent )
        {
            pos = setPos;
            uv = new Vector2( setU, setV );
            normal = setNormal;
            tangent = setTangent;
        } // TangentVertex(setPos, setU, setV)

        /// <summary>
        /// Create tangent vertex
        /// </summary>
        /// <param name="setPos">Set position</param>
        /// <param name="setUv">Set uv texture coordinates</param>
        /// <param name="setNormal">Set normal</param>
        /// <param name="setTangent">Set tangent</param>
        public TangentVertex(
            Vector3 setPos,
            Vector2 setUv,
            Vector3 setNormal,
            Vector3 setTangent )
        {
            pos = setPos;
            uv = setUv;
            normal = setNormal;
            tangent = setTangent;
        } // TangentVertex(setPos, setUv, setNormal)
        #endregion

        #region To string
        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return "TangentVertex(pos=" + pos + ", " +
                "u=" + uv.X + ", " +
                "v=" + uv.Y + ", " +
                "normal=" + normal + ", " +
                "tangent=" + tangent + ")";
        } // ToString()
        #endregion

        #region Generate vertex declaration
        /// <summary>
        /// Vertex elements for Mesh.Clone
        /// </summary>
        public static readonly VertexElement[] VertexElements =
            GenerateVertexElements();

        /// <summary>
        /// Vertex declaration for vertex buffers.
        /// </summary>
        //public static VertexDeclaration VertexDeclaration = CreateVertexDeclaration( TestServerClientMain.instance.XNAGame.GraphicsDevice );
        //new VertexDeclaration(  VertexElements );

        public static VertexDeclaration CreateVertexDeclaration( GraphicsDevice device )
        {
            return new VertexDeclaration( device, VertexElements );
        }

        /// <summary>
        /// Generate vertex declaration
        /// </summary>
        private static VertexElement[] GenerateVertexElements()
        {
            VertexElement[] decl = new VertexElement[]
				{
					// Construct new vertex declaration with tangent info
					// First the normal stuff (we should already have that)
					new VertexElement(0, 0, VertexElementFormat.Vector3,
						VertexElementMethod.Default, VertexElementUsage.Position, 0),
					new VertexElement(0, 12, VertexElementFormat.Vector2,
						VertexElementMethod.Default, VertexElementUsage.TextureCoordinate,
						0),
					new VertexElement(0, 20, VertexElementFormat.Vector3,
						VertexElementMethod.Default, VertexElementUsage.Normal, 0),
					// And now the tangent
					new VertexElement(0, 32, VertexElementFormat.Vector3,
						VertexElementMethod.Default, VertexElementUsage.Tangent, 0),
				};
            return decl;
        } // GenerateVertexElements()
        #endregion

        /// <summary>
        /// Returns true if two vertices are nearly equal. For example the
        /// tangent or normal data does not have to match 100%.
        /// Used to optimize vertex buffers and to generate indices.
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <returns>Bool</returns>
        public static bool NearlyEquals( TangentVertex a,
           TangentVertex b )
        {

            //SkinningWithColladaModelsInXna.Helpers.Log.Write("Compare a=" + a.pos + ", " + a.uv + ", "+a.normal+
            //  " with b=" + b.pos + ", " + b.uv+ ", "+b.normal);
            //return false;
            // Position has to match, else it is just different vertex
            return a.pos == b.pos &&
                // Ignore blend indices and blend weights, they are the same
                // anyway, because they are calculated from the bone distances.
                Math.Abs( a.uv.X - b.uv.X ) < 0.001f &&
                Math.Abs( a.uv.Y - b.uv.Y ) < 0.001f &&
                // Normals and tangents do not have to be very close, we can't see
                // any difference between small variations here, but by optimizing
                // similar vertices we can improve the overall rendering performance.
                ( a.normal - b.normal ).Length() < 0.1f &&
                ( a.tangent - b.tangent ).Length() < 0.1f;
            //SkinningWithColladaModelsInXna.Helpers.Log.Write("ret=" + ret);
            //			return ret;
        } // NearlyEqual(a, b)

        #region Is declaration tangent vertex declaration
        /// <summary>
        /// Returns true if declaration is tangent vertex declaration.
        /// </summary>
        public static bool IsTangentVertexDeclaration(
            VertexElement[] declaration )
        {
            return
                declaration.Length == 4 &&
                declaration[ 0 ].VertexElementUsage == VertexElementUsage.Position &&
                declaration[ 1 ].VertexElementUsage ==
                VertexElementUsage.TextureCoordinate &&
                declaration[ 2 ].VertexElementUsage == VertexElementUsage.Normal &&
                declaration[ 3 ].VertexElementUsage == VertexElementUsage.Tangent;
        } // IsTangentVertexDeclaration(declaration)
        #endregion
    } // struct TangentVertex
} // namespace SkinningWithColladaModelsInXna.Graphics
