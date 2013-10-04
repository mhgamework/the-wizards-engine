﻿using System;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.SkyMerchant._Engine
{
    /// <summary>
    /// Some more epic functional programming goodies!
    /// </summary>
    public static class LinqExtensions
    {
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
        /// Returns with a default value
        /// </summary>
        public static TResult Return<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator, TResult failureValue) where TInput : class
        {
            if (o == null) return failureValue;
            return evaluator(o);
        }

        public static IEnumerable<T> Print<T>(this IEnumerable<T> list)
        {
            foreach (var l in list)
                Console.WriteLine(l);

            return list;
        }

    }
}