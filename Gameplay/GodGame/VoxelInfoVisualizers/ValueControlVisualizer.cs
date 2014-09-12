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
        private readonly Matrix local;
        public ValueControl ValueControl { get; private set; }
        private GameVoxel voxel;
        public ValueControlVisualizer(IVoxelHandle handle, Func<int> getValue, Action<int> setValue, Matrix local)
        {
            this.getValue = getValue;
            this.setValue = setValue;
            this.local = local;
            ValueControl = new ValueControl();

            voxel = handle.GetInternalVoxel();



        }

        public void Show()
        {
            ValueControl.Show();
        }

        public void Update()
        {
            ValueControl.Value = getValue();
            var pos = voxel.GetBoundingBox().GetCenter() + new Vector2(0.5f).ToXZ();
            ValueControl.WorldMatrix = local * Matrix.Translation(pos);
            ValueControl.WorldMatrix = local * Matrix.Scaling(new Vector3(voxel.World.VoxelSize.X)) * Matrix.Translation(pos);
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