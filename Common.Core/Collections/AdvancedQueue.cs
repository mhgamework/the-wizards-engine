using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.TWClient
{
    /// <summary>
    /// Used in the LoadingManager
    /// </summary>
    public class AdvancedQueue<T>
    {
        private List<T> list;
        private int firstItemIndex;

        public AdvancedQueue()
        {
            list = new List<T>();
            firstItemIndex = 0;
        }

        public void Enqueue(T item)
        {
            list.Add(item);
        }

        public T Dequeue()
        {
            T item = Peek();
            firstItemIndex++;
            return Peek();
        }

        public T Peek()
        {
            return this[0];
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    //throw new InvalidOperationException( "Index is not in range!" );
                    return default(T);
                return list[firstItemIndex + index];
            }
        }

        public int Count
        {
            get
            {
                return list.Count - firstItemIndex;
            }
        }


        /// <summary>
        /// Returns the item in the list at the given index. Its possible that this item has allready been removed from the queue.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetItemAt(int index)
        {
            return list[index];
        }


        /// <summary>
        /// This method removes all the used tasks from the list. 
        /// This is usually a big memory operation but it is better than calling it after every Dequeue().
        /// </summary>
        public void ClearOldItems()
        {
            throw new InvalidOperationException("Not yet implemented!");
        }
    }
}
