using System;
using System.Collections.Generic;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class StateManagementSystemsCatalog : IStateManagementSystemsCatalog
    {
        private IDictionary<Type, object> StateManagementSystemsByType { get; }


        public StateManagementSystemsCatalog()
        {
            StateManagementSystemsByType = new Dictionary<Type, object>();
        }


        public void Register<TData>(IStateManagementSystem<TData> stateManagementSystem) 
            where TData : IData
        {
            var type = typeof(TData);

            if (!StateManagementSystemsByType.ContainsKey(type))
            {
                StateManagementSystemsByType.Add(type, stateManagementSystem);
            }
            else
            {
                throw new InvalidOperationException(
                    $"A state management system has already been registered for {type.FullName}");
            }
        }

        public IStateManagementSystem<TData> Retrieve<TData>() 
            where TData : IData
        {
            var type = typeof(TData);

            if (StateManagementSystemsByType.ContainsKey(type))
            {
                return StateManagementSystemsByType[type] as IStateManagementSystem<TData>;
            }

            throw new InvalidOperationException(
                $"A state management system was not registered for {type.FullName}");
        }
    }
}
