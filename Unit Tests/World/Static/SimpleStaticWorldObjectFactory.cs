using System;
using MHGameWork.TheWizards.Common;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.World.Static;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Tests.World.Static
{
    public class SimpleStaticWorldObjectFactory : IStaticWorldObjectFactory
    {
        private readonly SimpleMeshRenderer renderer;
        private readonly IMeshFactory meshFactory;

        public SimpleStaticWorldObjectFactory(SimpleMeshRenderer renderer, IMeshFactory meshFactory)
        {
            this.renderer = renderer;
            this.meshFactory = meshFactory;
        }

        public void ApplyUpdatePacket(IStaticWorldObject obj, StaticWorldObjectUpdatePacket p)
        {
            var o = obj as SimpleStaticWorldObject;

            var newMesh = meshFactory.GetMesh(p.MeshGuid);


            if (o.Mesh != newMesh)
            {
                o.ChangeMesh(newMesh);
            }

            o.WorldMatrix = XnaMathExtensions.CreateMatrixFromFloatArray(p.WorldMatrix, 0);
        }

        public IStaticWorldObject CreateNew()
        {
            var obj = new SimpleStaticWorldObject(renderer);
            return obj;
        }

        public void Delete(IStaticWorldObject obj)
        {
            ((SimpleStaticWorldObject)obj).Delete();
        }
    }
}
