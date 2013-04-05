using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.DirectX11.Rendering.Deferred;
using SlimDX;
using SlimDX.Direct3D11;
using CullMode = SlimDX.Direct3D11.CullMode;
using FillMode = SlimDX.Direct3D11.FillMode;

namespace MHGameWork.TheWizards.Rendering.Deferred.Meshes
{
    //TODO: call UpdateCullable in the correct places

    /// <summary>
    /// This class is responsible for the mesh subpart of the deferred renderer facade. 
    /// It provides MeshRenderElements that expose the functionality offered by the renderer.
    /// </summary>
    public class DeferredMeshesRenderer
    {
        private MeshRenderDataFactory renderDataFactory;

        private List<MeshElementPart> parts = new List<MeshElementPart>();


        public DeferredMeshesRenderer(DX11Game game, GBuffer gBuffer, TexturePool texturePool)
        {
            this.game = game;
            this.gBuffer = gBuffer;
            this.pool = texturePool;
            ctx = game.Device.ImmediateContext;

            initialize(texturePool);
        }



        private readonly DX11Game game;
        private readonly GBuffer gBuffer;
        private readonly TexturePool pool;


        private List<DeferredMeshElement> elements = new List<DeferredMeshElement>();

        private BasicShader baseShader;


        private DeviceContext ctx;
        private RasterizerState rasterizerState;
        private InputLayout layout;


        public List<DeferredMeshElement> Elements
        {
            get { return elements; }
        }

        public FrustumCuller Culler { get; set; }

        public int DrawCalls
        {
            get { return drawCalls; }
        }

        public RasterizerState RasterizerState
        {
            get { return rasterizerState; }
            set { rasterizerState = value; }
        }

        private Dictionary<IMesh, MeshRenderData> renderDataDict = new Dictionary<IMesh, MeshRenderData>();
        private List<MeshRenderData> renderDatas = new List<MeshRenderData>();

        private MeshRenderData getRenderData(IMesh mesh)
        {
            MeshRenderData ret;
            if (renderDataDict.TryGetValue(mesh, out ret)) return ret;

            ret = new MeshRenderData(mesh);
            renderDataDict[mesh] = ret;
            renderDatas.Add(ret);

            if (game != null)
                renderDataFactory.InitMeshRenderData(ret);


            return ret;
        }


        public DeferredMeshElement AddMesh(IMesh mesh)
        {
            var el = new DeferredMeshElement(this, mesh);
            //var data = getRenderData(mesh);

            //el.ElementNumber = data.WorldMatrices.Count;
            //data.WorldMatrices.Add(el.WorldMatrix);
            //data.Elements.Add(el);

            createParts(el);
            Elements.Add(el);
            //if (Culler != null)
            //Culler.AddCullable(el);

            return el;
        }

        private void createParts(DeferredMeshElement el)
        {
            foreach (var p in el.Mesh.GetCoreData().Parts)
            {
                var mat = createMaterial(p);
                var data = createRenderData(p.MeshPart);

                var part = new MeshElementPart(mat, data);
                part.CreateBuffers(game);
                part.perObject.UpdatePerObjectBuffer(ctx, p.ObjectMatrix.ToSlimDX() * el.WorldMatrix);
                parts.Add(part);
            }
        }

        private MeshPartRenderData createRenderData(IMeshPart part)
        {
            return renderDataFactory.CreateMeshPartData(part);
        }

        private DeferredMaterial createMaterial(MeshCoreData.Part part)
        {
            var diffuse = part.MeshMaterial.DiffuseMap;
            var normal = part.MeshMaterial.NormalMap;
            var specular = part.MeshMaterial.SpecularMap;


            var txDiffuse = diffuse == null ? null : pool.LoadTexture(diffuse);
            var txNormal = normal == null ? null : pool.LoadTexture(normal);
            var txSpecular = specular == null ? null : pool.LoadTexture(specular);


            return new DeferredMaterial(game, txDiffuse, txNormal, txSpecular);

        }

        public void DeleteMesh(DeferredMeshElement el)
        {
            if (el.IsDeleted) throw new InvalidOperationException();

            /*if (Culler != null)
                Culler.RemoveCullable(el);*/ // culling is disabled

            Elements.Remove(el);


        }

        public void UpdateWorldMatrix(DeferredMeshElement el)
        {
            renderDataDict[el.Mesh].WorldMatrices[el.ElementNumber] = el.WorldMatrix;

            if (Culler != null)
                Culler.UpdateCullable(el);

        }


        private void initialize(TexturePool texturePool)
        {
            RasterizerState = RasterizerState.FromDescription(game.Device, new RasterizerStateDescription
                                                                               {
                                                                                   CullMode = CullMode.Back,
                                                                                   FillMode = FillMode.Solid,
                                                                               });

            renderDataFactory = new MeshRenderDataFactory(game, baseShader, texturePool);




        }




        public void Draw() { drawInternal(false); }
        //TODO: optimize depth
        public void DrawShadowCastersDepth() { drawInternal(true); }

        private void drawInternal(bool depthOnly)
        {


            ctx.Rasterizer.State = rasterizerState;
            foreach (var p in parts)
            {
                p.Render(game.Camera, ctx);
            }



            //Performance.BeginEvent(new Color4(System.Drawing.Color.Red), "BeginDrawDeferredMeshes");
            //ctx.InputAssembler.InputLayout = layout;


            //baseShader.Effect.GetVariableByName("View").AsMatrix().SetMatrix(game.Camera.View);
            //baseShader.Effect.GetVariableByName("Projection").AsMatrix().SetMatrix(game.Camera.Projection);
            //if (!depthOnly) baseShader.Effect.GetConstantBufferByName("perObject").ConstantBuffer = perObjectBuffer;
            //perObjectBuffer = baseShader.Effect.GetConstantBufferByName("perObject").ConstantBuffer;


            //baseShader.Apply();
            context.Rasterizer.State = RasterizerState;



            //foreach (var el in Elements)
            //{
            //    // Test if it should be rendered
            //    if (el.IsDeleted) continue;
            //    if (!el.Visible) continue;
            //    if (depthOnly && !el.CastsShadows) continue;


            //    foreach ( var mat in  renderDataDict[ el.Mesh ].Materials )
            //    {
            //        foreach ( var part in mat.Parts )
            //        {


            //            renderMeshPart( el.WorldMatrix,mat,part );
            //        }
            //    }


            //}
            //Performance.EndEvent();
        }



        private int drawCalls;






    }
}
