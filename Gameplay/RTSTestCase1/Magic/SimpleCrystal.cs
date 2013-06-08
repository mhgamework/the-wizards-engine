using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Magic
{

    [ModelObjectChanged]
    public class SimpleCrystal : EngineModelObject, ICrystal, IFieldElement, IPhysical, IItem
    {

        public float Capacity { get; set; }
        public float Energy { get; set; }
        public Vector3 Position
        {
            get { return Physical.GetPosition(); }
            set { Physical.WorldMatrix = Matrix.Translation(value); }
        }

        public SimpleCrystal()
        {
            Capacity = 1;
            Energy = 0;
            Active = true;
            Physical = new Physical();
            Item = new ItemPart();

        }

        float ICrystal.GetCapacity()
        {
            return Capacity;
        }

        float ICrystal.GetEnergy()
        {
            return Energy;
        }

        void ICrystal.SetEnergy(float newEnergy)
        {
            Energy = newEnergy;
        }

        public Vector3 GetPosition()
        {
            return Position;
        }

        public bool Visible { get; set; }

        public bool IsActive()
        {
            return Active;
        }

        public bool Active { get; set; }

        float IFieldElement.Density
        {
            get { return Energy; }
        }

        public Physical Physical { get; set; }
        public void UpdatePhysical()
        {
            var ent = Physical;

            ent.Solid = false;
            ent.Static = true;

            ent.Mesh = TW.Assets.LoadMesh("RTS\\BuildingCrystal\\BuildingCrystal");
            ent.Visible = Active;
            ent.ObjectMatrix = Matrix.Scaling(0.1f, 0.1f, 0.1f) * Matrix.Translation(0, -0.3f, 0);
            //PointLight= PointLight ?? new PointLight();
            //PointLight.ShadowsEnabled = false;
            //PointLight.Position = crystal.Position + Vector3.UnitY * 2.3f;


            //PointLight.Size= 6;

        }

        public ItemPart Item { get; set; }





        public float glowPart { get; set; }
        private float glowSpeed = 10f;
        private Vector3 position;
        //public PointLight PointLight { get; set; }



        public void RenderBar()
        {
            if (!Visible)
                return;
            if (!IsActive())
                return;
            var level = Energy / Capacity;
            var beginPosition = GetPosition() + new Vector3(0, 0, 4);
            TW.Graphics.LineManager3D.AddBox(new BoundingBox(beginPosition, beginPosition + new Vector3(1, 1, level * 4)), new Color4(1, 0.4f, 0));
            TW.Graphics.LineManager3D.AddBox(new BoundingBox(beginPosition + new Vector3(0, 0, level * 4), beginPosition + new Vector3(1, 1, 4)), new Color4(0.4f, 1, 0));

        }

    }
}
