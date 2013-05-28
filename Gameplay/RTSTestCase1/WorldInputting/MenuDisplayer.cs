using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Engine.WorldRendering;
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

        public EditorMenuConfiguration Config
        {
            get { return config; }
        }

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

            provider = BoundingBoxSelectableProvider.Create(config.Items, getBoundingBox, OnClick);

            selector.AddProvider(provider);

        }

        private void OnClick(EditorMenuConfiguration.Item i)
        {
            i.Command();
            Hide();

        }

        private List<Entity> entities = new List<Entity>();

        private Dictionary<EditorMenuConfiguration.Item, Entity> map =
            new Dictionary<EditorMenuConfiguration.Item, Entity>();

        public void Show(Vector3 pos, Vector3 lookDir, Vector3 up)
        {
            Visible = true;
            Position = pos;
            lookDirection = lookDir;

            this.up = up;
            right = -Vector3.Cross(up, lookDirection);
            up = Vector3.Cross(lookDirection, up);

            foreach (var i in config.Items)
            {
                var color = new Color4(0.5f, 1, 0);
                if (provider.IsTargeted(i)) color = new Color4(1, 0, 0);

                var bb = getBoundingBox(i);

                if (! map.ContainsKey(i))
                {
                    var e = new Entity()
                    {
                        Mesh = UtilityMeshes.CreateMeshWithText(bb.GetSize().X * 0.5f, i.Name, TW.Graphics)
                        //Visible = false

                    };

                    entities.Add(e);

                    map[i] = e;
                }
                var ent = map[i];
                ent.WorldMatrix = Matrix.Translation(bb.GetCenter());
                ent.Visible = true;


            }

            provider.Enabled = true;
        }
        public void Hide()
        {
            Visible = false;
            foreach (var e in entities)
            {
                e.Visible = false;
            }
            provider.Enabled = false;
        }

        public void Toggle(Vector3 pos, Vector3 lookDir, Vector3 up)
        {
            Visible = !Visible;
            if (Visible) Show(pos, lookDir, up); else Hide();
        }

        public void Simulate()
        {
            if (!Visible) return;

            int num = 0;
            foreach (var i in config.Items)
            {
                var color = new Color4(0.5f, 1, 0);
                //color = new Color4(1, 0, 0);
                if (!provider.IsTargeted(i)) continue;

                TW.Graphics.LineManager3D.AddBox(getBoundingBox(i), color);
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