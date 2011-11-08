﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Utilities
{
    public class PrefilledList<T>
    {
        private readonly Func<T> createItemDelegate;

        private T[] list;

        public int Count { get; private set; }

        public PrefilledList(Func<T> createItemDelegate)
        {
            this.createItemDelegate = createItemDelegate;

            list = new T[10];

        }
        private void expandList()
        {
            Array.Resize(ref list, list.Length * 2);
        }



        public T Add()
        {
            if (Count == list.Length) expandList();
            return list[Count];

        }
        public void Clear()
        {
            Count = 0;
        }
        public T this[int index]
        {
            get { if (index >= Count) throw new IndexOutOfRangeException(); return list[index]; }

        }


        public void GetArray(out T[] array, out int length)
        {
            array = list;
            length = Count;
        }
    }
}
