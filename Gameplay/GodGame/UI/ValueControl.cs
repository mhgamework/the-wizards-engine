using System;
using System.Drawing;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.Scattered._Engine;
using SlimDX;
using DirectX11;

namespace MHGameWork.TheWizards.GodGame.UI
{
    /// <summary>
    /// TODO: maybe build a tree, where a value control consists of 2 buttons
    /// TODO: use scene graph
    /// </summary>
    public class ValueControl : IRenderable
    {
        //TODO: public BoundingBox BoundingBox { get; set; }
        public Matrix WorldMatrix { get; set; }

        private BoundingBox plusBox;
        private BoundingBox minBox;

        public int Value { get; set; }
        public int MaxValue { get; set; }

        public ValueControl()
        {
            MaxValue = 10;
            minBox = new Vector3(-1, 0, 0).ToBoundingBox(new Vector3(0.7f));
            plusBox = new Vector3(1, 0, 0).ToBoundingBox(new Vector3(0.7f));
            WorldMatrix = Matrix.Identity;

            initRendering();
        }

        public bool TryLeftClick(Ray ray)
        {
            return tryChangeValue(ray, 1);
        }
        public bool TryRightClick(Ray ray)
        {
            return tryChangeValue(ray, 5);
        }

        private bool tryChangeValue(Ray ray, int amount)
        {
            if (intersectsBox(ray, plusBox))
            {
                Value += amount;
                Value = (int)MathHelper.Clamp(Value, 0, MaxValue);
                return true;
            }
            if (intersectsBox(ray, minBox))
            {
                Value -= amount;
                Value = (int)MathHelper.Clamp(Value, 0, MaxValue);
                return true;
            }

            return false;
        }

        private bool intersectsBox(Ray ray, BoundingBox box)
        {
            var localRay = ray.Transform(Matrix.Invert(WorldMatrix));
            if (!localRay.xna().Intersects(box.xna()).HasValue) return false;
            return true;
        }



        private TextRectangle text;
        private Entity plusBoxEnt;
        private Entity minBoxEnt;

        public void initRendering()
        {
            text = new TextRectangle();
            text.Radius = 0.7f;
            text.IsBillboard = true;
            plusBoxEnt = new Entity();
            minBoxEnt = new Entity();

            plusBoxEnt.Mesh = UtilityMeshes.CreateMeshWithText(0.5f, "+", TW.Graphics);
            minBoxEnt.Mesh = UtilityMeshes.CreateMeshWithText(0.5f, "-", TW.Graphics);

            Update();
            Hide();

        }
        public void Show()
        {
            text.Entity.Visible = true;
            plusBoxEnt.Visible = true;
            minBoxEnt.Visible = true;
        }

        public void Update()
        {
            text.Text = Value.ToString();
            text.Position = WorldMatrix.GetTranslation() + new Vector3(0, 1.5f, 0);

            text.Update();

            plusBoxEnt.WorldMatrix = Matrix.Scaling(plusBox.GetSize()) *
                                  Matrix.Translation(plusBox.GetCenter()) * WorldMatrix;
            minBoxEnt.WorldMatrix = Matrix.Scaling(minBox.GetSize()) *
                                  Matrix.Translation(minBox.GetCenter()) * WorldMatrix;

            var old = TW.Graphics.LineManager3D.WorldMatrix;
            TW.Graphics.LineManager3D.WorldMatrix = WorldMatrix;
            if (intersectsBox(TW.Data.Get<CameraInfo>().GetCenterScreenRay(), minBox))
                TW.Graphics.LineManager3D.AddBox(minBox, Color.Yellow.dx());
            if (intersectsBox(TW.Data.Get<CameraInfo>().GetCenterScreenRay(), plusBox))
                TW.Graphics.LineManager3D.AddBox(plusBox, Color.Yellow.dx());
            TW.Graphics.LineManager3D.WorldMatrix = old;


        }

        public void Hide()
        {
            text.Entity.Visible = false;
            plusBoxEnt.Visible = false;
            minBoxEnt.Visible = false;
        }
    }
}