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


        public DataPool(ITerminalDescriptorsFlyweightFactory terminalDescriptorsFlyweightFactory, IDataManipulator<TData> dataManipulator)
        {
            this.terminalDescriptorsFlyweightFactory = terminalDescriptorsFlyweightFactory;

            this.dataStore = new DataStore<TData>(dataManipulator);
            this.nonTerminalDescriptorCompositionsStore = new NonTerminalDescriptorCompositionsStore();
        }


        public bool Contains(ITerminalDescriptor descriptor)
        {
            return this.Data.Contains(descriptor);
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


        public void Save(ITerminalDescriptor descriptor, TData data)
        {
            this.Data.Save(descriptor, data);
        }

        public void Save(INonTerminalDescriptor descriptor, IEnumerable<TData> data)
        {
            var composition = this.DescribeAndSave(data);
            this.NonTerminalDescriptorCompositions.Save(descriptor, composition);
        }

        public void Save(IEnumerable<TData> data)
        {
            this.DescribeAndSave(data);
        }

        protected virtual IEnumerable<ITerminalDescriptor> DescribeAndSave(IEnumerable<TData> data)
        {
            return data.Select(this.DescribeAndSave).ToArray();
        }

        private ITerminalDescriptor DescribeAndSave(TData data)
        {
            var descriptor = this.TerminalDescriptorsFlyweightFactory.Create(data);

            this.Data.Save(descriptor, data);

            return descriptor;
        }


        public IEnumerable<IDescriptor> FindIntersectingDescriptors(IDescriptor descriptor)
        {
            return this.RetrieveAllDescriptors().Where(_ => _.Intersects(descriptor)).ToArray();
        }

        protected virtual IEnumerable<IDescriptor> RetrieveAllDescriptors()
        {
            var terminalDescriptors = this.Data.RetrieveDescriptors().Cast<IDescriptor>();
            var nonTerminalDescriptors = this.NonTerminalDescriptorCompositions.RetrieveDescriptors().Cast<IDescriptor>();

            return terminalDescriptors.Concat(nonTerminalDescriptors);
        }


        public virtual IEnumerable<ITerminalDescriptor> UpdateCompositionAndProvideAdditions(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> composition)
        {
            return this.NonTerminalDescriptorCompositions.UpdateAndProvideAdditions(descriptor, composition);
        }
    }
}
