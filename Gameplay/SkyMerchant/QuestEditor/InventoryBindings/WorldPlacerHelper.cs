using System;
using DirectX11;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.SkyMerchant.Prototype;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.QuestEditor.InventoryBindings
{
    /// <summary>
    /// Implements placing things in the world using the mouse
    /// Can be used in tools to easily place objects
    /// </summary>
    public class WorldPlacerHelper : IHotbarItem
    {

        private Func<IWorldObject> createNew;
        private float zoom = 10;

        public WorldPlacerHelper(Func<IWorldObject> createNew)
        {
            this.createNew = createNew;
        }

        public string Name { get { return "IslandTool"; } }
        public void OnSelected()
        {

        }

        public void OnDeselected()
        {
        }

        public void Update()
        {
            drawTargetHighlight();
            drawPlacePosition();
            if (TW.Graphics.Mouse.RightMouseJustPressed)
                createNewObject();

            var zoomIncrease = Math.Sign(TW.Graphics.Mouse.RelativeScrollWheel);
            zoom = MathHelper.Clamp(zoom + zoomIncrease * 2, 1, 100);
        }

        private void drawPlacePosition()
        {
            TW.Graphics.LineManager3D.AddCenteredBox(GetPlacePosition(), 1f, new Color4(1, 1, 1));
        }

        private void drawTargetHighlight()
        {

        }

        private void createNewObject()
        {
            var p = createNew();
            p.Postion = GetPlacePosition();
            p.Rotation = GetPlaceOrientation();
        }

        private Vector3 GetPlacePosition()
        {
            var cam = TW.Data.Get<CameraInfo>();
            var camPos = new Vector3(0, 0, -zoom);

            return Vector3.TransformCoordinate(camPos, cam.ActiveCamera.ViewInverse);
        }
        private Quaternion GetPlaceOrientation()
        {

            var forward = TW.Data.Get<CameraInfo>().ActiveCamera.ViewInverse.xna().Right.dx();
            var cos = Vector3.Dot(forward, MathHelper.Right);
            var angle = (float)Math.Acos(cos);
            if (forward.Z < 0) angle = -angle;

            return Quaternion.RotationAxis(MathHelper.Up, -angle);

        }
    }
}