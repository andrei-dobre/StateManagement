using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DAA.StateManagement.Stores
{
    public abstract class Store<TKey, TValue>
    {
        protected IDictionary<TKey, TValue> KeyToValueMap { get; set; }


        public Store()
        {
            KeyToValueMap = new ConcurrentDictionary<TKey, TValue>();
        }


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
            if (Contains(key))
            {
                Update(key, value);
            }
            else
            {
                Insert(key, value);
            }
        }

        public virtual void Insert(TKey key, TValue value)
        {
            if (null == value)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (Contains(key))
            {
                throw new InvalidOperationException();
            }

            Set(key, value);
        }
        
        public abstract void Update(TKey key, TValue value);

        public virtual IEnumerable<TKey> RetrieveKeys()
        {
            return KeyToValueMap.Keys;
        }

        protected virtual void Set(TKey key, TValue value)
        {
            KeyToValueMap[key] = value;
        }
    }
}
