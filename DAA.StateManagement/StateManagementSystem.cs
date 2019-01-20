using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class StateManagementSystem<TData> : IStateManagementSystem<TData>
        where TData : IData
    {
        public IDataQualitySupervisor<TData> QualitySupervisor { get; }
        public IDataRepository<TData> Repository { get; }
        public IStateManagementEventsAggregator<TData> EventsAggregator { get; }


        public StateManagementSystem(IDataQualitySupervisor<TData> qualitySupervisor, IDataRepository<TData> repository, IStateManagementEventsAggregator<TData> eventsAggregator)
        {
            QualitySupervisor = qualitySupervisor;
            Repository = repository;
            EventsAggregator = eventsAggregator;
        }
    }
}
