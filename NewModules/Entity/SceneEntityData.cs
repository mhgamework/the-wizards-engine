using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Entity
{
    public class SceneEntityData : ISceneData
    {
        public List<IEntity> Entities = new List<IEntity>();
    }
}
