using System;
using System.Collections;
using System.Collections.Generic;
using DirectX11;
using QuickGraph;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.Graphing
{
    /// <summary>
    /// Responsible for creating 3D positions for a graph using a basic force based relaxation algorithm
    /// </summary>
    public class BasicForceRelaxer<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
        where TVertex : IVertex3D
    {
        private readonly IBidirectionalGraph<TVertex, TEdge> graph;

        private float attractionFactor = 2; //2
        private float minNodeDistance = 1f; // 1
        private float repulsionFactor = 1; // 1
        private float simulationSpeed = 15f; // 0.1f

        private Seeder r = new Seeder(87683234);

        public BasicForceRelaxer(IBidirectionalGraph<TVertex, TEdge> graph)
        {
            this.graph = graph;
        }

        public void Reset()
        {
            foreach (var v in getVertices())
            {
                v.Position = getRandomPosition();
            }
        }

        private Vector3 getRandomPosition()
        {
            return r.NextVector3(-MathHelper.One.xna(), MathHelper.One.xna()).dx();
        }

        public void Relax(float elapsed)
        {
            foreach (var v in getVertices())
            {
                var force = calculateVertexForce(v);
                force += -Vector3.Normalize(v.Position)*0.1f;
                
                var change = simulationSpeed*force*elapsed;
                var length = change.Length();
                //if (length > 0.1f)
                //{
                //    change = change/length*0.1f;
                //}
                v.Position += change;
            }
        }

        private IEnumerable<IVertex3D> getVertices()
        {
            return graph.Vertices.Cast<IVertex3D>();
        }

        private Vector3 calculateVertexForce(IVertex3D v)
        {
            var force = new Vector3();
            // Edges
            foreach (var n in getConnectedVertices(v))
            {
                if (n == v) continue;
                force += calculateSpringForce(v, n);
            }
            //vertices
            foreach (var n in getVertices())
            {
                if (n == v) continue;
                force += calculateRepulsionForce(v, n);
            }
            return force;

        }

        private Vector3 calculateRepulsionForce(IVertex3D forceTarget, IVertex3D other)
        {
            var dist = Vector3.Distance(forceTarget.Position, other.Position);
            if (dist < 0.01f) dist = 0.01f;
            var dir = (other.Position - forceTarget.Position) / dist;
            return (repulsionFactor / (dist * dist)) * -dir;
        }

        private Vector3 calculateSpringForce(IVertex3D forceTarget, IVertex3D other)
        {
            var dist = Vector3.Distance(forceTarget.Position, other.Position);
            if (dist < 0.01f) dist = 0.01f;
            var dir = (other.Position - forceTarget.Position) / dist;
            return attractionFactor * (float)Math.Log(dist / minNodeDistance) * dir;
        }

        private IEnumerable<IVertex3D> getConnectedVertices(IVertex3D vertex3D)
        {
            return graph.OutEdges((TVertex)vertex3D).Select(e => e.Target).Cast<IVertex3D>()
                .Union(
                graph.InEdges((TVertex)vertex3D).Select(e => e.Source).Cast<IVertex3D>()
                )
                ;
        }
    }

    public interface IVertex3D
    {
        Vector3 Position { get; set; }
    }
}