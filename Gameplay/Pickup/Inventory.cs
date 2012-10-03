using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.Pickup
{
    /// <summary>
    /// Responsible for keeping track of the items in the inventory and rendering them.
    /// Constraint: The index of the same item in the Items-List and entities-List MUST be the same.
    /// </summary>
    [ModelObjectChanged]
    public class Inventory : EngineModelObject
    {
        public List<ItemEntity> Items = new List<ItemEntity>();
        public ItemEntity SelectedItem;
        private bool visible;

        private List<WorldRendering.Entity> entities = new List<WorldRendering.Entity>();

        public void Update()
        {
            for (int index = 0; index < entities.Count; index++)
            {
                const float boxWidth = 0.2f;
                const float boxHeigth = 0.25f;
                const float nbBoxesPerRow = 4;
                float xPos2D = -0.8f + boxWidth * (index % nbBoxesPerRow);
                float yPos2D = 0.8f - boxHeigth * (float)Math.Floor(index / nbBoxesPerRow);

                WorldRendering.Entity ent = entities[index];
                ent.WorldMatrix = Matrix.Scaling(0.5f, 0.5f, 0.5f) * Matrix.Translation(0, 0, -9) *
                                  Matrix.PerspectiveFovRH(MathHelper.PiOver4, 4 / 3f, 0.1f, 10) *
                                  Matrix.Translation(xPos2D, yPos2D, 0) *
                                  Matrix.Invert(TW.Graphics.Camera.ViewProjection);

                if (index == Items.IndexOf(SelectedItem) && visible)
                {
                    var oriMatrix = TW.Graphics.LineManager3D.WorldMatrix;
                    TW.Graphics.LineManager3D.WorldMatrix = Matrix.Invert(TW.Graphics.Camera.ViewProjection);

                    Vector3 topLeft = new Vector3(xPos2D - boxWidth * 0.5f, yPos2D + boxHeigth * 0.5f, 0);
                    Vector3 bottomRight = new Vector3(topLeft.X + 0.2f, topLeft.Y - 0.25f, 0);

                    TW.Graphics.LineManager3D.AddLine(topLeft, new Vector3(topLeft.X, bottomRight.Y, 0), new Color4(1, 1, 1));
                    TW.Graphics.LineManager3D.AddLine(new Vector3(topLeft.X, bottomRight.Y, 0), bottomRight, new Color4(1, 1, 1));
                    TW.Graphics.LineManager3D.AddLine(bottomRight, new Vector3(bottomRight.X, topLeft.Y, 0), new Color4(1, 1, 1));
                    TW.Graphics.LineManager3D.AddLine(new Vector3(bottomRight.X, topLeft.Y, 0), topLeft, new Color4(1, 1, 1));

                    TW.Graphics.LineManager3D.WorldMatrix = oriMatrix;

                }
            }
        }

        public void AddItem(ItemEntity i)
        {
            Items.Add(i);

            var ent = new WorldRendering.Entity();
            ent.Mesh = i.GetMesh();
            ent.Visible = visible;
            entities.Add(ent);

            SelectedItem = i;
        }

        public ItemEntity RemoveItem(ItemEntity i)
        {
            if (Items.Contains(i))
            {
                int index = Items.IndexOf(i);

                Items.RemoveAt(index);

                var ent = entities[index];
                ent.Visible = false;
                entities.RemoveAt(index);

                SelectNextItem();
                return i;
            }

            return null;
        }

        public void SelectNextItem()
        {
            if (Items.Count == 0)
            {
                SelectedItem = null;
                return;
            }

            int index = -1;
            if (Items.Contains(SelectedItem))
            {
                index = Items.IndexOf(SelectedItem);
            }

            index++;
            index = index % Items.Count;
            SelectedItem = Items[index];
        }

        public void SetVisibility(bool isVisible)
        {
            visible = isVisible;

            foreach (WorldRendering.Entity ent in entities)
            {
                ent.Visible = visible;
            }
        }
    }

}
