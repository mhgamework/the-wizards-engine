using System;
using MHGameWork.TheWizards.GodGame.Internal;
using MHGameWork.TheWizards.GodGame.Internal.Model;
using MHGameWork.TheWizards.GodGame.Internal.Rendering;
using MHGameWork.TheWizards.GodGame.UI;
using SlimDX;

namespace MHGameWork.TheWizards.GodGame.VoxelInfoVisualizers
{
    public class ValueControlVisualizer : IRenderable
    {
        private readonly Func<int> getValue;
        private readonly Action<int> setValue;
        public ValueControl ValueControl { get; private set; }

        public ValueControlVisualizer(IVoxelHandle handle, Func<int> getValue, Action<int> setValue, Matrix local)
        {
            this.getValue = getValue;
            this.setValue = setValue;
            ValueControl = new ValueControl();


            var pos = handle.GetInternalVoxel().Coord.ToVector2().ToXZ() + new Vector2(0.5f).ToXZ();

            ValueControl.WorldMatrix = local * Matrix.Translation(pos) * Matrix.Scaling(new Vector3(handle.GetInternalVoxel().World.VoxelSize.X));
        }

        public void Show()
        {
            ValueControl.Show();
        }

        public void Update()
        {
            ValueControl.Value = getValue();
            ValueControl.Update();
        }

        public void Hide()
        {
            ValueControl.Hide();
        }

        public bool TryProcessUserInput(Ray ray)
        {
            ValueControl.Value = getValue();
            var ret = false;
            if (TW.Graphics.Mouse.LeftMouseJustPressed && ValueControl.TryLeftClick(ray)) ret = true;
            else if (TW.Graphics.Mouse.RightMouseJustPressed && ValueControl.TryRightClick(ray)) ret = true;

            setValue(ValueControl.Value);

            return ret;
        }
    }
}