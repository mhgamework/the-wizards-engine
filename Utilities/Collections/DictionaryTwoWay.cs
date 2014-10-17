using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Collections
{
    public class DictionaryTwoWay<T, U>
    {
        private Dictionary<T, U> uMap = new Dictionary<T, U>();
        private Dictionary<U, T> tMap = new Dictionary<U, T>();

        public DictionaryTwoWay()
        {

        }

        public bool Contains(T t)
        {
            return uMap.ContainsKey(t);

        }

        public bool Contains(U u)
        {
            return tMap.ContainsKey(u);
        }

        public void Add(T t, U u)
        {
            uMap.Add(t, u);
            tMap.Add(u, t);
        }

        public IEnumerable<U> GetAllU()
        {
            return uMap.Values;
        }
        public IEnumerable<T> GetAllT()
        {
            return tMap.Values;
        }

        public T this[U u] { get { return tMap[u]; } }
        public U this[T t] { get { return uMap[t]; } }

        public void set(T t, U u)
        {
            tMap[u] = t;
            uMap[t] = u;
        }

        public void Clear()
        {
            tMap.Clear();
            uMap.Clear();
        }
    }
}
