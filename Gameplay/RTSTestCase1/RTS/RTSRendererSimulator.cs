using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.MathExtra;
using MHGameWork.TheWizards.RTS.Commands;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;
using Quaternion = Microsoft.Xna.Framework.Quaternion;

namespace MHGameWork.TheWizards.RTS
{
    public class RTSRendererSimulator : ISimulator
    {
        public void Simulate()
        {
            updateGoblins();
            UpdateDroppedThings();
            UpdateFactories();
            updateCannons();
        }

        private void updateCannons()
        {
            TW.Data.EnsureAttachment<Cannon, CannonRenderData>(o => new CannonRenderData(o));
            foreach (var f in TW.Data.GetChangedObjects<Cannon>())
            {
                f.get<CannonRenderData>().Update();
            }
        }

        private static void updateGoblins()
        {
            TW.Data.EnsureAttachment<Goblin, GoblinRenderData>(g => new GoblinRenderData(g));
            foreach (var goblin in TW.Data.GetChangedObjects<Goblin>())
                goblin.get<GoblinRenderData>().fixRendering();
        }

        private void UpdateFactories()
        {
            TW.Data.EnsureAttachment<Factory, FactoryRenderData>(o => new FactoryRenderData(o));
            foreach (var f in TW.Data.GetChangedObjects<Factory>())
            {
                f.get<FactoryRenderData>().Update();
            }
        }

        private static void UpdateDroppedThings()
        {
            foreach (var c in TW.Data.GetChangesOfType<DroppedThing>())
            {
                
            }
        }






    

    public class FactoryRenderData : IModelObjectAddon<Factory>
    {
        private readonly Factory factory;
        private Engine.WorldRendering.Entity main;
        private Engine.WorldRendering.Entity input;
        private Engine.WorldRendering.Entity output;
        public FactoryRenderData(Factory factory)
        {
            this.factory = factory;
            main = new Engine.WorldRendering.Entity();
            var builder = new MeshBuilder();
            builder.AddBox(new Vector3(-1, 0, -1), new Vector3(1, 3, 1));
            main.Mesh = builder.CreateMesh();


            input = new Engine.WorldRendering.Entity();
            builder = new MeshBuilder();
            var bb = factory.GetInputArea();
            bb.Maximum += Vector3.UnitY * (-bb.Maximum.Y + 0.03f);
            builder.AddBox(bb.Minimum, bb.Maximum);
            input.Mesh = builder.CreateMesh();
            input.Mesh.GetCoreData().Parts[0].MeshMaterial.DiffuseMap = factory.InputType.Texture;


            output = new Engine.WorldRendering.Entity();
            builder = new MeshBuilder();
            bb = factory.GetOutputArea();
            bb.Maximum += Vector3.UnitY * (-bb.Maximum.Y + 0.03f);
            builder.AddBox(bb.Minimum, bb.Maximum);
            output.Mesh = builder.CreateMesh();
            output.Mesh.GetCoreData().Parts[0].MeshMaterial.DiffuseMap = factory.OutputType.Texture;


        }

        public void Dispose()
        {
            TW.Data.Objects.Remove(main);
        }

        public void Update()
        {
            main.WorldMatrix = Matrix.Translation(factory.Position);
        }
    }
}