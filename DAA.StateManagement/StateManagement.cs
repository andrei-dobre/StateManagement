﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class StateManagement : IStateManagement
    {
        public StateManagement()
        {
            StateManagementSystemsCatalog = new StateManagementSystemsCatalog();
            SystemBuildingDirector = new StateManagementSystemBuildingDirector();
        }

        public virtual IStateManagementSystemsCatalog StateManagementSystemsCatalog { get; }

        public virtual StateManagementSystemBuildingDirector SystemBuildingDirector { get; }

        public async Task<TData> RetrieveAsync<TData>(ITerminalDescriptor descriptor) 
            where TData : IData
        {
            return await GetRepository<TData>().RetrieveAsync(descriptor);
        }

        public async Task<TData> RetrieveAsync<TData>(ITerminalDescriptor descriptor, IDataBuilder<TData> builder)
            where TData : IData
        {
            return await GetRepository<TData>().RetrieveAsync(descriptor, builder);
        }

        public async Task<IEnumerable<TData>> RetrieveAsync<TData>(INonTerminalDescriptor descriptor) where TData : IData
        {
            return await GetRepository<TData>().RetrieveAsync(descriptor);
        }

        public async Task<IEnumerable<TData>> RetrieveAsync<TData>(INonTerminalDescriptor descriptor, IDataBuilder<TData> builder) where TData : IData
        {
            return await GetRepository<TData>().RetrieveAsync(descriptor, builder);
        }

        public async Task FillCollectionAsync<TData>(ICollection<TData> collection, INonTerminalDescriptor descriptor) 
            where TData : IData
        {
            var avoidClearingPopulatedCollection = IsCollectionRegisteredWithDescriptor(collection, descriptor) && collection.Count != 0;
            if (avoidClearingPopulatedCollection) return;

            await GetRepository<TData>().FillCollectionAsync(new FillCollectionArgs<TData>(collection, descriptor));
        }

        public async Task FillCollectionAsync<TData>(ICollection<TData> collection, INonTerminalDescriptor descriptor, IDataBuilder<TData> builder)
            where TData : IData
        {
            var avoidClearingPopulatedCollection = IsCollectionRegisteredWithDescriptor(collection, descriptor) && collection.Count != 0;
            if (avoidClearingPopulatedCollection)
            {
                await GetRepository<TData>().ChangeBuilderAsync(collection, builder);
            }
            else
            {
                await GetRepository<TData>().FillCollectionAsync(new FillCollectionArgs<TData>(collection, descriptor, builder));
            }
        }

        public async Task ChangeBuilderAsync<TData>(ICollection<TData> collection, IDataBuilder<TData> builder)
            where TData : IData
        {
            await GetRepository<TData>().ChangeBuilderAsync(collection, builder);
        }

        public bool IsCollectionRegistered<TData>(ICollection<TData> collection) 
            where TData : IData
        {
            return GetRepository<TData>().IsCollectionRegistered(collection);
        }

        public virtual bool IsCollectionRegisteredWithDescriptor<TData>(ICollection<TData> collection, INonTerminalDescriptor descriptor)
            where TData : IData
        {
            return GetRepository<TData>().IsCollectionRegisteredWithDescriptor(collection, descriptor);
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
