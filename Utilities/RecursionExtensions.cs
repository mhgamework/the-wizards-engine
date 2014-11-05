using System;
using System.Collections.Generic;

namespace ObjectVersioning
{
    public static class RecursionExtensions
    {
        /// <summary>
        /// Walks to a tree depth first.
        /// Does not return the root
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="start"></param>
        /// <param name="getChildren"></param>
        /// <returns></returns>
        public static IEnumerable<T> DepthFirst<T>(this T root, Func<T, IEnumerable<T>> getChildren) where T : class
        {
            var stack = new Stack<IEnumerator<T>>();

            stack.Push(getChildren(root).GetEnumerator());
            while (stack.Count > 0)
            {
                var en = stack.Peek();

                if (!en.MoveNext())
                {
                    // Empty
                    stack.Pop();
                    continue;
                }

                var el = en.Current;

                yield return el;

                stack.Push(getChildren(el).GetEnumerator());
            }
        }

        /// <summary>
        /// Walks to a tree breadth first.
        /// Does not return the root
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="start"></param>
        /// <param name="getChildren"></param>
        /// <returns></returns>
        public static IEnumerable<T> BreadthFirst<T>(this T root, Func<T, IEnumerable<T>> getChildren) where T : class
        {
            var stack = new Queue<IEnumerable<T>>();

            stack.Enqueue(getChildren(root));
            while (stack.Count > 0)
            {
                var el = stack.Dequeue();
                foreach (var c in el)
                {
                    yield return c;
                    stack.Enqueue(getChildren(c));
                }
            }
        }

        public static T Root<T>(this T child, Func<T, T> getParent)
        {
            while (getParent(child) != null) child = getParent(child);
            return child;
        }

        /// <summary>
        /// NOTE for cleanup: this is actually thesame as depthfirst or depthfirst for a child collection of 1 element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <param name="getParent"></param>
        /// <returns></returns>
        public static IEnumerable<T> Parents<T>(this T child, Func<T, T> getParent) where T : class
        {
            while (getParent(child) != null)
            {
                child = getParent(child);
                yield return child;
            }
        }
    }
}