using System.Collections.Generic;
using System.Linq;

using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement.DataManagement
{
    public class DataPool<TData> : IDataPool<TData>
        where TData : IData
    {
        protected virtual DataStore<TData> Data { get;  }
        protected virtual NonTerminalDescriptorCompositionsStore NonTerminalDescriptorCompositions { get; }
        protected virtual ITerminalDescriptorsFactory TerminalDescriptorsFactory { get; }


        public DataPool(ITerminalDescriptorsFactory terminalDescriptorsFactory, IDataManipulator<TData> dataManipulator)
        {
            TerminalDescriptorsFactory = terminalDescriptorsFactory;

            Data = new DataStore<TData>(dataManipulator);
            NonTerminalDescriptorCompositions = new NonTerminalDescriptorCompositionsStore();
        }


        public bool Contains(ITerminalDescriptor descriptor)
        {
            return Data.Contains(descriptor);
        }

        public bool Contains(INonTerminalDescriptor descriptor)
        {
            return NonTerminalDescriptorCompositions.Contains(descriptor);
        }


        public virtual TData Retrieve(ITerminalDescriptor descriptor)
        {
            return Data.Retrieve(descriptor);
        }

        public virtual IEnumerable<TData> Retrieve(INonTerminalDescriptor descriptor)
        {
            return NonTerminalDescriptorCompositions
                .Retrieve(descriptor)
                .Select(Retrieve);
        }


        public void Save(ITerminalDescriptor descriptor, TData data)
        {
            Data.Save(descriptor, data);
        }

        public void Save(INonTerminalDescriptor descriptor, IEnumerable<TData> data)
        {
            var composition = DescribeAndSave(data);
            NonTerminalDescriptorCompositions.Save(descriptor, composition);
        }

        public void Save(IEnumerable<TData> data)
        {
            DescribeAndSave(data);
        }

        protected virtual IEnumerable<ITerminalDescriptor> DescribeAndSave(IEnumerable<TData> data)
        {
            return data.Select(DescribeAndSave).ToArray();
        }

        private ITerminalDescriptor DescribeAndSave(TData data)
        {
            var descriptor = TerminalDescriptorsFactory.Create(data);

            Data.Save(descriptor, data);

            return descriptor;
        }


        public IEnumerable<IDescriptor> FindIntersectingDescriptors(IDescriptor descriptor)
        {
            return RetrieveAllDescriptors().Where(_ => _.Intersects(descriptor)).ToArray();
        }

        protected virtual IEnumerable<IDescriptor> RetrieveAllDescriptors()
        {
            var terminalDescriptors = Data.RetrieveDescriptors().Cast<IDescriptor>();
            var nonTerminalDescriptors = NonTerminalDescriptorCompositions.RetrieveDescriptors().Cast<IDescriptor>();

            return terminalDescriptors.Concat(nonTerminalDescriptors);
        }


        public virtual IEnumerable<ITerminalDescriptor> UpdateCompositionAndProvideAdditions(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> composition)
        {
            return NonTerminalDescriptorCompositions.UpdateAndProvideAdditions(descriptor, composition);
        }
    }
}
