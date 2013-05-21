using System;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.RTSTestCase1.Items;
using MHGameWork.TheWizards.RTSTestCase1.Magic;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Rendering
{
    using MHGameWork.TheWizards.Data;
    using MHGameWork.TheWizards.Engine;
    using MHGameWork.TheWizards.Engine.WorldRendering;
    using MHGameWork.TheWizards.RTSTestCase1.Items;
    using SlimDX;


    public class CrystalRenderData : IModelObjectAddon<SimpleCrystal>
    {
        private SimpleCrystal crystal;
        private Entity ent;
        public float glowPart{ get; set; }
        private float glowSpeed = 10f;
        //public PointLight PointLight { get; set; }
        public CrystalRenderData(SimpleCrystal crystal)
        {
            this.crystal = crystal;
            ent = new Engine.WorldRendering.Entity();
            ent.Solid = false;
            ent.Static = true;
            ent.Kinematic = false;
            ent.Tag = crystal;
            crystal.set(ent);
        }


        public void Update()
        {
            if (ent.Mesh == null)
                ent.Mesh = TW.Assets.LoadMesh("RTS\\BuildingCrystal\\BuildingCrystal");
            ent.Visible = crystal.Active;
            //PointLight= PointLight ?? new PointLight();
            //PointLight.ShadowsEnabled = false;
            //PointLight.Position = crystal.Position + Vector3.UnitY * 2.3f;
            
            ent.WorldMatrix =  Matrix.Translation(crystal.Position) ;
            
            //PointLight.Size= 6;
        }
        
        public void RenderBar()
        {
            if (!crystal.Visible)
                return;
            if (!crystal.IsActive())
                return;
            var level = crystal.Energy/crystal.Capacity;
            var beginPosition = crystal.GetPosition() + new Vector3(0, 0, 4);
            TW.Graphics.LineManager3D.AddBox(new BoundingBox(beginPosition,beginPosition + new Vector3(1,1,level*4)),new Color4(1,0.4f,0));
            TW.Graphics.LineManager3D.AddBox(new BoundingBox(beginPosition + new Vector3(0, 0, level * 4), beginPosition + new Vector3(1, 1, 4)), new Color4(0.4f,1, 0));
            
        }

        public void Dispose()
        {
            if (ent == null) return;

            ent.Visible = false;
            TW.Data.RemoveObject(ent);
            ent = null;
        }
    }
}
