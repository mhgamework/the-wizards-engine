using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.MathExtra;
using MHGameWork.TheWizards.WorldRendering;
using SlimDX;

namespace MHGameWork.TheWizards.RTS
{
    public class GoblinRendererSimulator : ISimulator
    {
        public void Simulate()
        {
            foreach (var goblin in TW.Data.GetChangedObjects<Goblin>())
            {
                if (goblin.get<GoblinRenderData>() == null)
                    goblin.set(new GoblinRenderData { LookDirection = new Vector3(0, 0, 1) });
                var ent = goblin.get<WorldRendering.Entity>();
                fixRendering(goblin);
            }


        }   
        private static void fixRendering(Goblin goblin)
        {
            if(goblin.get<WorldRendering.Entity>()== null)
                goblin.set(new WorldRendering.Entity());
            var ent = goblin.get<WorldRendering.Entity>();
                
            var renderData = goblin.get<GoblinRenderData>();
            if ((renderData.LastPosition - goblin.Position).Length() > 0.001f)
            {
                renderData.LookDirection = goblin.Position - renderData.LastPosition;
            }
            renderData.LastPosition = goblin.Position;
            var quat = Functions.CreateFromLookDir(-Vector3.Normalize(renderData.LookDirection).xna());
            ent.WorldMatrix = Microsoft.Xna.Framework.Matrix.CreateFromQuaternion(quat).dx() * /*Matrix.Scaling(0.01f, 0.01f, 0.01f) **/
                              Matrix.Translation(goblin.Position);

            renderData.LastPosition = goblin.Position;
            ent.Mesh = MeshFactory.Load("Core\\Barrel01");//Load("Goblin\\GoblinLowRes");
            ent.Solid = true;
            ent.Static = false;
        }
        private class GoblinRenderData//: WorldRendering.Entity
        {
            public Vector3 LastPosition { get; set; }
            public Vector3 LookDirection { get; set; }
        }
    }
}
