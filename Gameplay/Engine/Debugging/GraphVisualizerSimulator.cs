using System.Collections.Generic;
using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Graphing;
using MHGameWork.TheWizards.RTSTestCase1;
using QuickGraph;
using SlimDX;

namespace MHGameWork.TheWizards.Engine.Debugging
{
    public class GraphVisualizerSimulator : ISimulator
    {
        private BidirectionalGraph<ModelObjectGraphSimulator.Vertex, Edge<ModelObjectGraphSimulator.Vertex>> g;
        private BasicForceRelaxer<ModelObjectGraphSimulator.Vertex, Edge<ModelObjectGraphSimulator.Vertex>> relaxer;

        Seeder seeder = new Seeder(97897);

        private Dictionary<ModelObjectGraphSimulator.Vertex, Entity> entities =
            new Dictionary<ModelObjectGraphSimulator.Vertex, Entity>();

        public GraphVisualizerSimulator()
        {
            g = DI.Get<ModelObjectGraphSimulator>().Graph;
            relaxer = new BasicForceRelaxer<ModelObjectGraphSimulator.Vertex, Edge<ModelObjectGraphSimulator.Vertex>>(g);
            relaxer.Reset();

            g.VertexAdded += g_VertexAdded;

            foreach (var e in TW.Data.Get<Data>().OwnedEntities)
                TW.Data.RemoveObject(e);
            TW.Data.Get<Data>().OwnedEntities.Clear();

        }

        void g_VertexAdded(ModelObjectGraphSimulator.Vertex vertex)
        {
            vertex.Position = seeder.NextVector3(-MathHelper.One.xna(), MathHelper.One.xna()).dx();
        }
        public void Simulate()
        {
            var s = new Seeder(123);

            relaxGraph();

            foreach (var n in g.Vertices)
            {
                if (n.ModelObject is Entity && entities.ContainsValue(n.ModelObject as Entity)) continue;
                var ent = getEntity(n);
                ent.WorldMatrix = Matrix.Translation(n.Position);
                //TW.Graphics.LineManager3D.AddCenteredBox(n.Position, 1, getRandomColor(s));
            }
            foreach (var e in g.Edges)
            {
                TW.Graphics.LineManager3D.AddLine(e.Source.Position, getRandomColor(s), e.Target.Position, getRandomColor(s));
            }
        }

        private void relaxGraph()
        {
            //for (int i = 0; i < 2; i++)
            relaxer.Relax(TW.Graphics.Elapsed);
        }

        private static Color4 getRandomColor(Seeder s)
        {
            return new Color4(s.NextVector3(Vector3.Zero.xna(), MathHelper.One.xna()).dx());
        }

        private Entity getEntity(ModelObjectGraphSimulator.Vertex v)
        {
            if (!entities.ContainsKey(v))
            {
                var ent = new Entity();
                ent.Mesh = UtilityMeshes.CreateMeshWithText(0.2f, v.ModelObject.GetType().Name, TW.Graphics);
                entities[v] = ent;
                TW.Data.Get<Data>().OwnedEntities.Add(ent);
            }
            return entities[v];
        }

        public class Data : EngineModelObject
        {
            public Data()
            {
                OwnedEntities = new List<Entity>();
            }
            public List<Entity> OwnedEntities { get; set; }
        }
    }
}