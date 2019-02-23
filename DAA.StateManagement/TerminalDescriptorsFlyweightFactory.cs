using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public abstract class TerminalDescriptorsFlyweightFactory<TData> : ITerminalDescriptorsFactory<TData>
        where TData : IData
    {
        private IDictionary<object, ITerminalDescriptor> IntrinsicStateToTerminalDescriptorMap { get; }


        public TerminalDescriptorsFlyweightFactory()
        {
            IntrinsicStateToTerminalDescriptorMap = new ConcurrentDictionary<object, ITerminalDescriptor>();
        }


        public virtual ITerminalDescriptor Create(TData data)
        {
            var intrinsicState = data.DataIdentifier;
            var descriptor = Create(intrinsicState);

            return descriptor;
        }

        public IEnumerable<ITerminalDescriptor> Create(IEnumerable<TData> data)
        {
            return data.Select(Create);
        }

        public virtual ITerminalDescriptor Create(object intrinsicState)
        {
            if (!IntrinsicStateToTerminalDescriptorMap.ContainsKey(intrinsicState))
            {
                IntrinsicStateToTerminalDescriptorMap.Add(intrinsicState, Instantiate(intrinsicState));
            }

            return IntrinsicStateToTerminalDescriptorMap[intrinsicState];
        }

        public IEnumerable<ITerminalDescriptor> Create(IEnumerable<object> intrinsicStates)
        {
            return intrinsicStates.Select(Create);
        }

        protected abstract ITerminalDescriptor Instantiate(object intrinsicState);
    }
}
