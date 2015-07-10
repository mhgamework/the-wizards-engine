using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Rendering.Deferred.Meshes;
using SlimDX;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    /// <summary>
    /// Responsible for creating MeshRenderData from an IMesh
    /// Responsible for creating meshpart inputassembler data and setting it to the devicecontext
    /// </summary>
    public class MeshRenderDataFactory
    {
        private readonly DX11Game game;

        private ShaderResourceView checkerTextureRV;

        private TexturePool texturePool;
        private Texture2D checkerTexture;
        private BasicShader baseShader;


        public MeshRenderDataFactory(DX11Game game, BasicShader baseShader, TexturePool texturePool)
        {
            this.game = game;
            this.baseShader = baseShader;
            this.texturePool = texturePool;

            checkerTexture = Texture2D.FromFile(game.Device, TWDir.GameData.CreateSubdirectory("Core").FullName + "\\checker.png");

            checkerTextureRV = new ShaderResourceView(game.Device, checkerTexture);
        }

        [TWProfile]
        [Obsolete("This is currently obsolete, it contains too many optimizations. Not everything has been ported.")]
        public void InitMeshRenderData(MeshRenderData data)
        {
            var materials = new List<MeshRenderMaterial>();
            var parts = new Dictionary<MeshRenderMaterial, List<MeshCoreData.Part>>();

            var coreData = data.Mesh.GetCoreData();
            for (int i = 0; i < coreData.Parts.Count; i++)
            {
                var part = coreData.Parts[i];

                var mat = materials.Find(o => o.Material.Equals(part.MeshMaterial));
                if (mat == null)
                {
                    mat = new MeshRenderMaterial();
                    mat.Material = part.MeshMaterial;
                    materials.Add(mat);
                    parts[mat] = new List<MeshCoreData.Part>();
                }
                parts[mat].Add(part);

            }


            data.Materials = new MeshRenderMaterial[materials.Count];

            for (int i = 0; i < materials.Count; i++)
            {
                var renderMat = materials[i];
                data.Materials[i] = renderMat;

                var partList = parts[renderMat];

                renderMat.Parts = new MeshRenderPart[partList.Count];
                //if (partList.Count > 20) Debugger.Break();
                for (int j = 0; j < partList.Count; j++)
                {
                    var part = partList[j];
                    var geomData = part.MeshPart.GetGeometryData();
                    int vertCount = geomData.GetSourceVector3(MeshPartGeometryData.Semantic.Position).Length;
                    if (vertCount == 0) continue;


                    var renderPart = new MeshRenderPart();
                    renderMat.Parts[j] = renderPart;

                    var rawData = geomData.ToRawMeshData();

                    renderPart.IndexBuffer = CreateMeshPartIndexBuffer(rawData.Positions);
                    renderPart.VertexBuffer = CreateMeshPartVertexBuffer(rawData.Positions,rawData.Normals,rawData.Texcoords,rawData.Tangents);
                    renderPart.ObjectMatrix = part.ObjectMatrix.dx();


                    renderPart.VertexCount = vertCount;
                    renderPart.PrimitiveCount = vertCount / 3;

                }
                renderMat.Shader = baseShader.Clone();

                ShaderResourceView diffuseRV = checkerTextureRV;

                if (renderMat.Material.DiffuseMap != null)
                {

                    //var material = new DefaultModelMaterialTextured();

                    diffuseRV = texturePool.LoadTexture(renderMat.Material.DiffuseMap);

                    //material.SetMaterialToShader(renderMat.Shader);
                }
                else
                {
                    //renderMat.Shader.Technique = DefaultModelShader.TechniqueType.Textured;

                }
                renderMat.DiffuseTexture = diffuseRV;
                renderMat.Shader.Effect.GetVariableByName("txDiffuse").AsResource().SetResource(diffuseRV);

            }


        }



        public Buffer CreateMeshPartVertexBuffer(Vector3[] positions, Vector3[] normals, Vector2[] texcoords, Vector3[] tangents)
        {
            // This might not work when no texcoords

            var vertices = new DeferredMeshVertex[positions.Length];

            for (int j = 0; j < vertices.Length; j++)
            {

                // Discretize vertex positions, to decrease rounding errors

                var pos = positions[j];
                pos.X = (int)(pos.X * 1000) * 0.001f;
                pos.Y = (int)(pos.Y * 1000) * 0.001f;
                pos.Z = (int)(pos.Z * 1000) * 0.001f;

                vertices[j].Pos = new Vector4(pos, 1);

                vertices[j].Normal = normals[j];
                if (texcoords != null) vertices[j].UV = texcoords[j];
                if (tangents != null) vertices[j].Tangent = new Vector4(tangents[j], 1);
            }
            Buffer vb;
            using (var strm = new DataStream(vertices, true, false))
            {
                vb = new Buffer(game.Device, strm, new BufferDescription
                {
                    BindFlags = BindFlags.VertexBuffer,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None,
                    SizeInBytes = (int)strm.Length,
                    Usage = ResourceUsage.Immutable
                });
            }

            return vb;
        }

        public Buffer CreateMeshPartIndexBuffer(Vector3[] positions)
        {
            var indices = new int[positions.Length];
            for (int j = 0; j < indices.Length; j++)
                indices[j] = j;
            Buffer ib;
            using (var strm = new DataStream(indices, true, false))
            {
                ib = new Buffer(game.Device, strm, new BufferDescription
                {
                    BindFlags = BindFlags.IndexBuffer,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None,
                    SizeInBytes = (int)strm.Length,
                    Usage = ResourceUsage.Immutable
                });
            }

            return ib;
        }


        public MeshPartRenderData CreateMeshPartData(IMeshPart part)
        {
            return CreateMeshPartData(part.GetGeometryData().ToRawMeshData());
        }
        public MeshPartRenderData CreateMeshPartData(RawMeshData rawMeshData)
        {
            var data = new MeshPartRenderData();

            int vertCount = rawMeshData.Positions.Length;
            if (vertCount == 0) throw new InvalidOperationException();

            data.IndexBuffer = CreateMeshPartIndexBuffer(rawMeshData.Positions);
            data.VertexBuffer = CreateMeshPartVertexBuffer(rawMeshData.Positions, rawMeshData.Normals, rawMeshData.Texcoords, rawMeshData.Tangents);

            //data.VertexCount = vertCount;
            data.PrimitiveCount = vertCount / 3;

            return data;
        }
    }
}
