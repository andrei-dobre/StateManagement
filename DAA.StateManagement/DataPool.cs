using System;
using System.Collections.Generic;
using System.Linq;

using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class DataPool<TData> : IDataPool<TData>
        where TData : IData
    {
        private IDictionary<ITerminalDescriptor, TData> _terminalDescriptorToDataMap;
        private ITerminalDescriptorsFlyweightFactory _terminalDescriptorsFlyweightFactory;


        protected virtual IDictionary<ITerminalDescriptor, TData> TerminalDescriptorToDataMap { get => _terminalDescriptorToDataMap; }
        protected virtual ITerminalDescriptorsFlyweightFactory TerminalDescriptorsFlyweightFactory { get => _terminalDescriptorsFlyweightFactory; }


        public DataPool(ITerminalDescriptorsFlyweightFactory terminalDescriptorsFlyweightFactory)
        {
            this._terminalDescriptorToDataMap = new Dictionary<ITerminalDescriptor, TData>();
            this._terminalDescriptorsFlyweightFactory = terminalDescriptorsFlyweightFactory;
        }


        public virtual TData Retrieve(ITerminalDescriptor descriptor)
        {
            return this.TerminalDescriptorToDataMap[descriptor];
        }

        public virtual IEnumerable<TData> Retrieve(INonTerminalDescriptor nonTerminalDescriptor)
        {
            return
                this.RetrieveCompositionOfNonTerminalDescriptor(nonTerminalDescriptor)
                    .Select(terminalDescriptor => this.Retrieve(terminalDescriptor));
        }

        public bool Contains(IDescriptor descriptor)
        {
            if (descriptor is INonTerminalDescriptor)
            {
                return this.ContainsNonTerminalDescriptor(descriptor as INonTerminalDescriptor);
            }

            return this.ContainsTerminalDescriptor(descriptor as ITerminalDescriptor);
        }

        public void Store(IDescriptor descriptor, IEnumerable<TData> data)
        {
            var terminalDescriptorsOfStoredData = this.StoreDataAndProvideTerminalDescriptors(data);

            this.StoreDescriptorIfNonTerminal(descriptor, terminalDescriptorsOfStoredData);
        }
        
        public virtual IEnumerable<ITerminalDescriptor> ChangeCompositionOfNonTerminalDescriptor(INonTerminalDescriptor nonTerminalDescriptor, IEnumerable<ITerminalDescriptor> terminalDescriptors)
        {
            return null;
        }


        protected virtual IEnumerable<ITerminalDescriptor> RetrieveCompositionOfNonTerminalDescriptor(INonTerminalDescriptor nonTerminalDescriptor)
        {
            return null;
        }


        protected virtual IEnumerable<ITerminalDescriptor> StoreDataAndProvideTerminalDescriptors(IEnumerable<TData> dataCollection)
        {
            var terminalDescriptors = new List<ITerminalDescriptor>();

            foreach (var data in dataCollection)
            {
                var dataIdentifier = data.DataIdentifier;
                var temrinalDescriptor = this.TerminalDescriptorsFlyweightFactory.Create(dataIdentifier);

                this.StoreTerminalDescriptor(temrinalDescriptor, data);
                terminalDescriptors.Add(temrinalDescriptor);
            }

            return terminalDescriptors;
        }


        protected virtual void StoreDescriptorIfNonTerminal(IDescriptor descriptor, IEnumerable<ITerminalDescriptor> terminalDescriptors)
        {
            if (descriptor is INonTerminalDescriptor)
            {
                this.StoreNonTerminalDescriptor(descriptor as INonTerminalDescriptor, terminalDescriptors);
            }
        }

        protected virtual void StoreTerminalDescriptor(ITerminalDescriptor descriptor, TData data)
        {
            if (this.ContainsTerminalDescriptor(descriptor))
            {
                this.UpdateData(descriptor, data);
            }
            else
            {
                this.RegisterTerminalDescriptor(descriptor, data);
            }
        }

        protected virtual void StoreNonTerminalDescriptor(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> terminalDescriptors)
        {
            this.RegisterNonTerminalDescriptorIfUnknown(descriptor);
            this.ChangeCompositionOfNonTerminalDescriptor(descriptor, terminalDescriptors);
        }


        protected virtual void RegisterTerminalDescriptor(ITerminalDescriptor descriptor, TData data)
        {
            if (null == data)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (this.ContainsTerminalDescriptor(descriptor))
            {
                throw new InvalidOperationException();
            }

            this.TerminalDescriptorToDataMap.Add(descriptor, data);
        }

        protected virtual void RegisterNonTerminalDescriptorIfUnknown(INonTerminalDescriptor nonTerminalDescriptor)
        {
            if (!this.ContainsNonTerminalDescriptor(nonTerminalDescriptor))
            {
                this.RegisterNonTerminalDescriptor(nonTerminalDescriptor);
            }
        }

        protected virtual void RegisterNonTerminalDescriptor(INonTerminalDescriptor nonTerminalDescriptor)
        { }


        protected virtual bool ContainsTerminalDescriptor(ITerminalDescriptor descriptor)
        {
            return this.TerminalDescriptorToDataMap.ContainsKey(descriptor);
        }

        protected virtual bool ContainsNonTerminalDescriptor(INonTerminalDescriptor nonTerminalDescriptor)
        {
            return false;
        }


        protected virtual void UpdateData(ITerminalDescriptor descriptor, TData data)
        { }
    }
}
