// Project: SkinningWithColladaModelsInXna, File: SkinnedTangentVertex.cs
// Namespace: SkinningWithColladaModelsInXna.Graphics, Class: 
// Path: C:\code\Xna\SkinningWithColladaModelsInXna\Graphics, Author: abi
// Code lines: 227, Size of file: 6,87 KB
// Creation date: 20.02.2007 12:33
// Last modified: 20.02.2007 12:41
// Generated with Commenter by abi.exDream.com

#region Using directives

using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MHGameWork.TheWizards.ServerClient.Graphics;
#endregion

namespace MHGameWork.TheWizards.ServerClient.Engine
{
    /// <summary>
    /// Skinned tangent vertex, similar to TangentVertex, but adds the
    /// ability to skin vertices with bones :)
    /// </summary>
    public struct SkinnedTangentVertex
    {
        #region Variables
        /// <summary>
        /// Position
        /// </summary>
        [VertexElementAttribute(VertexElementUsage.Position)]
        public Vector3 pos;

        /// <summary>
        /// 3 weights of each bone (often just 1, 0, 0 to use only 1 bone)
        /// </summary>
        [VertexElementAttribute(VertexElementUsage.BlendWeight)]
        public Vector3 blendWeights;
        /// <summary>
        /// Indices for used bones. Just floats because shader expect that and
        /// we can't use UByte4 for lower than vs_2_0 hardware. Uses just int
        /// values really.
        /// </summary>
        [VertexElementAttribute(VertexElementUsage.BlendIndices)]
        public Vector3 blendIndices;

