using System;
using MHGameWork.TheWizards._XNA.Scripting.API;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Tests.Features.Core.Scripting
{
    public class SimpleScriptDatabinding : IScript
    {
        public string Name;
        public int Data;

        public IDataElement<Vector3> SharedTargetLocation;


        public void Init(IEntityHandle handle)
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }
    }
}
