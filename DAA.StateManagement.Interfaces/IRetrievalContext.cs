using System;
using System.Threading.Tasks;

namespace DAA.StateManagement.Interfaces
{
    public interface IRetrievalContext
    {
        event EventHandler DataAddedEvent;

        event EventHandler DataReconstitutedEvent;

        void PublishDataAddedEvent();
        
        void PublishDataReconstitutedEvent();
        
        Task CompleteReconstitutionAsync();
    }
}