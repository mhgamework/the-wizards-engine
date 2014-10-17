using System;
using System.Collections.Generic;
using System.Linq;

namespace MHGameWork.TheWizards.GodGame._Engine
{
    /// <summary>
    /// Represents a way to detect changes to a list, somewhat of an inverse observeable or a conversion from pull to push.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListObserver<T>
    {
        private HashSet<T> oldSet = new HashSet<T>();

        public ListObserver()
        {

        }

        /*TODO: could add RX version of the monitored list here
         * public ObservableCollection<T> ObservableCollection
        {
            get { return observableCollection; }
            set { observableCollection = value; }
        }*/

        private List<T> removed = new List<T>();
        private List<T> added = new List<T>();

        public IEnumerable<T> Removed { get { return removed; } }
        public IEnumerable<T> Added { get { return added; } }

        /// <summary>
        /// Idea: instead of building a list of added and removed, simply throw events/invoke handlers?
        /// </summary>
        /// <param name="list"></param>
        public void UpdateList(IEnumerable<T> list)
        {
            var arr = list.ToArray();
            var newSet = new HashSet<T>(arr);
            if (newSet.Count() != arr.Count())
                throw new InvalidOperationException("This does not work on duplicates!");

            removed.Clear();
            added.Clear();



            removed.AddRange(oldSet.Where(c => !newSet.Contains(c)));
            added.AddRange(newSet.Where(c => !oldSet.Contains(c)));

            oldSet = newSet;

        }
    }
}