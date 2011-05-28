using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Rendering;

namespace MHGameWork.TheWizards.Tests.Rendering
{
    public class SimpleMeshFactory : IMeshFactory
    {
        private Dictionary<Guid, IMesh> map = new Dictionary<Guid, IMesh>();

        public IMesh GetMesh(Guid guid)
        {
            return map[guid];
        }

        public void AddMesh(IMesh mesh)
        {
            map.Add(mesh.Guid, mesh);
        }
    }
}
