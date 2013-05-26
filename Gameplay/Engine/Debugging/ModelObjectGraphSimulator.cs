using System.Collections.Generic;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Reflection;
using QuickGraph;

namespace MHGameWork.TheWizards.Engine.Debugging
{
    /// <summary>
    /// Builds a graph of the model objects
    /// Note: this is also used as a DI service!!!
    /// </summary>
    public class ModelObjectGraphSimulator : ISimulator
    {
        private Dictionary<IModelObject, Vertex> map = new Dictionary<IModelObject, Vertex>();

        public ModelObjectGraphSimulator()
        {
            Graph = new BidirectionalGraph<Vertex, Edge<Vertex>>();
        }

        public BidirectionalGraph<Vertex, Edge<Vertex>> Graph { get; private set; }

        public void Simulate()
        {
            var modified = new HashSet<IModelObject>();

            // First create and destroy all vertices
            foreach (var c in TW.Data.GetChangesOfType<IModelObject>())
            {
                if (c.Change == ModelChange.Added)
                    createVertex(c.ModelObject);

                if (c.Change == ModelChange.Added || c.Change == ModelChange.Modified)
                    modified.Add(c.ModelObject);

                if (c.Change == ModelChange.Removed)
                    removeVertex(c.ModelObject);
            }

            // Now update the edges
            foreach (var c in modified)
            {
                updateEdges(getVertex(c));
            }

        }

        private void createVertex(IModelObject c)
        {
            var v = new Vertex(c);
            map[c] = v;
            Graph.AddVertex(v);
        }

        private Vertex getVertex(IModelObject c)
        {
            return map[c];
        }

        private void removeVertex(IModelObject c)
        {
            Graph.RemoveVertex(map[c]);
            map.Remove(c);
            //TODO: remove the edges also??
        }

        private void updateEdges(Vertex v)
        {
            var atts = ReflectionHelper.GetAllAttributes(v.ModelObject.GetType());

            // Remove all edges
            Graph.RemoveOutEdgeIf(v, delegate { return true; });

            foreach (var att in atts)
            {
                if (!att.Type.IsSubclassOf(typeof(IModelObject))) continue;

                var data = att.GetData(v.ModelObject) as IModelObject;

                if (data == null) continue;
                if (!map.ContainsKey(data)) continue;

                Graph.AddEdge(new Edge<Vertex>(v, getVertex(data)));

            }
        }

        public class Vertex
        {
            public Vertex(IModelObject obj)
            {
                ModelObject = obj;
            }
            public IModelObject ModelObject { get; private set; }
        }
    }


}