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
    public class ValueControl
    {
        private Renderable valueControlRenderable;
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
            valueControlRenderable = new Renderable(this);
            WorldMatrix = Matrix.Identity;
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

 


        public IRenderable GetRenderable()
        {
            return valueControlRenderable;
        }

        private class Renderable : IRenderable
        {
            private readonly ValueControl control;
            private TextRectangle text;
            private Entity plusBox;
            private Entity minBox;

            public Renderable(ValueControl control)
            {
                this.control = control;
                text = new TextRectangle();
                text.Radius = 0.7f;
                text.IsBillboard = true;
                plusBox = new Entity();
                minBox = new Entity();

                plusBox.Mesh = UtilityMeshes.CreateMeshWithText(0.5f, "+", TW.Graphics);
                minBox.Mesh = UtilityMeshes.CreateMeshWithText(0.5f, "-", TW.Graphics);

                Update();
                Hide();

            }
            public void Show()
            {
                text.Entity.Visible = true;
                plusBox.Visible = true;
                minBox.Visible = true;
            }

            public void Update()
            {
                text.Text = control.Value.ToString();
                text.Position = control.WorldMatrix.GetTranslation() + new Vector3(0, 1.5f, 0);

                text.Update();

                plusBox.WorldMatrix = Matrix.Scaling(control.plusBox.GetSize()) *
                                      Matrix.Translation(control.plusBox.GetCenter()) * control.WorldMatrix;
                minBox.WorldMatrix = Matrix.Scaling(control.minBox.GetSize()) *
                                      Matrix.Translation(control.minBox.GetCenter()) * control.WorldMatrix;

                var old = TW.Graphics.LineManager3D.WorldMatrix;
                TW.Graphics.LineManager3D.WorldMatrix = control.WorldMatrix;
                if (control.intersectsBox(TW.Data.Get<CameraInfo>().GetCenterScreenRay(), control.minBox))
                    TW.Graphics.LineManager3D.AddBox(control.minBox, Color.Yellow.dx());
                if (control.intersectsBox(TW.Data.Get<CameraInfo>().GetCenterScreenRay(), control.plusBox))
                    TW.Graphics.LineManager3D.AddBox(control.plusBox, Color.Yellow.dx());
                TW.Graphics.LineManager3D.WorldMatrix = old;


            }

            public void Hide()
            {
                text.Entity.Visible = false;
                plusBox.Visible = false;
                minBox.Visible = false;
            }
        }
    }
}