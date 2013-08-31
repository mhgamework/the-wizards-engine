using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Rendering.Deferred;
using SlimDX;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;
using Device = SlimDX.Direct3D11.Device;
using System.Linq;


namespace MHGameWork.TheWizards.Rendering.Vegetation.Grass
{
    /// <summary>
    /// This class is responsible for creating  a grassmesh from a given grassmap
    /// </summary>
    public class GrassMeshFactory
    {
        private List<DeferredMeshVertex> vertices = new List<DeferredMeshVertex>();
        private List<int> indices = new List<int>();
        private Seeder seeder = new Seeder(123);





        public GrassMeshRenderData CreateGrassMeshRenderData(DX11Game game, Device device, IGrassMap grassMap, int seed)
        {

            createMesh(grassMap, seed);
            Buffer vertexBuffer;
            Buffer indexBuffer;
            using (var strm = new DataStream(vertices.ToArray(), true, false))
            {
                vertexBuffer = new Buffer(device, strm, new BufferDescription
                {
                    BindFlags = BindFlags.VertexBuffer,
                    Usage = ResourceUsage.Immutable,
                    SizeInBytes = (int)strm.Length
                });
            }

            using (var strm = new DataStream(indices.Select(i => (ushort)i).ToArray(), true, false))
            {
                indexBuffer = new Buffer(device, strm, new BufferDescription
                {
                    BindFlags = BindFlags.IndexBuffer,
                    Usage = ResourceUsage.Immutable,
                    SizeInBytes = indices.Count * 2
                });
            }




            return new GrassMeshRenderData(vertexBuffer, indexBuffer, indices.Count, device, game);

        }

        private void createMesh(IGrassMap grassMap, int seed)
        {
            seeder = new Seeder(seed);
            for (int i = 0; i < grassMap.Length; i++)
            {
                for (int j = 0; j < grassMap.Width; j++)
                {
                    for (int k = 0; k < grassMap.getDensity(i, j); k++)// TODO: Might better be pseudo random that distributes the meshes evenly over the square
                    {
                        Vector2 vec = seeder.NextVector2(new Vector2(0, 0), new Vector2(1, 1));

                        addStar(new Vector4(i + vec.X, grassMap.getHeight(i, j), j + vec.Y, 1), grassMap.getGrowingHeight(i, j),k);// + seeder.NextFloat(-0.1f, 0.1f)
                    }
                }
            }
        }
        private void addStar(Vector4 position, float height,int starCount)
        {
            
            float unitLength = 0.25f;
            height = height*unitLength;
            Vector4 direction = Vector4.UnitX * unitLength;
            Vector3 direction3 = new Vector3(direction.X, direction.Y, direction.Z);
            Vector3 normal = Vector3.UnitX;
            Matrix rotation = Matrix.RotationY((float)(Math.PI / 2.5f));
            for (int i = 0; i < 5; i++)
            {
                direction = Vector4.Transform(direction, rotation);
                direction.W = 0;
                direction3 = new Vector3(direction.X, direction.Y, direction.Z);
                normal = Vector3.Cross(direction3, Vector3.UnitY);

                vertices.Add(new DeferredMeshVertex(position - direction, new Vector2(0, 1), normal));
                vertices.Add(new DeferredMeshVertex(position + Vector4.UnitY * height - direction, new Vector2(0, 0), normal));
                vertices.Add(new DeferredMeshVertex(position + direction, new Vector2(1, 1), normal));
                vertices.Add(new DeferredMeshVertex(position + Vector4.UnitY * height + direction, new Vector2(1, 0), normal));
                // triangle one
                indices.Add(0 + 4 * i+30*starCount);
                indices.Add(1 + 4 * i + 30 * starCount);
                indices.Add(3 + 4 * i + 30 * starCount);

                // triangle one
                indices.Add(0 + 4 * i + 30 * starCount);
                indices.Add(3 + 4 * i + 30 * starCount);
                indices.Add(2 + 4 * i + 30 * starCount);
            }


        }

    }
}