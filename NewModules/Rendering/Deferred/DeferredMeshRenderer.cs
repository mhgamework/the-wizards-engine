using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DirectX11;
using MHGameWork.TheWizards.Common.Core;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.DirectX11.Rendering.Deferred;
using Microsoft.Xna.Framework.Graphics;
using SlimDX;
using SlimDX.Direct3D11;
using BoundingFrustum = Microsoft.Xna.Framework.BoundingFrustum;
using Buffer = SlimDX.Direct3D11.Buffer;
using CullMode = SlimDX.Direct3D11.CullMode;
using DataStream = SlimDX.DataStream;
using FillMode = SlimDX.Direct3D11.FillMode;
using Texture2D = SlimDX.Direct3D11.Texture2D;

namespace MHGameWork.TheWizards.Rendering.Deferred
{
    //TODO: call UpdateCullable in the correct places

    /// <summary>
    /// This class manages a number of IMesh elements and renders them to a GBuffer
    /// Note that this class does not share IMeshPart data, that is parts are not shared across meshes
    /// </summary>
    public class DeferredMeshRenderer
    {
        public static readonly FileInfo DeferredMeshFX = new System.IO.FileInfo(CompiledShaderCache.Current.RootShaderPath +  "Deferred\\DeferredMesh.fx");


        public DeferredMeshRenderer(DX11Game game, GBuffer gBuffer, TexturePool texturePool)
        {
            this.game = game;
            this.gBuffer = gBuffer;
            this.texturePool = texturePool;
            context = game.Device.ImmediateContext;

            initialize();
        }



        private readonly DX11Game game;
        private readonly GBuffer gBuffer;
        private TexturePool texturePool;

        private List<DeferredMeshRenderElement> elements = new List<DeferredMeshRenderElement>();
        private List<MeshRenderData> renderDatas = new List<MeshRenderData>();
        private Dictionary<IMesh, MeshRenderData> renderDataDict = new Dictionary<IMesh, MeshRenderData>();

        private BasicShader baseShader;

        private Texture2D checkerTexture;
        private DeviceContext context;
        private RasterizerState rasterizerState;
        private InputLayout layout;
        private ShaderResourceView checkerTextureRV;


        public List<DeferredMeshRenderElement> Elements
        {
            get { return elements; }
        }

        public FrustumCuller Culler { get; set; }

        public DeferredMeshRenderElement AddMesh(IMesh mesh)
        {

            var el = new DeferredMeshRenderElement(this, mesh);


            var data = getRenderData(mesh);


            el.ElementNumber = data.WorldMatrices.Count;
            data.WorldMatrices.Add(el.WorldMatrix);
            data.Elements.Add(el);


            Elements.Add(el);
            if (Culler != null)
                Culler.AddCullable(el);

            return el;
        }

        public void DeleteMesh(DeferredMeshRenderElement el)
        {
            if (el.IsDeleted) throw new InvalidOperationException();

            if (Culler != null)
                Culler.RemoveCullable(el);

            Elements.Remove(el);


        }

        private MeshRenderData getRenderData(IMesh mesh)
        {
            MeshRenderData ret;
            if (renderDataDict.TryGetValue(mesh, out ret)) return ret;

            ret = new MeshRenderData(mesh);
            renderDataDict[mesh] = ret;
            renderDatas.Add(ret);

            if (game != null)
                initMeshRenderData(ret);


            return ret;
        }

        private void initMeshRenderData(MeshRenderData data)
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

                    renderPart.IndexBuffer = CreateMeshPartIndexBuffer(part.MeshPart);
                    renderPart.VertexBuffer = CreateMeshPartVertexBuffer(part.MeshPart);
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

        public void UpdateWorldMatrix(DeferredMeshRenderElement el)
        {
            renderDataDict[el.Mesh].WorldMatrices[el.ElementNumber] = el.WorldMatrix;

            if (Culler != null)
                Culler.UpdateCullable(el);

        }


