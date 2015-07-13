using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using pb;

namespace Test.Test_Basic
{
    public static class Test_CollectionsConcurrent
    {
        public static void Test_ConcurrentDictionary_01()
        {
            ConcurrentDictionary<int, string> dictionary = new ConcurrentDictionary<int, string>();
            dictionary.GetOrAdd(1, "toto1");
            dictionary.GetOrAdd(1, "toto2");
            dictionary.GetOrAdd(2, "tata");
            dictionary.GetOrAdd(3, "tutu");
            dictionary.GetOrAdd(4, "titi");
            foreach (KeyValuePair<int, string> value in dictionary)
            {
                Trace.WriteLine("{0} - \"{1}\"", value.Key, value.Value);
            }
        }

        public static void Test_ConcurrentDictionary_02()
        {
            ConcurrentDictionary<int, string> dictionary = new ConcurrentDictionary<int, string>();
            // Func<TKey, TValue, TValue> updateValueFactory : Fonction utilisée pour générer une nouvelle valeur pour une clé existante en fonction de la valeur existante de la clé
            dictionary.AddOrUpdate(1, "toto1", (id, text) => "toto1");
            dictionary.AddOrUpdate(1, "toto2", (id, text) => "toto2");
            dictionary.AddOrUpdate(2, "tata", (id, text) => "tata");
            dictionary.AddOrUpdate(3, "tutu", (id, text) => "tutu");
            dictionary.AddOrUpdate(4, "titi", (id, text) => "titi");
            foreach (KeyValuePair<int, string> value in dictionary)
            {
                Trace.WriteLine("{0} - \"{1}\"", value.Key, value.Value);
            }
        }

        public static void Test_ConcurrentDictionary_03()
        {
            ConcurrentDictionary<int, string> dictionary = new ConcurrentDictionary<int, string>();
            // Func<TKey, TValue, TValue> updateValueFactory : Fonction utilisée pour générer une nouvelle valeur pour une clé existante en fonction de la valeur existante de la clé
            dictionary.TryAdd(1, "toto1");
            dictionary.TryAdd(1, "toto2");
            dictionary.TryAdd(2, "tata");
            dictionary.TryAdd(3, "tutu");
            dictionary.TryAdd(4, "titi");
            foreach (KeyValuePair<int, string> value in dictionary)
            {
                Trace.WriteLine("{0} - \"{1}\"", value.Key, value.Value);
            }
        }

        public static void Test_ConcurrentDictionary_04()
        {
            ConcurrentDictionary<int, string> dictionary = new ConcurrentDictionary<int, string>();
            dictionary.GetOrAdd(1, "toto1");
            dictionary.GetOrAdd(1, "toto2");
            dictionary.GetOrAdd(2, "tata");
            dictionary.GetOrAdd(3, "tutu");
            dictionary.GetOrAdd(4, "titi");
            foreach (KeyValuePair<int, string> value in dictionary)
            {
                Trace.WriteLine("{0} - \"{1}\"", value.Key, value.Value);
                if (value.Key == 2)
                {
                    Trace.Write("TryRemove key 2");
                    string s;
                    if (dictionary.TryRemove(2, out s))
                        Trace.Write(" ok");
                    else
                        Trace.Write(" error");
                    Trace.WriteLine();
                }
            }
            Trace.WriteLine("read again");
            foreach (KeyValuePair<int, string> value in dictionary)
            {
                Trace.WriteLine("{0} - \"{1}\"", value.Key, value.Value);
            }
        }
    }
}
