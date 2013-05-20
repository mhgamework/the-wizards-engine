using System;
using MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Selecting;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldInputting
{
    /// <summary>
    /// Allows display of a 3D version of an EditorMenuConfiguration
    /// TODO: center the buttons
    /// </summary>
    public class MenuDisplayer
    {
        private readonly EditorMenuConfiguration config;

        private bool visible = false;

        public Vector3 Position { get; private set; }

        public bool Visible
        {
            get { return visible; }
            private set { visible = value; }
        }

        private Vector3 lookDirection;

        private BoundingBoxSelectableProvider provider;
        private Vector3 up;
        private Vector3 right;

        public MenuDisplayer(EditorMenuConfiguration config, WorldSelector selector)
        {
            this.config = config;

            provider = BoundingBoxSelectableProvider.Create(config.Items, getBoundingBox, OnClick );

            selector.AddProvider(provider);

        }

        private void OnClick(EditorMenuConfiguration.Item i)
        {
            i.Command();
            Hide();

        }

        public void Show(Vector3 pos, Vector3 lookDir)
        {
            Visible = true;
            Position = pos;
            lookDirection = lookDir;

            up = Vector3.UnitY;
            right = -Vector3.Cross(up, lookDirection);
            up = Vector3.Cross(lookDirection, up);

        }
        public void Hide()
        {
            Visible = false;
        }

        public void Toggle(Vector3 pos, Vector3 lookDir)
        {
            Visible = !Visible;
            if (Visible) Show(pos, lookDir); else Hide();
        }

        public void Simulate()
        {
            if (!Visible) return;

            int num = 0;
            foreach (var i in config.Items)
            {
                var color = new Color4(0.5f, 1, 0);
                if (provider.IsTargeted(i)) color = new Color4(1, 0, 0);

                TW.Graphics.LineManager3D.AddBox(getBoundingBox(i),color);
                num++;
            }

        }


        private BoundingBox getBoundingBox(EditorMenuConfiguration.Item item)
        {
            

            var index = config.Items.IndexOf(item);

            var size = new Vector3(0.5f, 0.5f, 0.5f);

            var pos = Position + right * index * 2.0f;

            return new BoundingBox(pos - size, pos + size);
        }
    }
}