        private void initialize()
        {
            rasterizerState = RasterizerState.FromDescription(game.Device, new RasterizerStateDescription
                                                                               {
                                                                                   CullMode = CullMode.None,
                                                                                   FillMode = FillMode.Solid,
                                                                               });

            checkerTexture = Texture2D.FromFile(game.Device, TWDir.GameData.CreateSubdirectory("Core").FullName + "\\checker.png");

            checkerTextureRV = new ShaderResourceView(game.Device, checkerTexture);


            baseShader = BasicShader.LoadAutoreload(game, DeferredMeshFX);
            baseShader.SetTechnique("Technique1");
            //baseShader.DiffuseTexture = checkerTexture;


            layout = new InputLayout(game.Device, baseShader.GetCurrentPass(0).Description.Signature, DeferredMeshVertex.Elements);

            perObjectBuffer = new Buffer(game.Device, new BufferDescription
                                                          {
                                                              BindFlags = BindFlags.ConstantBuffer,
                                                              CpuAccessFlags = CpuAccessFlags.Write,
                                                              OptionFlags = ResourceOptionFlags.None,
                                                              SizeInBytes = 16 * 4, // PerObjectCB
                                                              Usage = ResourceUsage.Dynamic,
                                                              StructureByteStride = 0
                                                          });

            perObjectStrm = new DataStream(baseShader.Effect.GetConstantBufferByName("perObject").ConstantBuffer.Description.SizeInBytes, false, true);
            perObjectBox = new DataBox(0, 0, perObjectStrm);

            for (int i = 0; i < renderDatas.Count; i++)
            {
                initMeshRenderData(renderDatas[i]);
            }
        }

        private struct PerObjectCB
        {
            public Matrix WorldMatrix;
        }

        public void Draw()
        {
            drawCalls = 0;
            Performance.BeginEvent(new Color4(System.Drawing.Color.Red), "BeginDrawDeferredMeshes");
            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            baseShader.Effect.GetVariableByName("View").AsMatrix().SetMatrix(game.Camera.View);
            baseShader.Effect.GetVariableByName("Projection").AsMatrix().SetMatrix(game.Camera.Projection);
            baseShader.Effect.GetConstantBufferByName("perObject").ConstantBuffer = perObjectBuffer;
            //perObjectBuffer = baseShader.Effect.GetConstantBufferByName("perObject").ConstantBuffer;



            baseShader.Apply();
            context.Rasterizer.State = rasterizerState;


            for (int i = 0; i < renderDatas.Count; i++)
            {
                //TODO: use instancing here
                var data = renderDatas[i];
                for (int j = 0; j < data.WorldMatrices.Count; j++)
                {
                    var el = data.Elements[j];
                    if (el.IsDeleted) continue;
                    if (!el.Visible) continue;
                    var mat = data.WorldMatrices[j];
                    renderMesh(data, mat);
                }
            }
            Performance.EndEvent();

            //game.AddToWindowTitle("Calls: " + drawCalls);

        }
        public void DrawShadowCastersDepth()
        {
            drawCalls = 0;
            Performance.BeginEvent(new Color4(System.Drawing.Color.Red), "DMeshes-Depth");
            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            baseShader.Effect.GetVariableByName("View").AsMatrix().SetMatrix(game.Camera.View);
            baseShader.Effect.GetVariableByName("Projection").AsMatrix().SetMatrix(game.Camera.Projection);
            baseShader.Apply();
            context.Rasterizer.State = rasterizerState;

            for (int i = 0; i < renderDatas.Count; i++)
            {
                //TODO: use instancing here
                var data = renderDatas[i];
                for (int j = 0; j < data.WorldMatrices.Count; j++)
                {
                    var el = data.Elements[j];
                    if (el.IsDeleted) continue;
                    if (!el.Visible) continue;
                    if (!el.CastsShadows) continue;
                    var mat = data.WorldMatrices[j];
                    renderMeshDepthOnly(data, mat);
                }
            }
            Performance.EndEvent();

            //game.AddToWindowTitle("Calls: " + drawCalls);

        }

        private int drawCalls;
        private DataStream perObjectStrm;
        private DataBox perObjectBox;
        private Buffer perObjectBuffer;

        private void renderMesh(MeshRenderData data, Matrix world)
        {
            for (int i = 0; i < data.Materials.Length; i++)
            {
                var mat = data.Materials[i];


                for (int j = 0; j < mat.Parts.Length; j++)
                {
                    var part = mat.Parts[j];
                    if (part == null) continue;


                    //context.PixelShader.SetConstantBuffer(mat.PerObjectConstantBuffer, 0);
                    //shaders[i].ViewProjection = game.Camera.ViewProjection;
                    //mat.Shader.Effect.GetVariableByName("World").AsMatrix().SetMatrix(world * part.ObjectMatrix);
                    //mat.Shader.Apply();

                    
                    var box = context.MapSubresource(perObjectBuffer, MapMode.WriteDiscard,
                                           MapFlags.None);
                    box.Data.Write(new PerObjectCB
                                       {
                                           WorldMatrix = Matrix.Transpose(part.ObjectMatrix * world)
                                       });

                    context.UnmapSubresource(perObjectBuffer, 0);

                    context.PixelShader.SetShaderResource(mat.DiffuseTexture, 0);

                    context.InputAssembler.SetIndexBuffer(part.IndexBuffer, SlimDX.DXGI.Format.R32_UInt, 0); //Using int indexbuffers
                    context.InputAssembler.SetVertexBuffers(0,
                                                            new VertexBufferBinding(part.VertexBuffer,
                                                                                    DeferredMeshVertex.SizeInBytes, 0));

                    context.DrawIndexed(part.PrimitiveCount * 3, 0, 0);
                    drawCalls++;
                    Performance.SetMarker(new Color4(System.Drawing.Color.Orange), "DrawMeshElement");


                }
            }
        }


