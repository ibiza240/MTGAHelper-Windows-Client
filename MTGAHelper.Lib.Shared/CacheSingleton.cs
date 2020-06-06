using System;

namespace MTGAHelper.Lib
{
    /// <summary>
    /// For use with CacheSingleton. This takes care of loading the data into the CacheSingleton.
    /// </summary>
    public interface ICacheLoader<T>
    {
        T LoadData();
    }

    public class SimpleLoader<T> : ICacheLoader<T>
    {
        readonly Func<T> loadFunc;

        public SimpleLoader(Func<T> loadFunc)
        {
            this.loadFunc = loadFunc;
        }

        public T LoadData()
        {
            return loadFunc();
        }
    }

    /// <summary>
    /// A Cache for data that doesn't change (often). T should be immutable.
    /// </summary>
    public class CacheSingleton<T> where T : class
    {
        readonly object lockSetCache = new object();
        readonly ICacheLoader<T> loader;

        T cache;

        public CacheSingleton(ICacheLoader<T> loader)
        {
            this.loader = loader;
        }

        public T Get()
        {
            // once cache is not null, it will never be set to null,
            // therefore we can return (possibly stale) value as long as it's not null.
            return cache ?? ReloadInternal(false);
        }

        public void Reload()
        {
            ReloadInternal(true);
        }

        T ReloadInternal(bool ifNotNull)
        {
            lock (lockSetCache)
            {
                // checks for null (again) inside the lock to prevent double loading
                if (ifNotNull || cache == null)
                    cache = loader.LoadData();
                return cache;
            }
        }

        public void Set(T newValue)
        {
            if (newValue == null)
                throw new ArgumentNullException(nameof(newValue));

            lock (lockSetCache)
            {
                cache = newValue;
            }
        }
    }
}
