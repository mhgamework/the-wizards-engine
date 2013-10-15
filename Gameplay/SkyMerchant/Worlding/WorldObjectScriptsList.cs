using System;
using System.Collections;
using System.Collections.Generic;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;

namespace MHGameWork.TheWizards.SkyMerchant.Worlding
{
    public class WorldObjectScriptsList : ICollection<IWorldScript>
    {
        private List<IWorldScript> scripts = new List<IWorldScript>();

        public WorldObjectScriptsList()
        {
        }

        public IEnumerator<IWorldScript> GetEnumerator()
        {
            return scripts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IWorldScript item)
        {
            scripts.Add(item);
            //item.Initialize(obj);//TODO: probably better use observer pattern

        }

        public void Clear()
        {
            scripts.Clear();
        }

        public bool Contains(IWorldScript item)
        {
            return scripts.Contains(item);
        }

        public void CopyTo(IWorldScript[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IWorldScript item)
        {
            return scripts.Remove(item);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public int Count
        {
            get { return scripts.Count; }
        }
    }
}