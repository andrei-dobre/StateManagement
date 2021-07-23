﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;
using DAA.StateManagement.Stores;

namespace DAA.StateManagement
{
    public class DataPool<TData> : IDataPool<TData>
        where TData : IData
    {
        public DataPool(ITerminalDescriptorsFactory<TData> terminalDescriptorsFactory, IDataManipulator<TData> dataManipulator)
        {
            TerminalDescriptorsFactory = terminalDescriptorsFactory;

            Data = new DataStore<TData>(dataManipulator);
            Compositions = new CompositionsStore();
        }

        protected virtual DataStore<TData> Data { get;  }

        protected virtual CompositionsStore Compositions { get; }

        protected virtual ITerminalDescriptorsFactory<TData> TerminalDescriptorsFactory { get; }

        public bool Contains(ITerminalDescriptor descriptor)
        {
            return Data.Contains(descriptor);
        }

        public bool Contains(INonTerminalDescriptor descriptor)
        {
            return Compositions.Contains(descriptor);
        }

        public virtual TData Retrieve(ITerminalDescriptor descriptor)
        {
            return Data.Retrieve(descriptor);
        }

        public virtual IEnumerable<TData> Retrieve(INonTerminalDescriptor descriptor)
        {
            return Compositions.Retrieve(descriptor).Select(Retrieve).ToArray();
        }

        public async Task SaveAsync(ITerminalDescriptor descriptor, IInstanceRetrievalContext<TData> retrievalContext)
        {
            await SaveAsync(descriptor, retrievalContext, doAfterDataAdded: null);
        }
        
        public async Task SaveAsync(ITerminalDescriptor descriptor, IInstanceRetrievalContext<TData> retrievalContext, Action doAfterDataAdded)
        {
            var isExistingInstance = !Data.Add(descriptor, retrievalContext.Data);
            
            doAfterDataAdded?.Invoke();
            await retrievalContext.CompleteReconstitutionAsync();

            if (isExistingInstance)
            {
                Data.Update(descriptor, retrievalContext.Data);
            }
        }

        public async Task SaveAsync(INonTerminalDescriptor descriptor, ICollectionRetrievalContext<TData> retrievalContext)
        {
            await SaveAsync(descriptor, retrievalContext, doAfterDataAdded: null);
        }

        public async Task SaveAsync(INonTerminalDescriptor descriptor, ICollectionRetrievalContext<TData> retrievalContext, Action doAfterDataAdded)
        {
            var composition = await SaveAndDescribeAsync(retrievalContext, doAfterDataAdded);
            
            Compositions.Save(descriptor, composition);
        }

        public async Task SaveAsync(ICollectionRetrievalContext<TData> retrievalContext)
        {
            await SaveAsync(retrievalContext, doAfterDataAdded: null);
        }

        public async Task SaveAsync(ICollectionRetrievalContext<TData> retrievalContext, Action doAfterDataAdded)
        {
            await SaveAndDescribeAsync(retrievalContext, doAfterDataAdded);
        }

        public IEnumerable<IDescriptor> FindIntersectingDescriptors(IDescriptor descriptor)
        {
            return RetrieveAllDescriptors().Where(_ => _.Intersects(descriptor)).ToArray();
        }

        public virtual IEnumerable<ITerminalDescriptor> UpdateCompositionAndProvideAdditions(INonTerminalDescriptor descriptor, IEnumerable<ITerminalDescriptor> composition)
        {
            return Compositions.UpdateAndProvideAdditions(descriptor, composition);
        }
        
        protected virtual async Task<IEnumerable<ITerminalDescriptor>> SaveAndDescribeAsync(ICollectionRetrievalContext<TData> retrievalContext, Action doAfterDataAdded)
        {
            var composition = new List<ITerminalDescriptor>();
            var existingInstancesByDescriptor = new Dictionary<ITerminalDescriptor, TData>();

            foreach (var data in retrievalContext.Data)
            {
                var terminalDescriptor = TerminalDescriptorsFactory.Create(data);
                
                var isExistingInstance = !Data.Add(terminalDescriptor, data);
                if (isExistingInstance)
                {
                    existingInstancesByDescriptor[terminalDescriptor] = data;
                }

                composition.Add(terminalDescriptor);
            }

            doAfterDataAdded?.Invoke();
            await retrievalContext.CompleteReconstitutionAsync();

            foreach (var existingInstanceByDescriptor in existingInstancesByDescriptor)
            {
                Data.Update(existingInstanceByDescriptor.Key, existingInstanceByDescriptor.Value);
            }

            return composition;
        }

        protected virtual IEnumerable<IDescriptor> RetrieveAllDescriptors()
        {
            return Data
                .RetrieveDescriptors().Cast<IDescriptor>()
                .Concat(Compositions.RetrieveDescriptors());
        }
    }
}
