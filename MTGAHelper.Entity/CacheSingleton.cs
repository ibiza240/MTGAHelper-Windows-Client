using System;

namespace MTGAHelper.Lib.Cache
{
    public class CacheSingleton<T> where T : class
    {
        readonly object lockCache = new object();

        T cache;

        public void Set(T obj)
        {
            lock (lockCache)
                cache = obj;
        }

        public T Get()
        {
            lock (lockCache)
            {
                return cache;
            }
        }

        public void PopulateIfNotSet(Func<T> populateFunction)
        {
            lock (lockCache)
            {
                if (IsNotSet())
                {
                    Set(populateFunction());
                }
            }
        }

        public bool IsNotSet()
        {
            lock (lockCache)
                return cache == null;
        }
    }
}
