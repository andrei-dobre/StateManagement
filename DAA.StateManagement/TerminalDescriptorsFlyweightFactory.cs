using System.Collections.Generic;
using System.Linq;

using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class TerminalDescriptorsFlyweightFactory : ITerminalDescriptorsFlyweightFactory
    {
        private IDictionary<object, ITerminalDescriptor> IntrinsicStateToTerminalDescriptorMap { get; set; }


        public TerminalDescriptorsFlyweightFactory()
        {
            this.IntrinsicStateToTerminalDescriptorMap = new Dictionary<object, ITerminalDescriptor>();
        }


        public virtual ITerminalDescriptor Create(object intrinsicState)
        {
            if (!this.IntrinsicStateToTerminalDescriptorMap.ContainsKey(intrinsicState))
            {
                this.IntrinsicStateToTerminalDescriptorMap.Add(intrinsicState, this.Instantiate(intrinsicState));
            }

            return this.IntrinsicStateToTerminalDescriptorMap[intrinsicState];
        }

        public IEnumerable<ITerminalDescriptor> Create(IEnumerable<object> intrinsicStates)
        {
            return intrinsicStates.Select(_ => this.Create(_));
        }


        protected virtual ITerminalDescriptor Instantiate(object intrinsicState)
        {
            return null;
        }
    }
}
