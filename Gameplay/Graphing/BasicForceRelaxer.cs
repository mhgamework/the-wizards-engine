using System;
using System.Collections;
using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.Graphing
{
    /// <summary>
    /// Responsible for creating 3D positions for a graph using a basic force based relaxation algorithm
    /// </summary>
    public class BasicForceRelaxer
    {

        private float c1 = 2;
        private float c2 = 1;
        private float c3 = 1;
        private float c4 = 0.1f;

        public void Reset()
        {
            foreach (var v in getVertices())
            {
                v.Position = getRandomPosition();
            }
        }

        private Vector3 getRandomPosition()
        {
            throw new System.NotImplementedException();
        }

        public void Relax()
        {




            foreach (var v in getVertices())
            {
                var force = calculateVertexForce(v);
                v.Position = v.Position + c4 * force;
            }


        }

        private IEnumerable<IVertex3D> getVertices()
        {
            throw new System.NotImplementedException();
        }

        private Vector3 calculateVertexForce(IVertex3D v)
        {
            var force = new Vector3();
            foreach (var n in getConnectedVertices(v))
            {
                var dist = Vector3.Distance(v.Position, n.Position);
                var dir = (n.Position - v.Position) / dist;
                force += c1 * (float)Math.Log(dist / c2) * dir;
            }
            foreach (var n in getVertices())
            {
                var dist = Vector3.Distance(v.Position, n.Position);
                var dir = (n.Position - v.Position) / dist;
                force += c3 / (dist * dist) * dir;
            }
            return force;
        }

        private IEnumerable<IVertex3D> getConnectedVertices(IVertex3D vertex3D)
        {
            throw new NotImplementedException();
        }
    }

    internal interface IVertex3D
    {
        Vector3 Position { get; set; }
    }
}