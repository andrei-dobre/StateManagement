using DAA.StateManagement.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DAA.StateManagement
{
    [TestClass]
    public class UnitTest_StateManagementSystemBuildingDirector
    {
        private Mock<IStateManagementSystemBuildingOperations> BuildingInterfaceMock { get; set; }

        private IStateManagementSystemBuildingOperations BuildingInterface { get => BuildingInterfaceMock.Object; }
        private StateManagementSystemBuildingDirector TestInstance { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            BuildingInterfaceMock = new Mock<IStateManagementSystemBuildingOperations>();

            TestInstance = new StateManagementSystemBuildingDirector();
        }


        [TestMethod]
        public void Build__EventsAggregatorBuilt()
        {
            BuildingInterfaceMock.Setup(_ => _.BuildEventsAggregator()).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
        }

        [TestMethod]
        public void Build__TerminalDescriptorsFactoryBuilt()
        {
            BuildingInterfaceMock.Setup(_ => _.BuildTerminalDescriptorsFactory()).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
        }

        [TestMethod]
        public void Build__DataManipulatorBuilt()
        {
            BuildingInterfaceMock.Setup(_ => _.BuildDataManipulator()).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
        }

        [TestMethod]
        public void Build__InstancesBuilderBuiltAfterEventsAggregator()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            BuildingInterfaceMock.Setup(_ => _.BuildEventsAggregator()).Callback(() => ++callCounter);
            BuildingInterfaceMock.Setup(_ => _.BuildInstancesBuilder()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataRetrieverBuiltAfterTerminalDescriptorsFactory()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            BuildingInterfaceMock.Setup(_ => _.BuildTerminalDescriptorsFactory()).Callback(() => ++callCounter);
            BuildingInterfaceMock.Setup(_ => _.BuildDataRetriever()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataPoolBuiltAfterDataManipulator()
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
        public void Build__DataPoolBuiltAfterTerminalDescriptorsFactory()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            BuildingInterfaceMock.Setup(_ => _.BuildTerminalDescriptorsFactory()).Callback(() => ++callCounter);
            BuildingInterfaceMock.Setup(_ => _.BuildDataPool()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataRepositoryBuiltAfterInstancesBuilder()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            BuildingInterfaceMock.Setup(_ => _.BuildInstancesBuilder()).Callback(() => ++callCounter);
            BuildingInterfaceMock.Setup(_ => _.BuildDataRepository()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataRepositoryBuiltAfterDataPool()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            BuildingInterfaceMock.Setup(_ => _.BuildDataPool()).Callback(() => ++callCounter);
            BuildingInterfaceMock.Setup(_ => _.BuildDataRepository()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataRepositoryBuiltAfterDataRetriever()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            BuildingInterfaceMock.Setup(_ => _.BuildDataRetriever()).Callback(() => ++callCounter);
            BuildingInterfaceMock.Setup(_ => _.BuildDataRepository()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataRepositoryBuiltAfterCollectionsManager()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            BuildingInterfaceMock.Setup(_ => _.BuildDataCollectionsManager()).Callback(() => ++callCounter);
            BuildingInterfaceMock.Setup(_ => _.BuildDataRepository()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__CollectionsManagerBuiltAfterEventsAggregator()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            BuildingInterfaceMock.Setup(_ => _.BuildEventsAggregator()).Callback(() => ++callCounter);
            BuildingInterfaceMock.Setup(_ => _.BuildDataCollectionsManager()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__CollectionsManagerBuiltAfterDataPool()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            BuildingInterfaceMock.Setup(_ => _.BuildDataPool()).Callback(() => ++callCounter);
            BuildingInterfaceMock.Setup(_ => _.BuildDataCollectionsManager()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataRefresherBuiltAfterDataRetriever()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            BuildingInterfaceMock.Setup(_ => _.BuildDataRetriever()).Callback(() => ++callCounter);
            BuildingInterfaceMock.Setup(_ => _.BuildDataRefresher()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataRefresherBuiltAfterDataPool()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            BuildingInterfaceMock.Setup(_ => _.BuildDataPool()).Callback(() => ++callCounter);
            BuildingInterfaceMock.Setup(_ => _.BuildDataRefresher()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataRefresherBuiltAfterEventsAggregator()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            BuildingInterfaceMock.Setup(_ => _.BuildEventsAggregator()).Callback(() => ++callCounter);
            BuildingInterfaceMock.Setup(_ => _.BuildDataRefresher()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }

        [TestMethod]
        public void Build__DataQualitySupervisorBuiltAfterDataRefresher()
        {
            var callCounter = 0;
            var callOrderFollowed = false;

            BuildingInterfaceMock.Setup(_ => _.BuildDataRefresher()).Callback(() => ++callCounter);
            BuildingInterfaceMock.Setup(_ => _.BuildDataQualitySupervisor()).Callback(() => callOrderFollowed = callCounter > 0).Verifiable();

            TestInstance.Build(BuildingInterface);

            BuildingInterfaceMock.Verify();
            Assert.IsTrue(callOrderFollowed);
        }
    }
}
