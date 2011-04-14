using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Entity
{
   public class ObjectRenderDataFactory
    {
        public ObjectRenderElement CreateRenderElement(IObject obj)
        {
            throw new NotImplementedException();
        }

        public void AddObjectDataFactory<T>(IObjectDataRenderDataFactory<T> meshFactory) where T : IObjectData
        {
            throw new NotImplementedException();
        }
    }
}
