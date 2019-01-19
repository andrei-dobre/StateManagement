using System.Collections.Generic;
using System.Linq;

using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement.DataManagement
{
    public class NonTerminalDescriptorCompositionsStore : DataInformationStore<INonTerminalDescriptor, IEnumerable<ITerminalDescriptor>>
    {
        public override void Update(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> composition)
        {
            Set(descriptor, composition);
        }

        public virtual IEnumerable<ITerminalDescriptor> UpdateAndProvideAdditions(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> newComposition)
        {
            var additions = CompareToInitialCompositionAndFindAdditions(descriptor, newComposition);

            Update(descriptor, newComposition);

            return additions;
        }


        protected virtual IEnumerable<ITerminalDescriptor> CompareToInitialCompositionAndFindAdditions(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> newComposition)
        {
            var initialComposition = Retrieve(descriptor);
            var additions = FindDistinctAdditions(initialComposition, newComposition);

            return additions;
        }

        protected virtual IEnumerable<ITerminalDescriptor> FindDistinctAdditions(IEnumerable<ITerminalDescriptor> initialComposition, IEnumerable<ITerminalDescriptor> newComposition)
        {
            var initialCompositionSet = new HashSet<ITerminalDescriptor>(initialComposition);

            return newComposition.Where(_ => !initialCompositionSet.Contains(_)).Distinct();
        }
    }
}
