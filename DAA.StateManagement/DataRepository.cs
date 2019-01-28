using System.Collections.Generic;
using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class DataRepository<TData> : IDataRepository<TData>
        where TData : IData
    {
        public virtual IDataRetriever<TData> DataRetriever { get; }
        public virtual IDataPool<TData> DataPool { get; }
        public virtual ICollectionsManager<TData> CollectionsManager { get; }


        public DataRepository(IDataRetriever<TData> dataRetriever, IDataPool<TData> dataPool, ICollectionsManager<TData> collectionsManager)
        {
            DataRetriever = dataRetriever;
            DataPool = dataPool;
            CollectionsManager = collectionsManager;
        }


        public async Task FillCollectionAsync(IFillCollectionArgs<TData> args)
        {
            await AcquireMissingData(args.Descriptor);
            await CollectionsManager.FillCollectionAsync(args);
        }

        public async Task ChangeBuilderAsync(ICollection<TData> collection, IDataBuilder<TData> builder)
        {
            await CollectionsManager.ChangeBuilderAsync(collection, builder);
        }

        public bool IsCollectionRegistered(ICollection<TData> collection)
        {
            return CollectionsManager.IsCollectionRegistered(collection);
        }

        public bool IsCollectionRegisteredWithDescriptor(ICollection<TData> collection, INonTerminalDescriptor descriptor)
        {
            return CollectionsManager.IsCollectionRegisteredWithDescriptor(collection, descriptor);
        }

        public void DropCollection(ICollection<TData> collection)
        {
            CollectionsManager.DropCollection(collection);
        }

        public virtual async Task AcquireMissingData(INonTerminalDescriptor descriptor)
        {
            if (!DataPool.Contains(descriptor))
            {
                var data = await DataRetriever.RetrieveAsync(descriptor);

                DataPool.Save(descriptor, data);
            }
        }
    }
}
