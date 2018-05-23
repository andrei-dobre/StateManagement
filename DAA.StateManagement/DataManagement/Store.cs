using System;
using System.Collections.Generic;

namespace DAA.StateManagement.DataManagement
{
    // TODO: Move to the framework.
    public abstract class Store<TKey, TValue>
    {
        private IDictionary<TKey, TValue> keyToValueMap;

        protected IDictionary<TKey, TValue> KeyToValueMap { get => this.keyToValueMap; }


        public Store()
        {
            this.keyToValueMap = new Dictionary<TKey, TValue>();
        }


        public virtual bool Contains(TKey key)
        {
            return this.KeyToValueMap.ContainsKey(key);
        }

        public virtual TValue Retrieve(TKey key)
        {
            return this.KeyToValueMap[key];
        }
        
        public virtual void Save(TKey key, TValue value)
        {
            if (this.Contains(key))
            {
                this.Update(key, value);
            }
            else
            {
                this.Insert(key, value);
            }
        }

        public virtual void Insert(TKey key, TValue value)
        {
            if (null == value)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (this.Contains(key))
            {
                throw new InvalidOperationException();
            }

            this.Set(key, value);
        }
        
        public abstract void Update(TKey key, TValue value);
        

        protected virtual void Set(TKey key, TValue value)
        {
            this.KeyToValueMap[key] = value;
        }

        protected virtual IEnumerable<TKey> RetrieveKeys()
        {
            return this.KeyToValueMap.Keys;
        }
    }
}
