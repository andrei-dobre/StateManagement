namespace DAA.StateManagement
{
    public class StateManagementServiceBuildingDirector
    {
        public void Build(IStateManagementServiceBuilder builder)
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
