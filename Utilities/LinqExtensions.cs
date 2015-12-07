using System;
using System.Collections.Generic;

namespace MHGameWork.TheWizards
{
    /// <summary>
    /// More link goodies!
    /// </summary>
    public static class LinqExtensions
    {
        public static T Alter<T>(this T obj, Action<T> act)
        {
            act(obj);
            return obj;
        }

        /// <summary>
        /// Basic Maybe monad implementation
        /// </summary>
        public static TResult With<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator)
            where TResult : class
            where TInput : class
        {
            if (o == null) return null;
            return evaluator(o);
        }
        /// <summary>
        /// Basic Maybe monad implementation
        /// </summary>
        public static TResult? With<TInput, TResult>(this TInput? o, Func<TInput, TResult> evaluator)
            where TResult : struct
            where TInput : struct
        {
            return o.HasValue ? evaluator(o.Value) : (TResult?)null;
        }

        /// <summary>
        /// MHGW addition
        /// </summary>
        public static void With<TInput>(this TInput o, Action<TInput> evaluator)
            where TInput : class
        {
            if (o == null) return;
            evaluator(o);
        }

        /// <summary>
        /// Returns with a default value
        /// </summary>
        public static TResult Return<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator, TResult failureValue) where TInput : class
        {
            if (o == null) return failureValue;
            return evaluator(o);
        }

        public static string Print(this string str)
        {
            Console.WriteLine(str);
            return str;
        }
        public static string Print(this string str, string formattedString)
        {
            Console.WriteLine(formattedString, str);
            return str;
        }
        public static IEnumerable<T> Print<T>(this IEnumerable<T> list)
        {
            foreach (var l in list)
                Console.WriteLine(l);

            return list;
        }

        public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (var l in list)
                action(l);
        }

        public static T[] Single<T>(this T obj)
        {
            return new[] { obj };
        }
    }
}