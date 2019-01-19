using System.Collections.Generic;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement.Stores
{
    public abstract class StateManagementStore<TDescriptor, TInformation> : Store<TDescriptor, TInformation>
        where TDescriptor : IDescriptor
    {
        public virtual IEnumerable<TDescriptor> RetrieveDescriptors()
        {
            return RetrieveKeys();
        }
    }
}
