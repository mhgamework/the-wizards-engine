using System.Collections.Generic;
using MHGameWork.TheWizards.World.Static;

namespace MHGameWork.TheWizards.ApplicationLogic.EntityOud
{
    public class StaticEntityContainer
    {
        public List<StaticEntity> Entities = new List<StaticEntity>();

        public void InitAll(ServerStaticWorldObjectSyncer syncer)
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                Entities[i].Init(syncer);
            }
        }
    }
}