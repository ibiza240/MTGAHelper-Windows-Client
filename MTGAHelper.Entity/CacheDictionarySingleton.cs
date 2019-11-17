//using System;
//using System.Collections.Generic;
//using MTGAHelper.Lib.Config;

//namespace MTGAHelper.Lib.Cache
//{
//    public class CacheDictionarySingleton<K, V>
//    {
//        object lockCache = new object();

//        Dictionary<K, V> cache;// = new Dictionary<K, V>();

//        public void Set(K key, V obj)
//        {
//            lock (lockCache)
//                cache[key] = obj;
//        }

//        public V Get(K key)
//        {
//            lock (lockCache)
//            {
//                return cache[key];
//            }
//        }

//        public void PopulateIfNotSet(Func<Dictionary<K, V>> populateFunction)
//        {
//            lock (lockCache)
//            {
//                if (IsNotSet())
//                {
//                    lock (lockCache)
//                        cache = populateFunction();
//                }
//            }
//        }

//        public bool IsNotSet()
//        {
//            lock (lockCache)
//                return cache == null;
//        }

//        public ICollection<V> GetValues()
//        {
//            lock (lockCache)
//                return cache.Values;
//        }

//        public bool ContainsKey(K key)
//        {
//            lock (lockCache)
//                return cache.ContainsKey(key);
//        }

//        public void Remove(K key)
//        {
//            lock (lockCache)
//                cache.Remove(key);
//        }
//    }
//}
