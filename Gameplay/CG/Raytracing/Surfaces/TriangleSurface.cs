using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG.Shading;
using MHGameWork.TheWizards.CG.Spatial;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.MathExtra;
using MHGameWork.TheWizards.Rendering;
using SlimDX;

namespace MHGameWork.TheWizards.CG.Raytracing.Surfaces
{
    public class TriangleSurface : IGenericSurface, ISurface
    {
        private readonly TangentVertex[] vertices;
        private readonly int startVertexIndex;

        public IShader shader;

        public TriangleSurface(TangentVertex[] vertices, int triangleIndex, IShader shader)
        {
            this.shader = shader;
            this.vertices = vertices;
            this.startVertexIndex = triangleIndex * 3;
        }

        public BoundingBox GetBoundingBox(IBoundingBoxCalculator calc)
        {
            return
                BoundingBox.FromPoints(new[]
                                           {
                                               vertices[startVertexIndex].pos.dx(),
                                               vertices[startVertexIndex+1].pos.dx(),
                                               vertices[startVertexIndex+2].pos.dx()
                                           });
        }

        public void Intersects(ref RayTrace trace, out float? result, out IShadeCommand shadeCommand, bool generateShadeCommand)
        {
            var v1 = vertices[startVertexIndex].pos;
            var v2 = vertices[startVertexIndex+1].pos;
            var v3 = vertices[startVertexIndex+2].pos;

            var ray = trace.Ray.xna();

            float v;
            float u;
            Functions.RayIntersectsTriangle(ref ray, ref v1, ref v2, ref v3, out result, out u, out v);
            if (result == null)
            {
                shadeCommand = null;
                return;
            }
            shadeCommand = new ShadeCommand(shader, this, u, v, trace, trace.Ray.Position + trace.Ray.Direction * result.Value);
        }

        private class ShadeCommand : IShadeCommand
        {
            private IShader shader;
            private TriangleSurface surface;
            private float U;
            private float V;
            private RayTrace trace;
            private readonly Vector3 hitPoint;

            public ShadeCommand(IShader shader, TriangleSurface surface, float u, float v, RayTrace trace, Vector3 hitPoint)
            {
                this.shader = shader;
                this.surface = surface;
                U = u;
                V = v;
                this.trace = trace;
                this.hitPoint = hitPoint;
            }

            public Color4 CalculateColor()
            {
                var Vertex1 = surface.vertices[surface.startVertexIndex];
                var Vertex2 = surface.vertices[surface.startVertexIndex+1];
                var Vertex3 = surface.vertices[surface.startVertexIndex+2];

                var input = new GeometryInput();
                input.Position = hitPoint;
                input.Normal = (Vertex2.normal * U + Vertex3.normal * V + Vertex1.normal * (1 - U - V)).dx();

                //input.Normal =
                //    Vector3.Normalize(-Vector3.Cross((raycast.Vertex1.Position - raycast.Vertex2.Position),
                //                                    (raycast.Vertex1.Position - raycast.Vertex3.Position)));

                //input.Normal = raycast.Vertex1.Normal;
                input.Normal = Vector3.Normalize(input.Normal); // Renormalize!
                input.Texcoord = (Vertex2.uv * U + Vertex3.uv * V + Vertex1.uv * (1 - U - V)).dx();
                //TODO: perspective correction

                //input.Normal = raycast.Vertex2.Normal;

                //input.Diffuse = new Color4(0.2f, 0.8f, 0.3f);
                //input.Diffuse = new Color4(0.8f, 0.8f, 0.8f);
                //input.SpecularColor = new Color4(1, 1, 1);
                //input.SpecularPower = 15;
                //input.SpecularIntensity = 2;
                //input.Diffuse = new Color4(raycast.U, raycast.V, 0);

                //if (raycast.Material.DiffuseMap != null)
                //{
                //    var tex = cache.Load(raycast.Material.DiffuseMap);
                //    var sampler = new Texture2DSampler();
                //    //input.Diffuse = sampler.SampleBilinear(tex, input.Texcoord);
                //}


                return shader.Shade(input, trace);
            }
        }
    }
}
