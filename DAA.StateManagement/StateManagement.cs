using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class StateManagement : IStateManagement
    {
        public virtual IStateManagementSystemsCatalog StateManagementSystemsCatalog { get; }
        public virtual StateManagementSystemBuildingDirector SystemBuildingDirector { get; }


        public StateManagement()
        {
            StateManagementSystemsCatalog = new StateManagementSystemsCatalog();
            SystemBuildingDirector = new StateManagementSystemBuildingDirector();
        }


        public async Task FillCollectionAsync<TData>(ICollection<TData> collection, INonTerminalDescriptor descriptor) 
            where TData : IData
        {
            await GetRepository<TData>().FillCollectionAsync(collection, descriptor);
        }

        public bool IsCollectionRegistered<TData>(ICollection<TData> collection) 
            where TData : IData
        {
            return GetRepository<TData>().IsCollectionRegistered(collection);
        }

        public void DropCollection<TData>(ICollection<TData> collection) 
            where TData : IData
        {
            GetRepository<TData>().DropCollection(collection);
        }

        public virtual IDataRepository<TData> GetRepository<TData>()
            where TData : IData
        {
            return StateManagementSystemsCatalog.Retrieve<TData>().Repository;
        }

        public virtual void Register<TData>(IStateManagementSystemBuilder<TData> builder)
            where TData : IData
        {
            SystemBuildingDirector.Build(builder);
            StateManagementSystemsCatalog.Register(builder.ExtractResult());
        }
    }
}
