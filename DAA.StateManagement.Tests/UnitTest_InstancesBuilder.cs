using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DAA.StateManagement
{
    [TestClass]
    public class UnitTest_InstancesBuilder
    {
        private IStateManagementEventsAggregator<IData> EventsAggregator => MockEventsAggregator.Object;
        private Mock<IStateManagementEventsAggregator<IData>> MockEventsAggregator { get; set; }

        private InstancesBuilder<IData> TestInstance => MockedTestInstance.Object;
        private Mock<InstancesBuilder<IData>> MockedTestInstance { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            MockEventsAggregator = new Mock<IStateManagementEventsAggregator<IData>>();

            MockedTestInstance = new Mock<InstancesBuilder<IData>>(EventsAggregator);
            MockedTestInstance.CallBase = true;
        }


        [TestMethod]
        public async Task BuildInstanceAsync_BuildersEnqueuedForInstance_AllBuildersWorked()
        {
            var descriptor = CreateDescriptor();
            var mockedBuilderOne = new Mock<IDataBuilder<IData>>();
            var builderOne = mockedBuilderOne.Object;
            var mockedBuilderTwo = new Mock<IDataBuilder<IData>>();
            var builderTwo = mockedBuilderTwo.Object;
            var instance = new Mock<IData>().Object;

            TestInstance.EnqueueBuilderForInstance(descriptor, builderOne);
            TestInstance.EnqueueBuilderForInstance(descriptor, builderTwo);

            await TestInstance.BuildInstanceAsync(descriptor, instance);

            mockedBuilderOne.Verify(_ => _.DoWorkAsync(instance));
            mockedBuilderTwo.Verify(_ => _.DoWorkAsync(instance));
        }

        [TestMethod]
        public async Task BuildInstanceAsync_BuildersEnqueuedForDifferentInstance_BuildersDidNotWork()
        {
            var descriptorOne = CreateDescriptor();
            var descriptorTwo = CreateDescriptor();
            var mockedBuilderOne = new Mock<IDataBuilder<IData>>();
            var builderOne = mockedBuilderOne.Object;
            var mockedBuilderTwo = new Mock<IDataBuilder<IData>>();
            var builderTwo = mockedBuilderTwo.Object;
            var instance = new Mock<IData>().Object;

            TestInstance.EnqueueBuilderForInstance(descriptorOne, builderOne);
            TestInstance.EnqueueBuilderForInstance(descriptorOne, builderTwo);

            await TestInstance.BuildInstanceAsync(descriptorTwo, instance);

            mockedBuilderOne.Verify(_ => _.DoWorkAsync(instance), Times.Never);
            mockedBuilderTwo.Verify(_ => _.DoWorkAsync(instance), Times.Never);
        }

        [TestMethod]
        public async Task BuildInstanceAsync_BuildersEnqueuedForCorrectAndDifferentInstance_CorrectBuildersWorked()
        {
            var descriptorOne = CreateDescriptor();
            var descriptorTwo = CreateDescriptor();
            var mockedBuilderOne = new Mock<IDataBuilder<IData>>();
            var builderOne = mockedBuilderOne.Object;
            var mockedBuilderTwo = new Mock<IDataBuilder<IData>>();
            var builderTwo = mockedBuilderTwo.Object;
            var instance = new Mock<IData>().Object;

            TestInstance.EnqueueBuilderForInstance(descriptorOne, builderOne);
            TestInstance.EnqueueBuilderForInstance(descriptorTwo, builderTwo);

            await TestInstance.BuildInstanceAsync(descriptorTwo, instance);

            mockedBuilderOne.Verify(_ => _.DoWorkAsync(instance), Times.Never);
            mockedBuilderTwo.Verify(_ => _.DoWorkAsync(instance));
        }

        [TestMethod]
        public async Task BuildInstanceAsync_NoBuildersEnqueued_NoError()
        {
            var caught = false;
            var descriptor = CreateDescriptor();
            var instance = new Mock<IData>().Object;

            try
            {
                await TestInstance.BuildInstanceAsync(descriptor, instance);
            }
            catch
            {
                caught = true;
            }

            Assert.IsFalse(caught);
        }

        [TestMethod]
        public void WhenInstanceChanged__InstanceBuilt()
        {
            var terminalDescriptor = new Mock<ITerminalDescriptor>().Object;
            var data = new Mock<IData>().Object;
            var args = new InstanceChangedEventArgs<IData>(terminalDescriptor, data);

            MockedTestInstance.Setup(_ => _.BuildInstanceAsync(It.IsAny<ITerminalDescriptor>(), It.IsAny<IData>()))
                .Returns(Task.FromResult(0));

            TestInstance.WhenInstanceChanged(new object(), args);

            MockedTestInstance.Verify(_ => _.BuildInstanceAsync(terminalDescriptor, data));
        }

        private ITerminalDescriptor CreateDescriptor()
        {
            return new Mock<ITerminalDescriptor>().Object;
        }
    }
}
