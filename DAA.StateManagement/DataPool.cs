using System.Collections.Generic;

namespace DAA.StateManagement
{
    public class DataPool<TData> : IDataPool<TData>
        where TData : IData
    {
        public void Store(IDescriptor descriptor, ICollection<TData> data)
        {
            var terminalDescriptors = this.StoreDataAndProvideTerminalDescriptors(data);

            this.RegisterDescriptorIfNonterminalAndUnknown(descriptor);
            this.UpdateCompositionOfDescriptorIfNonterminal(descriptor, terminalDescriptors);
        }


        protected virtual void RegisterDescriptorIfNonterminalAndUnknown(IDescriptor descriptor)
        { }

        protected virtual ICollection<ITerminalDescriptor> StoreDataAndProvideTerminalDescriptors(ICollection<TData> data)
        {
            return null;
        }

        protected virtual void UpdateCompositionOfDescriptorIfNonterminal(IDescriptor descriptor, ICollection<ITerminalDescriptor> terminalDescriptors)
        { }
    }
}
