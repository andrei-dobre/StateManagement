using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DAA.StateManagement.Stores
{
    public abstract class Store<TKey, TValue>
    {
        public Store()
        {
            KeyToValueMap = new ConcurrentDictionary<TKey, TValue>();
        }

        protected ConcurrentDictionary<TKey, TValue> KeyToValueMap { get; }

        public virtual bool Contains(TKey key)
        {
            return KeyToValueMap.ContainsKey(key);
        }

        public virtual TValue Retrieve(TKey key)
        {
            return KeyToValueMap[key];
        }
        
        public virtual void Save(TKey key, TValue value)
        {
            if (!Add(key, value))
            {
                Update(key, value);
            }
        }

        public virtual void Insert(TKey key, TValue value)
        {
            if (!Add(key, value))
            {
                throw new InvalidOperationException();
            }
        }

        public virtual bool Add(TKey key, TValue value)
        {
            if (null == value)
            {
                throw new ArgumentNullException(nameof(value));
            }
            
            return KeyToValueMap.TryAdd(key, value);
        }

        public abstract void Update(TKey key, TValue value);

        public virtual IEnumerable<TKey> RetrieveKeys()
        {
            return KeyToValueMap.Keys;
        }

        protected virtual void Set(TKey key, TValue value)
        {
            KeyToValueMap.AddOrUpdate(key, value, (keyDuplicate, valueDuplicate) => value);
        }
    }
}
