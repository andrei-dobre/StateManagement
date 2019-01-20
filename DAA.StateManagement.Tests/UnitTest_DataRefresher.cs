using System.Collections.Generic;
using System.Threading.Tasks;
using DAA.Helpers;
using DAA.StateManagement.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace DAA.StateManagement
{
    [TestClass]
    public class UnitTest_DataRefresher
    {
        private IDataPool<IData> DataPool => DataPoolMock.Object;
        private Mock<IDataPool<IData>> DataPoolMock { get; set; }

        private IDataRetriever<IData> DataRetriever => DataRetrieverMock.Object;
        private Mock<IDataRetriever<IData>> DataRetrieverMock { get; set; }

        private IData Data => DataMock.Object;
        private Mock<IData> DataMock { get; set; }

        private IDescriptor Descriptor => DescriptorMock.Object;
        private Mock<IDescriptor> DescriptorMock { get; set; }

        private ITerminalDescriptor TerminalDescriptor => TerminalDescriptorMock.Object;
        private Mock<ITerminalDescriptor> TerminalDescriptorMock { get; set; }

        private INonTerminalDescriptor NonTerminalDescriptor => NonTerminalDescriptorMock.Object;
        private Mock<INonTerminalDescriptor> NonTerminalDescriptorMock { get; set; }

        private IEnumerable<IDescriptor> DescriptorsCollection => DescriptorsCollectionMock.Object;
        private Mock<IEnumerable<IDescriptor>> DescriptorsCollectionMock { get; set; }

        private IEnumerable<ITerminalDescriptor> TerminalDescriptorsCollection => TerminalDescriptorsCollectionMock.Object;
        private Mock<IEnumerable<ITerminalDescriptor>> TerminalDescriptorsCollectionMock { get; set; }

        private IStateEventsAggregator StateEventsAggregator => StateEventsAggregatorMock.Object;
        private Mock<IStateEventsAggregator> StateEventsAggregatorMock { get; set; }

        private DataRefresher<IData> TestInstance => TestInstanceMock.Object;
        private Mock<DataRefresher<IData>> TestInstanceMock { get; set; }
        private IProtectedMock<DataRefresher<IData>> TestInstanceMockProtected => TestInstanceMock.Protected();


        [TestInitialize]
        public void BeforeEach()
        {
            DataPoolMock = new Mock<IDataPool<IData>>();
            DataRetrieverMock = new Mock<IDataRetriever<IData>>();
            DataMock = new Mock<IData>();
            DescriptorMock = new Mock<IDescriptor>();
            TerminalDescriptorMock = new Mock<ITerminalDescriptor>();
            NonTerminalDescriptorMock = new Mock<INonTerminalDescriptor>();
            DescriptorsCollectionMock = new Mock<IEnumerable<IDescriptor>>();
            TerminalDescriptorsCollectionMock = new Mock<IEnumerable<ITerminalDescriptor>>();
            StateEventsAggregatorMock = new Mock<IStateEventsAggregator>();

            TestInstanceMock = new Mock<DataRefresher<IData>>(DataRetriever, DataPool, StateEventsAggregator);
            TestInstanceMock.CallBase = true;
        }


        [TestMethod]
        public void GetDataPool__ProvidedValue()
        {
            var testInstance = new DataRefresher<IData>(DataRetriever, DataPool, StateEventsAggregator);

            var result = ReflectionHelper.Invoke(testInstance, "DataPool");

            Assert.AreSame(result, DataPool);
        }

        [TestMethod]
        public void GetDataRetriever__ProvidedValue()
        {
            var testInstance = new DataRefresher<IData>(DataRetriever, DataPool, StateEventsAggregator);

            var result = ReflectionHelper.Invoke(testInstance, "DataRetriever");

            Assert.AreSame(result, DataRetriever);
        }

        [TestMethod]
        public void GetStateEventsAggregator__ProvidedValue()
        {
            var testInstance = new DataRefresher<IData>(DataRetriever, DataPool, StateEventsAggregator);

            var result = ReflectionHelper.Invoke(testInstance, "StateEventsAggregator");

            Assert.AreSame(result, StateEventsAggregator);
        }


        [TestMethod]
        public async Task RefreshAsync__RefreshesAllIntersectingData()
        {
            DataPoolMock
                .Setup(_ => _.FindIntersectingDescriptors(Descriptor))
                .Returns(DescriptorsCollection)
                .Verifiable();

            TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(DescriptorsCollection))
                .Returns(Task.Delay(0))
                .Verifiable();

            await TestInstance.RefreshAsync(Descriptor);

            DataPoolMock.Verify();
            TestInstanceMock.Verify();
        }

        [TestMethod]
        public async Task RefreshAsync__TheRefreshingOfTheDataIsAwaited()
        {
            var taskToRefreshTheData = Task.Delay(10);

            DataPoolMock
                .Setup(_ => _.FindIntersectingDescriptors(Descriptor));

            TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(It.IsAny<IEnumerable<IDescriptor>>()))
                .Returns(taskToRefreshTheData);

            await TestInstance.RefreshAsync(Descriptor);

            Assert.IsTrue(taskToRefreshTheData.IsCompleted);
        }


        [TestMethod]
        public async Task RefreshDataAsync_CollectionOfDescriptors_DataOfAllDescriptorsRefreshed()
        {
            var descriptors = ArraysHelper.CreateWithContent(new Mock<IDescriptor>().Object, new Mock<IDescriptor>().Object);

            descriptors.ForEach(descriptor =>
            {
                TestInstanceMock
                    .Setup(_ => _.RefreshDataAsync(descriptor))
                    .Returns(Task.Delay(0))
                    .Verifiable();
            });

            await TestInstance.RefreshDataAsync(descriptors);

            TestInstanceMock.Verify();
        }

        [TestMethod]
        public async Task RefreshDataAsync_CollectionOfDescriptors_AwaitsTheRefreshOfAllData()
        {
            var descriptors = ArraysHelper.CreateWithContent(new Mock<IDescriptor>().Object, new Mock<IDescriptor>().Object, new Mock<IDescriptor>().Object);
            var tasks = new Task[descriptors.Length];

            for (var i = 0; i < descriptors.Length; ++i)
            {
                var task = Task.Delay(5 * (i + 1));
                var descriptor = descriptors[i];

                TestInstanceMock
                    .Setup(_ => _.RefreshDataAsync(descriptor))
                    .Returns(task);

                tasks[i] = task;
            }

            await TestInstance.RefreshDataAsync(descriptors);

            tasks.ForEach(_ => Assert.IsTrue(_.IsCompleted));
        }


        [TestMethod]
        public async Task RefreshDataAsync_TerminalDescriptorThroughGeneralHandler_DataRefreshedForTerminalDescriptor()
        {
            var descriptor = TerminalDescriptor as IDescriptor;

            TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.Delay(0));
            TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(TerminalDescriptor))
                .Returns(Task.Delay(0))
                .Verifiable();

            await TestInstance.RefreshDataAsync(descriptor);

            TestInstanceMock.Verify();
        }

        [TestMethod]
        public async Task RefreshDataAsync_TerminalDescriptorThroughGeneralHandler_TerminalDescriptorDataRefreshAwaited()
        {
            var taskToRefreshTheData = Task.Delay(10);
            var descriptor = TerminalDescriptor as IDescriptor;

            TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.Delay(0));
            TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(TerminalDescriptor))
                .Returns(taskToRefreshTheData);

            await TestInstance.RefreshDataAsync(descriptor);

            Assert.IsTrue(taskToRefreshTheData.IsCompleted);
        }

        [TestMethod]
        public async Task RefreshDataAsync_NonTerminalDescriptorThroughGeneralHandler_DataRefreshedForNonTerminalDescriptor()
        {
            var descriptor = NonTerminalDescriptor as IDescriptor;

            TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.Delay(0));
            TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(NonTerminalDescriptor))
                .Returns(Task.Delay(0))
                .Verifiable();

            await TestInstance.RefreshDataAsync(descriptor);

            TestInstanceMock.Verify();
        }

        [TestMethod]
        public async Task RefreshDataAsync_NonTerminalDescriptorThroughGeneralHandler_NonTerminalDescriptorDataRefreshAwaited()
        {
            var taskToRefreshTheData = Task.Delay(10);
            var descriptor = NonTerminalDescriptor as IDescriptor;

            TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.Delay(0));
            TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(NonTerminalDescriptor))
                .Returns(taskToRefreshTheData);

            await TestInstance.RefreshDataAsync(descriptor);

            Assert.IsTrue(taskToRefreshTheData.IsCompleted);
        }

        [TestMethod]
        public async Task RefreshDataAsync_NonTerminalDescriptorThroughGeneralHandler_TerminalDescriptorDataRefreshNotAttempted()
        {
            var descriptor = NonTerminalDescriptor as IDescriptor;

            TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.Delay(0));
            TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.Delay(0));

            await TestInstance.RefreshDataAsync(descriptor);

            TestInstanceMock.Verify(_ => _.RefreshDataAsync(It.IsAny<ITerminalDescriptor>()), Times.Never());
        }

        [TestMethod]
        public async Task RefreshDataAsync_TerminalDescriptorThroughGeneralHandler_NonTerminalDescriptorDataRefreshNotAttempted()
        {
            var descriptor = TerminalDescriptor as IDescriptor;

            TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.Delay(0));
            TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.Delay(0));

            await TestInstance.RefreshDataAsync(descriptor);

            TestInstanceMock.Verify(_ => _.RefreshDataAsync(It.IsAny<INonTerminalDescriptor>()), Times.Never());
        }


        [TestMethod]
        public async Task RefreshDataAsync_TerminalDescriptor_DataRetrievedAndSaved()
        {
            StateEventsAggregatorMock
                .Setup(_ => _.PublishDataChangedEvent(It.IsAny<IDescriptor>()));

            DataRetrieverMock
                .Setup(_ => _.RetrieveAsync(TerminalDescriptor))
                .Returns(Task.FromResult(Data))
                .Verifiable();

            DataPoolMock
                .Setup(_ => _.Save(TerminalDescriptor, Data))
                .Verifiable();

            await TestInstance.RefreshDataAsync(TerminalDescriptor);

            DataRetrieverMock.Verify();
            DataPoolMock.Verify();
        }

        [TestMethod]
        public async Task RefreshDataAsync_TerminalDescriptor_DataChangedEventPublishedAfterSave()
        {
            var callCounter = 0;
            var saveDataCallNumber = 0;
            var publishEventCallNumber = 0;

            DataRetrieverMock
                .Setup(_ => _.RetrieveAsync(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.FromResult(Data));

            DataPoolMock
                .Setup(_ => _.Save(It.IsAny<ITerminalDescriptor>(), It.IsAny<IData>()))
                .Callback(() => saveDataCallNumber = ++callCounter);

            StateEventsAggregatorMock
                .Setup(_ => _.PublishDataChangedEvent(TerminalDescriptor))
                .Callback(() => publishEventCallNumber = ++callCounter)
                .Verifiable();

            await TestInstance.RefreshDataAsync(TerminalDescriptor);

            StateEventsAggregatorMock.Verify();
            Assert.IsTrue(publishEventCallNumber >= saveDataCallNumber);
        }


        [TestMethod]
        public async Task RefreshDataAsync_NonTerminalDescriptor_CompositionRetrievedAndUpdated()
        {
            DataRetrieverMock
                .Setup(_ => _.RetrieveCompositionAsync(NonTerminalDescriptor))
                .Returns(Task.FromResult(TerminalDescriptorsCollection))
                .Verifiable();

            TestInstanceMockProtected
                .Setup<Task>("UpdateCompositionAndAcquireAdditionsAsync", NonTerminalDescriptor, TerminalDescriptorsCollection)
                .Returns(Task.Delay(0))
                .Verifiable();

            await TestInstance.RefreshDataAsync(NonTerminalDescriptor);

            DataRetrieverMock.Verify();
            TestInstanceMock.Verify();
        }

        [TestMethod]
        public async Task RefreshDataAsync_NonTerminalDescriptor_UpdateOfTheCompositionAwaited()
        {
            var task = Task.Delay(10);

            DataRetrieverMock
                .Setup(_ => _.RetrieveCompositionAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult(TerminalDescriptorsCollection));

            TestInstanceMockProtected
                .Setup<Task>("UpdateCompositionAndAcquireAdditionsAsync", ItExpr.IsAny<INonTerminalDescriptor>(), ItExpr.IsAny<IEnumerable<ITerminalDescriptor>>())
                .Returns(task);

            await TestInstance.RefreshDataAsync(NonTerminalDescriptor);

            Assert.IsTrue(task.IsCompleted);
        }

        [TestMethod]
        public async Task RefreshDataAsync_NonTerminalDescriptor_DataChangedEventPublishedAfterCompositionWasUpdated()
        {
            var callCounter = 0;
            var compositionUpdateCallNumber = 0;
            var publishEventCallNumber = 0;

            DataRetrieverMock
                .Setup(_ => _.RetrieveCompositionAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult(TerminalDescriptorsCollection));

            TestInstanceMockProtected
                .Setup<Task>("UpdateCompositionAndAcquireAdditionsAsync", ItExpr.IsAny<INonTerminalDescriptor>(), ItExpr.IsAny<IEnumerable<ITerminalDescriptor>>())
                .Callback(() => compositionUpdateCallNumber = ++callCounter)
                .Returns(Task.Delay(0));
            
            StateEventsAggregatorMock
                .Setup(_ => _.PublishDataChangedEvent(NonTerminalDescriptor))
                .Callback(() => publishEventCallNumber = ++callCounter)
                .Verifiable();

            await TestInstance.RefreshDataAsync(NonTerminalDescriptor);

            StateEventsAggregatorMock.Verify();
            Assert.IsTrue(publishEventCallNumber >= compositionUpdateCallNumber);
        }

        [TestMethod]
        public async Task RefreshDataAsync_NonTerminalDescriptor_CompositionChangedEventPublishedAfterCompositionWasUpdated()
        {
            var callCounter = 0;
            var compositionUpdateCallNumber = 0;
            var publishEventCallNumber = 0;

            DataRetrieverMock
                .Setup(_ => _.RetrieveCompositionAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult(TerminalDescriptorsCollection));

            TestInstanceMockProtected
                .Setup<Task>("UpdateCompositionAndAcquireAdditionsAsync", ItExpr.IsAny<INonTerminalDescriptor>(), ItExpr.IsAny<IEnumerable<ITerminalDescriptor>>())
                .Callback(() => compositionUpdateCallNumber = ++callCounter)
                .Returns(Task.Delay(0));

            StateEventsAggregatorMock
                .Setup(_ => _.PublishCompositionChangedEvent(NonTerminalDescriptor))
                .Callback(() => publishEventCallNumber = ++callCounter)
                .Verifiable();

            await TestInstance.RefreshDataAsync(NonTerminalDescriptor);

            StateEventsAggregatorMock.Verify();
            Assert.IsTrue(publishEventCallNumber >= compositionUpdateCallNumber);
        }


        [TestMethod]
        public async Task UpdateCompositionAndAcquireAdditionsAsync__CompositionUpdatedAndAdditionsRetrieved()
        {
            var additions = new Mock<IEnumerable<ITerminalDescriptor>>().Object;
            var dataCollection = new Mock<IEnumerable<IData>>().Object;

            DataPoolMock
                .Setup(_ => _.UpdateCompositionAndProvideAdditions(NonTerminalDescriptor, TerminalDescriptorsCollection))
                .Returns(additions)
                .Verifiable();

            DataRetrieverMock
                .Setup(_ => _.RetrieveAsync(additions))
                .Returns(Task.FromResult(dataCollection))
                .Verifiable();

            await (ReflectionHelper.Invoke(TestInstance, "UpdateCompositionAndAcquireAdditionsAsync", NonTerminalDescriptor, TerminalDescriptorsCollection) as Task);

            DataPoolMock.Verify();
            DataRetrieverMock.Verify();
        }

        [TestMethod]
        public async Task UpdateCompositionAndAcquireAdditionsAsync__AdditionsSaved()
        {
            var dataCollection = new Mock<IEnumerable<IData>>().Object;

            DataPoolMock
                .Setup(_ => _.UpdateCompositionAndProvideAdditions(It.IsAny<INonTerminalDescriptor>(), It.IsAny<IEnumerable<ITerminalDescriptor>>()))
                .Returns(new Mock<IEnumerable<ITerminalDescriptor>>().Object);

            DataRetrieverMock
                .Setup(_ => _.RetrieveAsync(It.IsAny<IEnumerable<ITerminalDescriptor>>()))
                .Returns(Task.FromResult(dataCollection));

            DataPoolMock
                .Setup(_ => _.Save(dataCollection))
                .Verifiable();

            await (ReflectionHelper.Invoke(TestInstance, "UpdateCompositionAndAcquireAdditionsAsync", NonTerminalDescriptor, TerminalDescriptorsCollection) as Task);

            DataPoolMock.Verify();
        }
    }
}
