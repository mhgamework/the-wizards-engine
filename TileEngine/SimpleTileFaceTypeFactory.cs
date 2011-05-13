using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.TileEngine;

namespace MHGameWork.TheWizards.Tests.TileEngine
{
    public class SimpleTileFaceTypeFactory : ITileFaceTypeFactory
    {
        public TileFaceType GetTileFaceType(Guid guid)
        {
            for(int i=0; i<types.Count;i++)
            {
                if (types[i].Guid.Equals(guid)) return types[i];
            }
            var ret = new TileFaceType(guid);
            types.Add(ret);
            return ret;
        }

        private List<TileFaceType> types = new List<TileFaceType>();
        public void Add(TileFaceType type)
        {
            types.Add(type);
        }
    }
}
