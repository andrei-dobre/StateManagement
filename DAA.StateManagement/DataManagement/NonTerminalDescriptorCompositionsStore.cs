using System.Collections.Generic;

using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement.DataManagement
{
    public class NonTerminalDescriptorCompositionsStore : Store<INonTerminalDescriptor, IEnumerable<ITerminalDescriptor>>
    {
        public override void Update(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> composition)
        { }

        public virtual IEnumerable<ITerminalDescriptor> UpdateAndProvideAdditions(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> composition)
        {
            return null;
        }
    }
}
