using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Utilities
{
    public class EventList<T>
    {
        private readonly Action<T> onAdd;
        private readonly Action<T> onRemove;
        private List<T> list = new List<T>();

        public EventList(Action<T> onAdd, Action<T> onRemove)
        {
            this.onAdd = onAdd;
            this.onRemove = onRemove;
        }

        public void Clear()
        {
            var cache = list.ToArray();
            list.Clear();
            for (int i = 0; i < cache.Length; i++)
            {
                onRemove(cache[i]);
            }
        }
        public bool Contains(T item)
        {
            return list.Contains(item);
        }
        public bool Remove(T item)
        {
            var ret = list.Remove(item);
            if (ret)
                onRemove(item);
            return ret;
        }
        public void Add(T item)
        {
            list.Add(item);
            onAdd(item);
        }
        public int Count
        {
            get { return list.Count; }
        }
        public T this[int index]
        {
            get { return list[index]; }
            set
            {
                throw new InvalidOperationException(); list[index] = value;
            }
        }



    }
}
