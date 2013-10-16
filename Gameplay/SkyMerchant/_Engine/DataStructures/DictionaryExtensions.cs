using System;
using System.Collections.Generic;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.DataStructures
{
    public static class DictionaryExtensions
    {
        public static K GetOrCreate<T, K>(this IDictionary<T, K> dictionary, T key, Func<K> createNew )
        {
            K val;
            if (dictionary.TryGetValue(key, out val)) return val;

            val = createNew();
            dictionary.Add(key,val);

            return val;

        }
    }
}