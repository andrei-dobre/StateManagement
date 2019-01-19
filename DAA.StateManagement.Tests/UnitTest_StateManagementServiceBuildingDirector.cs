using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement.Tests
{
    [TestClass]
    public class UnitTest_StateManagementServiceBuildingDirector
    {
        private Mock<IStateManagementServiceBuildingOperations> BuildingInterfaceMock { get; set; }

        private IStateManagementServiceBuildingOperations BuildingInterface { get => BuildingInterfaceMock.Object; }
        private StateManagementServiceBuildingDirector TestInstance { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            BuildingInterfaceMock = new Mock<IStateManagementServiceBuildingOperations>();

            TestInstance = new StateManagementServiceBuildingDirector();
        }
        

        [TestMethod]
        public void Build__EventsAggregatorBuildFirst()
        {
            BuildingInterfaceMock.Setup(_ => _.BuildEventsAggregator()).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
        }

        [TestMethod]
        public void Build__DataQualitySupervisorBuildAfterEventsAggregator()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            BuildingInterfaceMock.Setup(_ => _.BuildEventsAggregator()).Callback(() => ++callCounter);
            BuildingInterfaceMock.Setup(_ => _.BuildDataQualitySupervisor()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataRetrieverAfterDataQualitySupervisorBuild()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            BuildingInterfaceMock.Setup(_ => _.BuildDataQualitySupervisor()).Callback(() => ++callCounter);
            BuildingInterfaceMock.Setup(_ => _.BuildDataRetriever()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataManipulatorAfterDataRetriever()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            BuildingInterfaceMock.Setup(_ => _.BuildDataRetriever()).Callback(() => ++callCounter);
            BuildingInterfaceMock.Setup(_ => _.BuildDataManipulator()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataPoolAfterDataManipulator()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            BuildingInterfaceMock.Setup(_ => _.BuildDataManipulator()).Callback(() => ++callCounter);
            BuildingInterfaceMock.Setup(_ => _.BuildDataPool()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataCollectionsManagerAfterDataPool()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            BuildingInterfaceMock.Setup(_ => _.BuildDataPool()).Callback(() => ++callCounter);
            BuildingInterfaceMock.Setup(_ => _.BuildDataCollectionsManager()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }
    }
}
