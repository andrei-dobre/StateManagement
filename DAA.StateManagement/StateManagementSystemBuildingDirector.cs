using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement
{
    public class StateManagementSystemBuildingDirector
    {
        public void Build(IStateManagementSystemBuildingOperations builder)
        {
            builder.BuildEventsAggregator();
            builder.BuildTerminalDescriptorsFactory();
            builder.BuildDataManipulator();

            builder.BuildDataRetriever();

            builder.BuildDataPool();
            builder.BuildDataRefresher();

            builder.BuildDataCollectionsManager();

            builder.BuildDataRepository();
            builder.BuildDataQualitySupervisor();
        }
    }
}
