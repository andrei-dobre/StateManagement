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
        
        public virtual void Save(TKey key, TValue data)
        {
            if (this.Contains(key))
            {
                this.Update(key, data);
            }
            else
            {
                this.Insert(key, data);
            }
        }

        public virtual void Insert(TKey key, TValue data)
        {
            if (null == data)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (this.Contains(key))
            {
                throw new InvalidOperationException();
            }

            this.KeyToValueMap.Add(key, data);
        }
        
        public abstract void Update(TKey key, TValue data);


        protected virtual IEnumerable<TKey> RetrieveKeys()
        {
            return this.KeyToValueMap.Keys;
        }
    }
}
