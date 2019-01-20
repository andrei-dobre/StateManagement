namespace DAA.StateManagement.Interfaces
{
    public interface IStateManagementSystemBuildingOperations
    {
        void BuildDataCollectionsManager();

        void BuildDataManipulator();

        void BuildDataPool();

        void BuildDataQualitySupervisor();

        void BuildDataRetriever();

        void BuildEventsAggregator();

        void BuildTerminalDescriptorsFactory();

        void BuildDataRefresher();

        void BuildDataRepository();
    }
}