        /// <summary>
        /// Texture coordinates
        /// </summary>
        [VertexElementAttribute(VertexElementUsage.TextureCoordinate)]
        public Vector2 uv;
        /// <summary>
        /// Normal
        /// </summary>
        [VertexElementAttribute(VertexElementUsage.Normal)]
        public Vector3 normal;
        /// <summary>
        /// Tangent
        /// </summary>
        [VertexElementAttribute(VertexElementUsage.Tangent)]
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
                // 3 floats pos, 3 for the blend weights and 3 for the blend indices,
                // 2 floats uv, 3 floats normal and 3 float tangent.
                return 4 * (3 + 3 + 3 + 2 + 3 + 3);
            } // get
        } // SizeInBytes

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
        /// Create skinned tangent vertex
        /// </summary>
        /// <param name="setPos">Set position</param>
        /// <param name="setBlendWeights">Set blend weights</param>
        /// <param name="setBlendIndices">Set blend indices</param>
        /// <param name="setU">Set u texture coordinate</param>
        /// <param name="setV">Set v texture coordinate</param>
        /// <param name="setNormal">Set normal</param>
        /// <param name="setTangent">Set tangent</param>
        public SkinnedTangentVertex(
            Vector3 setPos,
            Vector3 setBlendWeights,
            Vector3 setBlendIndices,
            float setU, float setV,
            Vector3 setNormal,
            Vector3 setTangent)//, Vector3 setBinormal)
        {
            pos = setPos;
            blendWeights = setBlendWeights;
            blendIndices = setBlendIndices;
            uv = new Vector2(setU, setV);
            normal = setNormal;
            tangent = setTangent;
        } // SkinnedTangentVertex(setPos, setBlendWeights, setBlendIndices)

        /// <summary>
        /// Create tangent vertex
        /// </summary>
        /// <param name="setPos">Set position</param>
        /// <param name="setBlendWeights">Set blend weights</param>
        /// <param name="setBlendIndices">Set blend indices</param>
        /// <param name="setUv">Set uv texture coordinates</param>
        /// <param name="setNormal">Set normal</param>
        /// <param name="setTangent">Set tangent</param>
        public SkinnedTangentVertex(
            Vector3 setPos,
            Vector3 setBlendWeights,
            Vector3 setBlendIndices,
            Vector2 setUv,
            Vector3 setNormal,
            Vector3 setTangent)
        {
            pos = setPos;
            blendWeights = setBlendWeights;
            blendIndices = setBlendIndices;
            uv = setUv;
            normal = setNormal;
            tangent = setTangent;
        } // SkinnedTangentVertex(setPos, setBlendWeights, setBlendIndices)
        #endregion

        #region To string
        /// <summary>
        /// To string
        /// </summary>
        public override string ToString()
        {
            return "SkinnedTangentVertex(pos=" + pos + ", " +
                "blendWeights=" + blendWeights + ", " +
                "blendIndices=" + blendIndices + ", " +
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
        /*/// <summary>
        /// Vertex declaration for vertex buffers.
        /// </summary>
        public static readonly VertexDeclaration VertexDeclaration =
            new VertexDeclaration( ServerClientMainOud.instance.XNAGame.GraphicsDevice, VertexElements );*/
        public static VertexDeclaration CreateVertexDeclaration(IXNAGame game)
        {
            return new VertexDeclaration(game.GraphicsDevice, VertexElements);
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
					new VertexElement(0, 12, VertexElementFormat.Vector3,
						VertexElementMethod.Default, VertexElementUsage.BlendWeight, 0),
					new VertexElement(0, 24, VertexElementFormat.Vector3,
						VertexElementMethod.Default, VertexElementUsage.BlendIndices, 0),
					new VertexElement(0, 36, VertexElementFormat.Vector2,
						VertexElementMethod.Default, VertexElementUsage.TextureCoordinate,
						0),
					new VertexElement(0, 44, VertexElementFormat.Vector3,
						VertexElementMethod.Default, VertexElementUsage.Normal, 0),
					// And now the tangent
					new VertexElement(0, 56, VertexElementFormat.Vector3,
						VertexElementMethod.Default, VertexElementUsage.Tangent, 0),
				};
            return decl;
        } // GenerateVertexElements()
        #endregion

        #region Is decl tangent vertex declaration
        /// <summary>
        /// Returns true if decl is tangent vertex declaration.
        /// </summary>
        public static bool IsSkinnedTangentVertexDeclaration(
            VertexElement[] declaration)
        {
            return
                declaration.Length == 6 &&
                declaration[0].VertexElementUsage == VertexElementUsage.Position &&
                declaration[1].VertexElementUsage == VertexElementUsage.BlendWeight &&
                declaration[2].VertexElementUsage == VertexElementUsage.BlendIndices &&
                declaration[3].VertexElementUsage ==
                VertexElementUsage.TextureCoordinate &&
                declaration[4].VertexElementUsage == VertexElementUsage.Normal &&
                declaration[5].VertexElementUsage == VertexElementUsage.Tangent;
        } // IsSkinnedTangentVertexDeclaration(declaration)
        #endregion

        #region Nearly equal
        /// <summary>
        /// Returns true if two vertices are nearly equal. For example the
        /// tangent or normal data does not have to match 100%.
        /// Used to optimize vertex buffers and to generate indices.
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <returns>Bool</returns>
        public static bool NearlyEquals(SkinnedTangentVertex a,
            SkinnedTangentVertex b)
        {
            //SkinningWithColladaModelsInXna.Helpers.Log.Write("Compare a=" + a.pos + ", " + a.uv + ", "+a.normal+
            //  " with b=" + b.pos + ", " + b.uv+ ", "+b.normal);
            //return false;
            // Position has to match, else it is just different vertex
            return a.pos == b.pos &&
                // Ignore blend indices and blend weights, they are the same
                // anyway, because they are calculated from the bone distances.
                Math.Abs(a.uv.X - b.uv.X) < 0.001f &&
                Math.Abs(a.uv.Y - b.uv.Y) < 0.001f &&
                // Normals and tangents do not have to be very close, we can't see
                // any difference between small variations here, but by optimizing
                // similar vertices we can improve the overall rendering performance.
                (a.normal - b.normal).Length() < 0.1f &&
                (a.tangent - b.tangent).Length() < 0.1f;
            //SkinningWithColladaModelsInXna.Helpers.Log.Write("ret=" + ret);
            //			return ret;
        } // NearlyEqual(a, b)
        #endregion
    } // struct SkinnedTangentVertex
    
}
