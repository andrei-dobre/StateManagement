using System.Collections.Generic;
using System.Linq;
using DAA.StateManagement.Interfaces;
using DAA.StateManagement.Stores;

namespace DAA.StateManagement
{
    public class DataPool<TData> : IDataPool<TData>
        where TData : IData
    {
        protected virtual DataStore<TData> Data { get;  }
        protected virtual CompositionsStore Compositions { get; }
        protected virtual ITerminalDescriptorsFactory<TData> TerminalDescriptorsFactory { get; }


        public DataPool(ITerminalDescriptorsFactory<TData> terminalDescriptorsFactory, IDataManipulator<TData> dataManipulator)
        {
            TerminalDescriptorsFactory = terminalDescriptorsFactory;

            Data = new DataStore<TData>(dataManipulator);
            Compositions = new CompositionsStore();
        }


        public bool Contains(ITerminalDescriptor descriptor)
        {
            return Data.Contains(descriptor);
        }

        public bool Contains(INonTerminalDescriptor descriptor)
        {
            return Compositions.Contains(descriptor);
        }

        public virtual TData Retrieve(ITerminalDescriptor descriptor)
        {
            return Data.Retrieve(descriptor);
        }

        public virtual IEnumerable<TData> Retrieve(INonTerminalDescriptor descriptor)
        {
            return Compositions.Retrieve(descriptor).Select(Retrieve).ToArray();
        }

        public void Save(ITerminalDescriptor descriptor, TData data)
        {
            Data.Save(descriptor, data);
        }

        public void Save(INonTerminalDescriptor descriptor, IEnumerable<TData> data)
        {
            var composition = DescribeAndSave(data);

            Compositions.Save(descriptor, composition);
        }

        public void Save(IEnumerable<TData> data)
        {
            DescribeAndSave(data);
        }

        public IEnumerable<IDescriptor> FindIntersectingDescriptors(IDescriptor descriptor)
        {
            return RetrieveAllDescriptors().Where(_ => _.Intersects(descriptor)).ToArray();
        }

        public virtual IEnumerable<ITerminalDescriptor> UpdateCompositionAndProvideAdditions(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> composition)
        {
            return Compositions.UpdateAndProvideAdditions(descriptor, composition);
        }

        private ITerminalDescriptor DescribeAndSave(TData data)
        {
            var descriptor = TerminalDescriptorsFactory.Create(data);

            Data.Save(descriptor, data);

            return descriptor;
        }

        protected virtual IEnumerable<ITerminalDescriptor> DescribeAndSave(IEnumerable<TData> data)
        {
            return data.Select(DescribeAndSave).ToArray();
        }

        protected virtual IEnumerable<IDescriptor> RetrieveAllDescriptors()
        {
            var terminalDescriptors = Data.RetrieveDescriptors().Cast<IDescriptor>();
            var nonTerminalDescriptors = Compositions.RetrieveDescriptors().Cast<IDescriptor>();

            return terminalDescriptors.Concat(nonTerminalDescriptors);
        }
    }
}
