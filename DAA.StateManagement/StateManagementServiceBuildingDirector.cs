using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class StateManagementServiceBuildingDirector
    {
        public void Build(IStateManagementServiceBuildingOperations builder)
        {
            builder.BuildEventsAggregator();
            builder.BuildDataQualitySupervisor();
            builder.BuildDataRetriever();
            builder.BuildDataManipulator();
            builder.BuildDataPool();
            builder.BuildDataCollectionsManager();
        }
    }
}
