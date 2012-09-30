using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Common.Core;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering.Default;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// This is the location to do instancing, or other hardware speedups for multiple mesh rendering (make this an abstract factory)
    /// Currently this is a simple implementation
    /// </summary>
    public class SimpleMeshRenderer : IXNAObject, ISimpleMeshRenderer
    {
        private TexturePool texturePool;
        private MeshPartPool meshPartPool;
        private VertexDeclarationPool vertexDeclarationPool;
        private IXNAGame game;

        private List<SimpleMeshRenderElement> elements = new List<SimpleMeshRenderElement>();
        private List<MeshRenderData> renderDatas = new List<MeshRenderData>();
        private Dictionary<IMesh, MeshRenderData> renderDataDict = new Dictionary<IMesh, MeshRenderData>();

        private DefaultModelShader baseShader;

        private Texture2D checkerTexture;

        public SimpleMeshRenderer(TexturePool texturePool, MeshPartPool meshPartPool, VertexDeclarationPool vertexDeclarationPool)
        {
            this.texturePool = texturePool;
            this.meshPartPool = meshPartPool;
            this.vertexDeclarationPool = vertexDeclarationPool;
        }


        public SimpleMeshRenderElement AddMesh(IMesh mesh)
        {

            var el = new SimpleMeshRenderElement(this, mesh);


            var data = getRenderData(mesh);


            el.ElementNumber = data.WorldMatrices.Count;
            data.WorldMatrices.Add(el.WorldMatrix);
            data.ElementDeleted.Add(false);


            elements.Add(el);

            return el;
        }

        public void DeleteMesh(SimpleMeshRenderElement el)
        {
            if (el.IsDeleted) throw new InvalidOperationException();

            var data = getRenderData(el.Mesh);


            data.ElementDeleted[el.ElementNumber] = true;


            elements.Remove(el);

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
                for (int j = 0; j < partList.Count; j++)
                {
                    var part = partList[j];
                    var renderPart = new MeshRenderPart();
                    renderMat.Parts[j] = renderPart;

                    renderPart.IndexBuffer = meshPartPool.GetIndexBuffer(part.MeshPart);
                    renderPart.VertexBuffer = meshPartPool.GetVertexBuffer(part.MeshPart);
                    renderPart.ObjectMatrix = part.ObjectMatrix;

                    var geomData = part.MeshPart.GetGeometryData();
                    int vertCount = geomData.GetSourceVector3(MeshPartGeometryData.Semantic.Position).Length;

                    renderPart.VertexCount = vertCount;
                    renderPart.PrimitiveCount = vertCount / 3;

                }
                renderMat.Shader = baseShader.Clone();

                if (renderMat.Material.DiffuseMap != null)
                {
                    var material = new DefaultModelMaterialTextured();
                    material.DiffuseTexture = texturePool.LoadTexture(renderMat.Material.DiffuseMap);

                    material.SetMaterialToShader(renderMat.Shader);
                }
                else
                {
                    renderMat.Shader.DiffuseTexture = checkerTexture;
                    renderMat.Shader.Technique = DefaultModelShader.TechniqueType.Textured;

                }
            }


        }

        public void UpdateWorldMatrix(SimpleMeshRenderElement el)
        {
            renderDataDict[el.Mesh].WorldMatrices[el.ElementNumber] = el.WorldMatrix;
        }


        public void Initialize(IXNAGame _game)
        {
            game = _game;

            checkerTexture = Texture2D.FromFile(game.GraphicsDevice,
                                               EmbeddedFile.GetStream(
                                                   "MHGameWork.TheWizards.Rendering.Files.Checker.png", "Checker.png"));


            baseShader = new DefaultModelShader(_game, new EffectPool());
            baseShader.LightColor = new Vector3(0.9f, 0.9f, 0.9f);
            baseShader.AmbientColor = new Vector4(0.15f, 0.15f, 0.15f, 1f);
            baseShader.Shininess = 100;
            baseShader.SpecularColor = Color.Black.ToVector4();
            baseShader.LightDirection = Vector3.Normalize(new Vector3(1, -5, 1));
            baseShader.DiffuseTexture = checkerTexture;



            for (int i = 0; i < renderDatas.Count; i++)
            {
                initMeshRenderData(renderDatas[i]);
            }
        }

        public void Render(IXNAGame _game)
        {
            game.GraphicsDevice.RenderState.CullMode = CullMode.CullClockwiseFace;
            game.GraphicsDevice.VertexDeclaration = vertexDeclarationPool.GetVertexDeclaration<TangentVertex>();
            baseShader.ViewProjection = game.Camera.ViewProjection;


            for (int i = 0; i < renderDatas.Count; i++)
            {
                var data = renderDatas[i];
                for (int j = 0; j < data.WorldMatrices.Count; j++)
                {
                    var mat = data.WorldMatrices[j];
                    if (data.ElementDeleted[j]) continue;
                    renderMesh(data, mat);
                }
            }
        }

        private void renderMesh(MeshRenderData data, Matrix world)
        {
            for (int i = 0; i < data.Materials.Length; i++)
            {
                var mat = data.Materials[i];
                for (int j = 0; j < mat.Parts.Length; j++)
                {
                    var part = mat.Parts[j];

                    var shader = mat.Shader;


                    //shaders[i].ViewProjection = game.Camera.ViewProjection;
                    shader.World = world * part.ObjectMatrix;
                    shader.DrawPrimitives(delegate
                    {
                        game.GraphicsDevice.Vertices[0].SetSource(part.VertexBuffer, 0,
                                                                  TangentVertex.SizeInBytes);
                        game.GraphicsDevice.Indices = part.IndexBuffer;

                        game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, part.VertexCount, 0,
                                                                  part.PrimitiveCount);
                    }
                        );


                }
            }
        }

        public void Update(IXNAGame _game)
        {
        }

        private class MeshRenderData
        {
            public IMesh Mesh;

            public MeshRenderData(IMesh mesh)
            {
                Mesh = mesh;
            }

            public List<Matrix> WorldMatrices = new List<Matrix>();
            public List<bool> ElementDeleted = new List<bool>();
            public MeshRenderMaterial[] Materials;

        }

        private class MeshRenderMaterial
        {
            public MeshCoreData.Material Material;


            public DefaultModelShader Shader;
            public MeshRenderPart[] Parts;
        }

        private class MeshRenderPart
        {
            public Matrix ObjectMatrix;
            public VertexBuffer VertexBuffer;
            public IndexBuffer IndexBuffer;
            public int PrimitiveCount;
            public int VertexCount;
        }


    }
}
