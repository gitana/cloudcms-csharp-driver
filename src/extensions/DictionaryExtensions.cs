using System.Collections.Generic;

namespace CloudCMS
{
    public static class DictionaryExtensions
    {
        public static void AddAll<K, V>(this IDictionary<K, V> first, IDictionary<K, V> second)
        {
            if (second != null)
            {
                foreach (KeyValuePair<K, V> item in second) {
                    first[item.Key] = item.Value;
                }
            }

        }
    }
}