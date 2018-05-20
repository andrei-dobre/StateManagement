using System;
using System.Collections.Generic;

namespace DAA.StateManagement.DataManagement
{
    public abstract class Store<TKey, TValue>
    {
        private IDictionary<TKey, TValue> keyToValueMap;

        protected IDictionary<TKey, TValue> KeyToValueMap { get => this.keyToValueMap; }


        public Store()
        {
            this.keyToValueMap = new Dictionary<TKey, TValue>();
        }


        public virtual bool Contains(TKey descriptor)
        {
            return this.KeyToValueMap.ContainsKey(descriptor);
        }

        public virtual TValue Retrieve(TKey descriptor)
        {
            return this.KeyToValueMap[descriptor];
        }

        public virtual void Save(TKey descriptor, TValue data)
        {
            if (this.Contains(descriptor))
            {
                this.Update(descriptor, data);
            }
            else
            {
                this.Insert(descriptor, data);
            }
        }

        public virtual void Insert(TKey descriptor, TValue data)
        {
            if (null == data)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (this.Contains(descriptor))
            {
                throw new InvalidOperationException();
            }

            this.KeyToValueMap.Add(descriptor, data);
        }
        
        public abstract void Update(TKey descriptor, TValue data);
    }
}
