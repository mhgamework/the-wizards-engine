using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.DirectX11.SlimDXConversion;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Culling;
using MHGameWork.TheWizards.ServerClient;
using SlimDX;

namespace MHGameWork.TheWizards.Rendering
{
    /// <summary>
    /// Maybe keep for testing?
    /// </summary>
    [Obsolete("There is no longer any use for this class. It may reappear later but for now better not use this. See Also DefaultRenderer")]
    public class SimpleRenderer : IXNAObject
    {
        private IXNAGame game;

        private bool initialized;

        private List<ISimpleRenderable> renderables = new List<ISimpleRenderable>();

        private ICuller culler;

        private ICamera cullCamera;
        private ICamera renderCamera;
        public ICamera CullCamera
        {
            get { return cullCamera; }
            set { cullCamera = value; culler.CullCamera = new ConversionCameraDX(cullCamera); }
        }
        public ICamera RenderCamera
        {
            get { return renderCamera; }
            set { renderCamera = value; }
        }

        public SimpleRenderer(IXNAGame game, ICuller culler)
        {
            this.game = game;
            this.culler = culler;
            initialized = false;


        }

        public void Initialize(IXNAGame _game)
        {
            for (int i = 0; i < renderables.Count; i++)
            {
                renderables[i].Initialize(game);
            }

            initialized = true;
        }
        public void Render(IXNAGame _game)
        {
            _game.GraphicsDevice.RenderState.DepthBufferEnable = true;
            ICamera oldCam = game.Camera;
            game.SetCamera(renderCamera);
            for (int i = 0; i < renderables.Count; i++)
            {
                if (renderables[i].VisibleReferenceCount > 0)
                    renderables[i].Render();
            }
            game.SetCamera(oldCam);
        }
        public void Update(IXNAGame _game)
        {
            ICamera oldCam = game.Camera;
            game.SetCamera(renderCamera);
            for (int i = 0; i < renderables.Count; i++)
            {
                if (renderables[i].VisibleReferenceCount > 0)
                    renderables[i].Update();
            }
            culler.UpdateVisibility();
            game.SetCamera(oldCam);
        }

        /// <summary>
        /// Call this when the properties of given renderable were changed. The culler will be updated
        /// </summary>
        /// <param name="renderable"></param>
        public void UpdateRenderable(ISimpleRenderable renderable)
        {
            culler.UpdateCullable(renderable);
        }
        public void AddRenderable(ISimpleRenderable renderable)
        {
            renderables.Add(renderable);
            culler.AddCullable(renderable);

            if (initialized) renderable.Initialize(game);

        }
        public void RemoveRenderable(ISimpleRenderable renderable)
        {
            renderables.Remove(renderable);
            culler.RemoveCullable(renderable);

        }


        public SimpleBoxMesh CreateBoxMesh()
        {
            SimpleBoxMesh mesh = new SimpleBoxMesh();

            AddRenderable(mesh);

            return mesh;
        }
        public SimpleSphereMesh CreateSphereMesh()
        {
            SimpleSphereMesh mesh = new SimpleSphereMesh();

            AddRenderable(mesh);

            return mesh;
        }
        public SimplePlaneMesh CreatePlaneMesh()
        {
            SimplePlaneMesh mesh = new SimplePlaneMesh();

            AddRenderable(mesh);

            return mesh;
        }





    }

    public class SimpleBoxMesh : BoxMesh, ISimpleRenderable
    {

        private IXNAGame game;
        private int visibleReferenceCount;
        BoundingBox ICullable.BoundingBox { get { return base.BoundingBox.dx(); } }

        public int VisibleReferenceCount
        {
            get { return visibleReferenceCount; }
            set { visibleReferenceCount = value; }
        }

        #region ISimpleRenderable Members

        public override void Initialize(IXNAGame game)
        {
            base.Initialize(game);
            this.game = game;

        }

        public void Update()
        {
            Update(game);
        }

        public void Render()
        {
            Render(game);
        }

        #endregion

    }

    public class SimpleSphereMesh : SphereMesh, ISimpleRenderable
    {
        private IXNAGame game;
        private int visibleReferenceCount;
        public int VisibleReferenceCount
        {
            get { return visibleReferenceCount; }
            set { visibleReferenceCount = value; }
        }
        BoundingBox ICullable.BoundingBox { get { return base.BoundingBox.dx(); } }


        #region ISimpleRenderable Members

        public override void Initialize(IXNAGame game)
        {
            base.Initialize(game);
            this.game = game;
        }

        public void Update()
        {
            Update(game);
        }

        public void Render()
        {
            Render(game);
        }

        #endregion

    }
    public class SimplePlaneMesh : PlaneMesh, ISimpleRenderable
    {
        private IXNAGame game;
        private int visibleReferenceCount;
        public int VisibleReferenceCount
        {
            get { return visibleReferenceCount; }
            set { visibleReferenceCount = value; }
        }
        BoundingBox ICullable.BoundingBox { get { return base.BoundingBox.dx(); } }


        #region ISimpleRenderable Members

        public override void Initialize(IXNAGame game)
        {
            base.Initialize(game);
            this.game = game;
        }

        public void Update()
        {
            Update(game);
        }

        public void Render()
        {
            Render(game);
        }

        #endregion

    }
}