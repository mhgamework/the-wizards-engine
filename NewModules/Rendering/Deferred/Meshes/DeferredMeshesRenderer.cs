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
    /// This class is responsible for the mesh subpart of the deferred renderer facade. 
    /// It provides MeshRenderElements that expose the functionality offered by the renderer.
    /// </summary>
    public class DeferredMeshesRenderer
    {
        private MeshRenderDataFactory renderDataFactory;


        public DeferredMeshesRenderer(DX11Game game, GBuffer gBuffer, TexturePool texturePool)
        {
            this.game = game;
            this.gBuffer = gBuffer;
            context = game.Device.ImmediateContext;

            initialize(texturePool);
        }



        private readonly DX11Game game;
        private readonly GBuffer gBuffer;


        private List<DeferredRendererMeshes> elements = new List<DeferredRendererMeshes>();

        private BasicShader baseShader;


        private DeviceContext context;
        private RasterizerState rasterizerState;
        private InputLayout layout;


        public List<DeferredRendererMeshes> Elements
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


        public DeferredRendererMeshes AddMesh(IMesh mesh)
        {
            var el = new DeferredRendererMeshes(this, mesh);
            var data = getRenderData(mesh);

            el.ElementNumber = data.WorldMatrices.Count;
            data.WorldMatrices.Add(el.WorldMatrix);
            data.Elements.Add(el);


            Elements.Add(el);
            if (Culler != null)
                Culler.AddCullable(el);

            return el;
        }

        public void DeleteMesh(DeferredRendererMeshes el)
        {
            if (el.IsDeleted) throw new InvalidOperationException();

            /*if (Culler != null)
                Culler.RemoveCullable(el);*/ // culling is disabled

            Elements.Remove(el);


        }

        public void UpdateWorldMatrix(DeferredRendererMeshes el)
        {
            renderDataDict[el.Mesh].WorldMatrices[el.ElementNumber] = el.WorldMatrix;

            if (Culler != null)
                Culler.UpdateCullable(el);

        }


        private void initialize(TexturePool texturePool)
        {
            RasterizerState = RasterizerState.FromDescription(game.Device, new RasterizerStateDescription
                                                                               {
                                                                                   CullMode = CullMode.None,
                                                                                   FillMode = FillMode.Solid,
                                                                               });

            renderDataFactory = new MeshRenderDataFactory(game, baseShader, texturePool);


     

        }

    


        public void Draw() { drawInternal(false); }
        public void DrawShadowCastersDepth() {throw new NotImplementedException(); drawInternal(true); }

        private void drawInternal(bool depthOnly)
        {
            drawCalls = 0;
            Performance.BeginEvent(new Color4(System.Drawing.Color.Red), "BeginDrawDeferredMeshes");
            //context.InputAssembler.InputLayout = layout;
            

            //baseShader.Effect.GetVariableByName("View").AsMatrix().SetMatrix(game.Camera.View);
            //baseShader.Effect.GetVariableByName("Projection").AsMatrix().SetMatrix(game.Camera.Projection);
            //if (!depthOnly) baseShader.Effect.GetConstantBufferByName("perObject").ConstantBuffer = perObjectBuffer;
            //perObjectBuffer = baseShader.Effect.GetConstantBufferByName("perObject").ConstantBuffer;


            baseShader.Apply();
            context.Rasterizer.State = RasterizerState;


            foreach (var el in Elements)
            {
                // Test if it should be rendered
                if (el.IsDeleted) continue;
                if (!el.Visible) continue;
                if (depthOnly && !el.CastsShadows) continue;


                foreach ( var mat in  renderDataDict[ el.Mesh ].Materials )
                {
                    foreach ( var part in mat.Parts )
                    {
                        renderMeshPart( el.WorldMatrix,mat,part );
                    }
                }


            }
            Performance.EndEvent();
        }



        private int drawCalls;
        
        private void renderMeshPart(Matrix world, MeshRenderMaterial mat, MeshRenderPart part)
        {
            //updatePerObjectBuffer(part, world);

            throw new NotImplementedException();
            //setMaterial( mat );
            //setIndexAndVertexBuffer(part);
            //drawIndexed(part);

            Performance.SetMarker(new Color4(System.Drawing.Color.Orange), "DrawMeshElement");
        }

            


    }
}
