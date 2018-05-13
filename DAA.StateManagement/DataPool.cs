using System.Collections.Generic;

using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class DataPool<TData> : IDataPool<TData>
        where TData : IData
    {
        private ITerminalDescriptorsFlyweightFactory _terminalDescriptorsFlyweightFactory;

        protected virtual ITerminalDescriptorsFlyweightFactory TerminalDescriptorsFlyweightFactory { get => _terminalDescriptorsFlyweightFactory; }

        public DataPool(ITerminalDescriptorsFlyweightFactory terminalDescriptorsFlyweightFactory)
        {
            this._terminalDescriptorsFlyweightFactory = terminalDescriptorsFlyweightFactory;
        }


        public void Store(IDescriptor descriptor, IEnumerable<TData> data)
        {
            var terminalDescriptorsOfStoredData = this.StoreDataAndProvideTerminalDescriptors(data);

            this.StoreNonTerminalDescriptorIfNonTerminal(descriptor, terminalDescriptorsOfStoredData);
        }

        public bool Contains(IDescriptor descriptor)
        {
            if (descriptor is INonTerminalDescriptor)
            {
                return this.ContainsNonTerminalDescriptor(descriptor as INonTerminalDescriptor);
            }

            return this.ContainsTerminalDescriptor(descriptor as ITerminalDescriptor);
        }

        public virtual IEnumerable<ITerminalDescriptor> UpdateNonTerminalDescriptorComposition(INonTerminalDescriptor nonTerminalDescriptor, IEnumerable<ITerminalDescriptor> terminalDescriptors)
        {
            //
            // Should update the composition of the registered descriptor, not the
            // received instance.
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

        protected virtual void StoreNonTerminalDescriptorIfNonTerminal(IDescriptor descriptor, IEnumerable<ITerminalDescriptor> terminalDescriptors)
        {
            if (descriptor is INonTerminalDescriptor)
            {
                this.StoreNonTerminalDescriptor(descriptor as INonTerminalDescriptor, terminalDescriptors);
            }
        }

        protected virtual void StoreNonTerminalDescriptor(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> terminalDescriptors)
        {
            this.RegisterNonTerminalDescriptorIfUnknown(descriptor);
            this.UpdateNonTerminalDescriptorComposition(descriptor, terminalDescriptors);
        }

        protected virtual void RegisterNonTerminalDescriptorIfUnknown(INonTerminalDescriptor nonTerminalDescriptor)
        {
            if (!this.ContainsNonTerminalDescriptor(nonTerminalDescriptor))
            {
                this.RegisterNonTerminalDescriptor(nonTerminalDescriptor);
            }
        }


        protected virtual bool ContainsTerminalDescriptor(ITerminalDescriptor descriptor)
        {
            return false;
        }

        protected virtual bool ContainsNonTerminalDescriptor(INonTerminalDescriptor nonTerminalDescriptor)
        {
            return false;
        }

        protected virtual void RegisterTerminalDescriptor(ITerminalDescriptor descriptor, IData data)
        { }

        protected virtual void RegisterNonTerminalDescriptor(INonTerminalDescriptor nonTerminalDescriptor)
        { }

        protected virtual void UpdateData(ITerminalDescriptor descriptor, TData data)
        { }
    }
}