        private void renderMeshDepthOnly(MeshRenderData data, Matrix world)
        {
            for (int i = 0; i < data.Materials.Length; i++)
            {
                var mat = data.Materials[i];


                for (int j = 0; j < mat.Parts.Length; j++)
                {
                    var part = mat.Parts[j];
                    if (part == null) continue;

                    Performance.BeginEvent(new Color4(System.Drawing.Color.Red), "DMesh-Depth");


                    var box = context.MapSubresource(perObjectBuffer, MapMode.WriteDiscard,
                                           MapFlags.None);
                    box.Data.Write(new PerObjectCB
                    {
                        WorldMatrix = Matrix.Transpose(part.ObjectMatrix * world)
                    });

                    context.UnmapSubresource(perObjectBuffer, 0);

                    context.InputAssembler.SetIndexBuffer(part.IndexBuffer, SlimDX.DXGI.Format.R32_UInt, 0); //Using int indexbuffers
                    context.InputAssembler.SetVertexBuffers(0,
                                                            new VertexBufferBinding(part.VertexBuffer,
                                                                                    DeferredMeshVertex.SizeInBytes, 0));

                    context.DrawIndexed(part.PrimitiveCount * 3, 0, 0);
                    drawCalls++;

                    /*baseShader.Effect.GetVariableByName("World").AsMatrix().SetMatrix(world * part.ObjectMatrix);
                    baseShader.Apply();


                    context.InputAssembler.SetIndexBuffer(part.IndexBuffer, SlimDX.DXGI.Format.R32_UInt, 0); //Using int indexbuffers
                    context.InputAssembler.SetVertexBuffers(0,
                                                            new VertexBufferBinding(part.VertexBuffer,
                                                                                    DeferredMeshVertex.SizeInBytes, 0));

                    context.DrawIndexed(part.PrimitiveCount * 3, 0, 0);
                    drawCalls++;*/

                    Performance.EndEvent();

                }
            }
        }



        public Buffer CreateMeshPartVertexBuffer(IMeshPart meshPart)
        {
            var geomData = meshPart.GetGeometryData();
            var positions = geomData.GetSourceVector3(MeshPartGeometryData.Semantic.Position);
            var normals = geomData.GetSourceVector3(MeshPartGeometryData.Semantic.Normal);
            var texcoords = geomData.GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord);
            // This might not work when no texcoords

            var vertices = new DeferredMeshVertex[positions.Length];

            for (int j = 0; j < vertices.Length; j++)
            {
                vertices[j].Pos = new Vector4(positions[j].ToSlimDX(), 1);
                vertices[j].Normal = normals[j].ToSlimDX();
                if (texcoords != null)
                    vertices[j].UV = texcoords[j].ToSlimDX();
                //TODO: tangent
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

        public Buffer CreateMeshPartIndexBuffer(IMeshPart meshPart)
        {
            var geomData = meshPart.GetGeometryData();
            var positions = geomData.GetSourceVector3(MeshPartGeometryData.Semantic.Position);

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


        private class MeshRenderData
        {
            public IMesh Mesh;

            public MeshRenderData(IMesh mesh)
            {
                Mesh = mesh;
            }

            public List<Matrix> WorldMatrices = new List<Matrix>();
            public List<DeferredMeshRenderElement> Elements = new List<DeferredMeshRenderElement>();
            public MeshRenderMaterial[] Materials;

        }

        private class MeshRenderMaterial
        {
            public MeshCoreData.Material Material;

            public ShaderResourceView DiffuseTexture;
            public BasicShader Shader;
            public Buffer PerObjectConstantBuffer;
            public MeshRenderPart[] Parts;
        }

        private class MeshRenderPart
        {
            public Matrix ObjectMatrix;
            public Buffer VertexBuffer;
            public Buffer IndexBuffer;
            public int PrimitiveCount;
            public int VertexCount;
        }


    }
}
