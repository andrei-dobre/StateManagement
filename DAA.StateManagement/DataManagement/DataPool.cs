using System.Collections.Generic;
using System.Linq;

using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement.DataManagement
{
    public class DataPool<TData> : IDataPool<TData>
        where TData : IData
    {
        private DataStore<TData> dataStore;
        private NonTerminalDescriptorCompositionsStore nonTerminalDescriptorCompositionsStore;
        private ITerminalDescriptorsFlyweightFactory terminalDescriptorsFlyweightFactory;

        protected virtual DataStore<TData> Data { get => dataStore; }
        protected virtual NonTerminalDescriptorCompositionsStore NonTerminalDescriptorCompositions { get => nonTerminalDescriptorCompositionsStore; }
        protected virtual ITerminalDescriptorsFlyweightFactory TerminalDescriptorsFlyweightFactory { get => terminalDescriptorsFlyweightFactory; }


        public DataPool(ITerminalDescriptorsFlyweightFactory terminalDescriptorsFlyweightFactory)
        {
            this.terminalDescriptorsFlyweightFactory = terminalDescriptorsFlyweightFactory;

            this.dataStore = new DataStore<TData>();
            this.nonTerminalDescriptorCompositionsStore = new NonTerminalDescriptorCompositionsStore();
        }


        public bool Contains(ITerminalDescriptor descriptor)
        {
            return this.Data.Contains(descriptor as ITerminalDescriptor);
        }

        public bool Contains(INonTerminalDescriptor descriptor)
        {
            return this.NonTerminalDescriptorCompositions.Contains(descriptor);
        }


        public virtual TData Retrieve(ITerminalDescriptor descriptor)
        {
            return this.Data.Retrieve(descriptor);
        }

        public virtual IEnumerable<TData> Retrieve(INonTerminalDescriptor descriptor)
        {
            return
                this.NonTerminalDescriptorCompositions
                    .Retrieve(descriptor)
                    .Select(this.Retrieve);
        }


        public virtual IEnumerable<ITerminalDescriptor> UpdateDescriptorCompositionAndProvideAdditions(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> composition)
        {
            return null;
        }


        public void Save(ITerminalDescriptor descriptor, TData data)
        {
            this.Data.Save(descriptor, data);
        }

        public void Save(INonTerminalDescriptor descriptor, IEnumerable<TData> data)
        {
            var composition = this.DescribeAndSaveData(data);

            this.NonTerminalDescriptorCompositions.Save(descriptor, composition);
        }

        protected virtual IEnumerable<ITerminalDescriptor> DescribeAndSaveData(IEnumerable<TData> data)
        {
            return data.Select(this.DescribeAndSaveData).ToArray();
        }

        private ITerminalDescriptor DescribeAndSaveData(TData data)
        {
            var descriptor = this.TerminalDescriptorsFlyweightFactory.Create(data.DataIdentifier);

            this.Data.Save(descriptor, data);

            return descriptor;
        }


        public IEnumerable<IDescriptor> FindIntersectingDescriptors(IDescriptor descriptor)
        {
            return null;
        }
    }
}
