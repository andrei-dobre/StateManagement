namespace DAA.StateManagement.Interfaces
{
    public interface IStateManagementServiceBuildingOperations
    {
        void BuildDataCollectionsManager();

        void BuildDataManipulator();

        void BuildDataPool();

        void BuildDataQualitySupervisor();

        void BuildDataRetriever();

        void BuildEventsAggregator();

        void BuildTerminalDescriptorsFactory();
    }
}
