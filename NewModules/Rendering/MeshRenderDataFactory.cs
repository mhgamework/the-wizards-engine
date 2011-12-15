using System;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Physics;

namespace MHGameWork.TheWizards.Rendering
{
    [Obsolete]
    public class MeshRenderDataFactory : IObjectDataRenderDataFactory<ObjectMeshData>
    {
        public DefaultRenderer Renderer { get; private set; }

        public MeshRenderDataFactory(DefaultRenderer renderer)
        {
            Renderer = renderer;
        }

        public SimpleMeshRenderElement CreateRenderElement(IMesh mesh)
        {
            //DOESNT WORK

            SimpleMeshRenderElement mEl = null;// = new SimpleMeshRenderElement();
            //TODO: Noob implementation. (No shared resources)

            var coreData = mesh.GetCoreData();

            for (int i = 0; i < coreData.Parts.Count; i++)
            {
                var part = coreData.Parts[i];
                var geomData = part.MeshPart.GetGeometryData();
                var positions = geomData.GetSourceVector3(MeshPartGeometryData.Semantic.Position);
                var normals = geomData.GetSourceVector3(MeshPartGeometryData.Semantic.Normal);

                var vertices = new TangentVertex[positions.Length];
                for (int j = 0; j < vertices.Length; j++)
                {
                    vertices[j].pos = positions[j];
                    vertices[j].normal = normals[j];
                    //TODO: normal,texcoord,tangent
                }

                var indices = new short[positions.Length];
                for (short j = 0; j < indices.Length; j++)
                    indices[j] = j;

                var material = new DefaultModelMaterialTextured();
                //material.DiffuseTexture = part.MeshMaterial.DiffuseMap  //TODO: load texture

                var renderable = Renderer.CreateModelRenderable(vertices, indices, material);
                var el = Renderer.CreateRenderElement(renderable);
                el.WorldMatrix = part.ObjectMatrix;


            }

            return mEl;
        }

    }
}

