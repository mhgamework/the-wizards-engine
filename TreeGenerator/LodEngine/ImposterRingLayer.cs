using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework.Graphics;
using TreeGenerator.ImposterRing;
using TreeGenerator.TreeEngine;
using Microsoft.Xna.Framework;

namespace TreeGenerator.LodEngine
{
   public  class ImposterRingLayer:ITreeLodLayer
   {
       private ImposterRing.ImposterRing ring = new ImposterRing.ImposterRing();

       private Dictionary<TreeLodEntity, ImposterRingRenderable> imposterEnitiesIndex = new Dictionary<TreeLodEntity, ImposterRingRenderable>();
       private List<ISimpleRenderable> rendrables = new List<ISimpleRenderable>();

       private SimpleRenderer renderer;
       private FrustumCullerSimple culler;
       private EngineTreeRenderDataGenerater treeRenderGenerater;
       private XNAGame game;
       //private float ringRadius;
       private RenderablesDelayedRenderProvider renderProvider;
       public void initialize(XNAGame game, BoundingBox CullerBoundingBox, int numberOfSplits)
       {
           this.game = game;
            //culler = new FrustumCullerSimple(CullerBoundingBox, numberOfSplits);
           renderer = new SimpleRenderer(game, culler);
           ring.initialize(game);
           renderProvider = new RenderablesDelayedRenderProvider(rendrables, culler);
           ring.renderProvider = renderProvider;
       }
       public void Update()
       {
           ring.Update();//texture is rendered here don't know if this is the most optimal place
       }
       public void Render()
       {
           ring.Render();
       }
       //public void RenderImposter()
       //{
           
       //}

        #region ITreeLodLayer Members

        public bool AddEntity(TreeLodEntity ent)
        {
            EngineTreeRenderData renderData = treeRenderGenerater.GetRenderData(ent.TreeStructure, game, 1);
           ImposterRingRenderable renderable = new ImposterRingRenderable(ent,renderData);
            renderable.Initialize(game);
            renderer.AddRenderable(renderable);
            renderer.UpdateRenderable(renderable);
            rendrables.Add(renderable);
            imposterEnitiesIndex.Add(ent, renderable);
            return true;

        }

        public void RemoveEntity(TreeLodEntity ent)
        {
            ImposterRingRenderable renderable = imposterEnitiesIndex[ent];
            rendrables.Remove(renderable);
            imposterEnitiesIndex.Remove(ent);
            //no delete function for simplerenderer
        }

       private float minDistance;
        public float MinDistance
        {
            get
            {
                return minDistance;
            }
            set
            {
                minDistance=value;
            }
        }

        #endregion

       public ImposterRingLayer(float ringRadius, EngineTreeRenderDataGenerater treeRenderGenerater)
       {
           ring.radius= ringRadius;
           this.treeRenderGenerater = treeRenderGenerater;
       }

       #region ITreeLodLayer Members


       public bool IsFull()
       {
           return false;
       }

       #endregion
   }
   public class ImposterRingRenderable : ISimpleRenderable
   {
       public TreeLodEntity ent;
       private EngineTreeRenderData renderData;

       private BoundingBox boundingBox;
     

       public ImposterRingRenderable(TreeLodEntity ent, EngineTreeRenderData renderData)
       {
           this.ent = ent;
           this.renderData = renderData;
       }

       public void Initialize(MHGameWork.TheWizards.Graphics.IXNAGame game)
        {
            renderData.Initialize();
            boundingBox= BoundingBox.CreateFromPoints(renderData.BoundingBoxData);
            
        }

        public void Render()
        {
            renderData.SetWorldMatrix(ent.WorldMatrix);
            renderData.draw();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        

        #region ICullable Members
       
        public Microsoft.Xna.Framework.BoundingBox BoundingBox
        {
            get { return boundingBox; }
        }

       private int visibleRefrenceCount;
        public int VisibleReferenceCount
        {
            get
            {
                return visibleRefrenceCount;
            }
            set
            {
                visibleRefrenceCount=value;
            }
        }

        #endregion
    }

   public class RenderablesDelayedRenderProvider : IDelayedRenderProvider
   {
       private List<ISimpleRenderable> renderables = new List<ISimpleRenderable>();
       private FrustumCullerSimple culler;
       private IXNAGame game;
       public RenderablesDelayedRenderProvider(List<ISimpleRenderable> _renderables, FrustumCullerSimple culler)
       {
           this.culler = culler;
           this.renderables = _renderables;
       }
       public void StartDelayedRendering(IXNAGame _game, ICamera camera)
       {
           game = _game;
           //culler.CullCamera = camera;
           culler.UpdateVisibility();
           index = -1;
       }

       public bool CanRender()
       {
           return index < renderables.Count;
       }

       private int index = 0;
       public void SingleRender()
       {
           game.GraphicsDevice.RenderState.DepthBufferEnable = true;
           game.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
           if (index == -1)
           {
               game.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.SkyBlue, 1, 0);
               index++;
               return;
           }
           while (renderables[index].VisibleReferenceCount == 0)
           {
               index++;
               if (index == renderables.Count) return;
           }
           renderables[index].Render();
           index++;
       }

       public bool IsRenderingComplete()
       {
           if (index >= renderables.Count)
               return true;

           return false;
       }

   }
}
