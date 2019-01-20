using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class StateManagement : IStateManagement
    {
        public async Task FillCollectionAsync<TData>(ICollection<TData> collection, INonTerminalDescriptor descriptor) 
            where TData : IData
        {

        }

        public bool IsCollectionRegistered<TData>(ICollection<TData> collection) 
            where TData : IData
        {
            return false;
        }

        public void DropCollection<TData>(ICollection<TData> collection) 
            where TData : IData
        {

        }
    }
}
