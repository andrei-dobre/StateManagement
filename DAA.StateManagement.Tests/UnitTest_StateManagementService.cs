using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Moq.Protected;

using DAA.Helpers;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement.Tests
{
    [TestClass]
    public class UnitTest_StateManagementService
    {
        private IDataPool<IData> DataPool { get => this.DataPoolMock.Object; }
        private Mock<IDataPool<IData>> DataPoolMock { get; set; }

        private IDataRetriever<IData> DataRetriever { get => this.DataRetrieverMock.Object; }
        private Mock<IDataRetriever<IData>> DataRetrieverMock { get; set; }

        private IData Data { get => this.DataMock.Object; }
        private Mock<IData> DataMock { get; set; }

        private IDescriptor Descriptor { get => this.DescriptorMock.Object; }
        private Mock<IDescriptor> DescriptorMock { get; set; }

        private ITerminalDescriptor TerminalDescriptor { get => this.TerminalDescriptorMock.Object; }
        private Mock<ITerminalDescriptor> TerminalDescriptorMock { get; set; }

        private INonTerminalDescriptor NonTerminalDescriptor { get => this.NonTerminalDescriptorMock.Object; }
        private Mock<INonTerminalDescriptor> NonTerminalDescriptorMock { get; set; }

        private IEnumerable<IDescriptor> DescriptorsCollection { get => this.DescriptorsCollectionMock.Object; }
        private Mock<IEnumerable<IDescriptor>> DescriptorsCollectionMock { get; set; }

        private IEnumerable<ITerminalDescriptor> TerminalDescriptorsCollection { get => this.TerminalDescriptorsCollectionMock.Object; }
        private Mock<IEnumerable<ITerminalDescriptor>> TerminalDescriptorsCollectionMock { get; set; }

        private IStateEventsAggregator StateEventsAggregator { get => this.StateEventsAggregatorMock.Object; }
        private Mock<IStateEventsAggregator> StateEventsAggregatorMock { get; set; }

        private StateManagementService<IData> TestInstance { get => this.TestInstanceMock.Object; }
        private Mock<StateManagementService<IData>> TestInstanceMock { get; set; }
        private IProtectedMock<StateManagementService<IData>> TestInstanceMockProtected { get => this.TestInstanceMock.Protected(); }


        [TestInitialize]
        public void BeforeEach()
        {
            this.DataPoolMock = new Mock<IDataPool<IData>>();
            this.DataRetrieverMock = new Mock<IDataRetriever<IData>>();
            this.DataMock = new Mock<IData>();
            this.DescriptorMock = new Mock<IDescriptor>();
            this.TerminalDescriptorMock = new Mock<ITerminalDescriptor>();
            this.NonTerminalDescriptorMock = new Mock<INonTerminalDescriptor>();
            this.DescriptorsCollectionMock = new Mock<IEnumerable<IDescriptor>>();
            this.TerminalDescriptorsCollectionMock = new Mock<IEnumerable<ITerminalDescriptor>>();
            this.StateEventsAggregatorMock = new Mock<IStateEventsAggregator>();

            this.TestInstanceMock = new Mock<StateManagementService<IData>>(this.DataRetriever, this.DataPool, this.StateEventsAggregator);
            this.TestInstanceMock.CallBase = true;
        }


        [TestMethod]
        public void GetDataPool__ProvidedValue()
        {
            var testInstance = new StateManagementService<IData>(this.DataRetriever, this.DataPool, this.StateEventsAggregator);

            var result = ReflectionHelper.Invoke(testInstance, "DataPool");

            Assert.AreSame(result, this.DataPool);
        }

        [TestMethod]
        public void GetDataRetriever__ProvidedValue()
        {
            var testInstance = new StateManagementService<IData>(this.DataRetriever, this.DataPool, this.StateEventsAggregator);

            var result = ReflectionHelper.Invoke(testInstance, "DataRetriever");

            Assert.AreSame(result, this.DataRetriever);
        }

        [TestMethod]
        public void GetStateEventsAggregator__ProvidedValue()
        {
            var testInstance = new StateManagementService<IData>(this.DataRetriever, this.DataPool, this.StateEventsAggregator);

            var result = ReflectionHelper.Invoke(testInstance, "StateEventsAggregator");

            Assert.AreSame(result, this.StateEventsAggregator);
        }


        [TestMethod]
        public async Task RefreshIntersectingDataAsync__RefreshesAllIntersectingData()
        {
            this.DataPoolMock
                .Setup(_ => _.FindIntersectingDescriptors(this.Descriptor))
                .Returns(this.DescriptorsCollection)
                .Verifiable();

            this.TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(this.DescriptorsCollection))
                .Returns(Task.FromResult(new VoidTaskResult()))
                .Verifiable();

            await this.TestInstance.RefreshIntersectingDataAsync(this.Descriptor);

            this.DataPoolMock.Verify();
            this.TestInstanceMock.Verify();
        }

        [TestMethod]
        public async Task RefreshIntersectingDataAsync__TheRefreshingOfTheDataIsAwaited()
        {
            var taskToRefreshTheData = Task.Delay(10);

            this.DataPoolMock
                .Setup(_ => _.FindIntersectingDescriptors(this.Descriptor));

            this.TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(It.IsAny<IEnumerable<IDescriptor>>()))
                .Returns(taskToRefreshTheData);

            await this.TestInstance.RefreshIntersectingDataAsync(this.Descriptor);

            Assert.IsTrue(taskToRefreshTheData.IsCompleted);
        }


        [TestMethod]
        public async Task RefreshDataAsync_CollectionOfDescriptors_DataOfAllDescriptorsRefreshed()
        {
            var descriptors = ArraysHelper.CreateWithContent(new Mock<IDescriptor>().Object, new Mock<IDescriptor>().Object);

            descriptors.ForEach(descriptor =>
            {
                this.TestInstanceMock
                    .Setup(_ => _.RefreshDataAsync(descriptor))
                    .Returns(Task.FromResult(new VoidTaskResult()))
                    .Verifiable();
            });

            await this.TestInstance.RefreshDataAsync(descriptors);

            this.TestInstanceMock.Verify();
        }

        [TestMethod]
        public async Task RefreshDataAsync_CollectionOfDescriptors_AwaitsTheRefreshOfAllData()
        {
            var descriptors = ArraysHelper.CreateWithContent(new Mock<IDescriptor>().Object, new Mock<IDescriptor>().Object, new Mock<IDescriptor>().Object);
            var tasks = new Task[descriptors.Length];

            for (int i = 0; i < descriptors.Length; ++i)
            {
                var task = Task.Delay(5 * (i + 1));
                var descriptor = descriptors[i];

                this.TestInstanceMock
                    .Setup(_ => _.RefreshDataAsync(descriptor))
                    .Returns(task);

                tasks[i] = task;
            }

            await this.TestInstance.RefreshDataAsync(descriptors);

            tasks.ForEach(_ => Assert.IsTrue(_.IsCompleted));
        }


        [TestMethod]
        public async Task RefreshDataAsync_TerminalDescriptorThroughGeneralHandler_DataRefreshedForTerminalDescriptor()
        {
            var descriptor = this.TerminalDescriptor as IDescriptor;

            this.TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult(new VoidTaskResult()));
            this.TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(this.TerminalDescriptor))
                .Returns(Task.FromResult(new VoidTaskResult()))
                .Verifiable();

            await this.TestInstance.RefreshDataAsync(descriptor);

            this.TestInstanceMock.Verify();
        }

        [TestMethod]
        public async Task RefreshDataAsync_TerminalDescriptorThroughGeneralHandler_TerminalDescriptorDataRefreshAwaited()
        {
            var taskToRefreshTheData = Task.Delay(10);
            var descriptor = this.TerminalDescriptor as IDescriptor;

            this.TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult(new VoidTaskResult()));
            this.TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(this.TerminalDescriptor))
                .Returns(taskToRefreshTheData);

            await this.TestInstance.RefreshDataAsync(descriptor);

            Assert.IsTrue(taskToRefreshTheData.IsCompleted);
        }

        [TestMethod]
        public async Task RefreshDataAsync_NonTerminalDescriptorThroughGeneralHandler_DataRefreshedForNonTerminalDescriptor()
        {
            var descriptor = this.NonTerminalDescriptor as IDescriptor;

            this.TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.FromResult(new VoidTaskResult()));
            this.TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(this.NonTerminalDescriptor))
                .Returns(Task.FromResult(new VoidTaskResult()))
                .Verifiable();

            await this.TestInstance.RefreshDataAsync(descriptor);

            this.TestInstanceMock.Verify();
        }

        [TestMethod]
        public async Task RefreshDataAsync_NonTerminalDescriptorThroughGeneralHandler_NonTerminalDescriptorDataRefreshAwaited()
        {
            var taskToRefreshTheData = Task.Delay(10);
            var descriptor = this.NonTerminalDescriptor as IDescriptor;

            this.TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.FromResult(new VoidTaskResult()));
            this.TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(this.NonTerminalDescriptor))
                .Returns(taskToRefreshTheData);

            await this.TestInstance.RefreshDataAsync(descriptor);

            Assert.IsTrue(taskToRefreshTheData.IsCompleted);
        }

        [TestMethod]
        public async Task RefreshDataAsync_NonTerminalDescriptorThroughGeneralHandler_TerminalDescriptorDataRefreshNotAttempted()
        {
            var descriptor = this.NonTerminalDescriptor as IDescriptor;

            this.TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.FromResult(new VoidTaskResult()));
            this.TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult(new VoidTaskResult()));

            await this.TestInstance.RefreshDataAsync(descriptor);

            this.TestInstanceMock.Verify(_ => _.RefreshDataAsync(It.IsAny<ITerminalDescriptor>()), Times.Never());
        }

        [TestMethod]
        public async Task RefreshDataAsync_TerminalDescriptorThroughGeneralHandler_NonTerminalDescriptorDataRefreshNotAttempted()
        {
            var descriptor = this.TerminalDescriptor as IDescriptor;

            this.TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.FromResult(new VoidTaskResult()));
            this.TestInstanceMock
                .Setup(_ => _.RefreshDataAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult(new VoidTaskResult()));

            await this.TestInstance.RefreshDataAsync(descriptor);

            this.TestInstanceMock.Verify(_ => _.RefreshDataAsync(It.IsAny<INonTerminalDescriptor>()), Times.Never());
        }


        [TestMethod]
        public async Task RefreshDataAsync_TerminalDescriptor_DataRetrievedAndSaved()
        {
            this.StateEventsAggregatorMock
                .Setup(_ => _.PublishDataChangedEvent(It.IsAny<IDescriptor>(), It.IsAny<object>()));

            this.DataRetrieverMock
                .Setup(_ => _.RetrieveAsync(this.TerminalDescriptor))
                .Returns(Task.FromResult(this.Data))
                .Verifiable();

            this.DataPoolMock
                .Setup(_ => _.Save(this.TerminalDescriptor, this.Data))
                .Verifiable();

            await this.TestInstance.RefreshDataAsync(this.TerminalDescriptor);

            this.DataRetrieverMock.Verify();
            this.DataPoolMock.Verify();
        }

        [TestMethod]
        public async Task RefreshDataAsync_TerminalDescriptor_DataChangedEventPublishedAfterSave()
        {
            var callCounter = 0;
            var saveDataCallNumber = 0;
            var publishEventCallNumber = 0;

            this.DataRetrieverMock
                .Setup(_ => _.RetrieveAsync(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.FromResult(this.Data));

            this.DataPoolMock
                .Setup(_ => _.Save(It.IsAny<ITerminalDescriptor>(), It.IsAny<IData>()))
                .Callback(() => saveDataCallNumber = ++callCounter);

            this.StateEventsAggregatorMock
                .Setup(_ => _.PublishDataChangedEvent(this.TerminalDescriptor, this.TestInstance))
                .Callback(() => publishEventCallNumber = ++callCounter)
                .Verifiable();

            await this.TestInstance.RefreshDataAsync(this.TerminalDescriptor);

            this.StateEventsAggregatorMock.Verify();
            Assert.IsTrue(publishEventCallNumber >= saveDataCallNumber);
        }


        [TestMethod]
        public async Task RefreshDataAsync_NonTerminalDescriptor_CompositionRetrievedAndUpdated()
        {
            this.DataRetrieverMock
                .Setup(_ => _.RetrieveCompositionAsync(this.NonTerminalDescriptor))
                .Returns(Task.FromResult(this.TerminalDescriptorsCollection))
                .Verifiable();

            this.TestInstanceMockProtected
                .Setup<Task>("UpdateCompositionAndAcquireAdditionsAsync", this.NonTerminalDescriptor, this.TerminalDescriptorsCollection)
                .Returns(Task.FromResult(new VoidTaskResult()))
                .Verifiable();

            await this.TestInstance.RefreshDataAsync(this.NonTerminalDescriptor);

            this.DataRetrieverMock.Verify();
            this.TestInstanceMock.Verify();
        }

        [TestMethod]
        public async Task RefreshDataAsync_NonTerminalDescriptor_UpdateOfTheCompositionAwaited()
        {
            var task = Task.Delay(10);

            this.DataRetrieverMock
                .Setup(_ => _.RetrieveCompositionAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult(this.TerminalDescriptorsCollection));

            this.TestInstanceMockProtected
                .Setup<Task>("UpdateCompositionAndAcquireAdditionsAsync", ItExpr.IsAny<INonTerminalDescriptor>(), ItExpr.IsAny<IEnumerable<ITerminalDescriptor>>())
                .Returns(task);

            await this.TestInstance.RefreshDataAsync(this.NonTerminalDescriptor);

            Assert.IsTrue(task.IsCompleted);
        }

        [TestMethod]
        public async Task RefreshDataAsync_NonTerminalDescriptor_DataChangedEventPublishedAfterCompositionWasUpdated()
        {
            var callCounter = 0;
            var compositionUpdateCallNumber = 0;
            var publishEventCallNumber = 0;

            this.DataRetrieverMock
                .Setup(_ => _.RetrieveCompositionAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult(this.TerminalDescriptorsCollection));

            this.TestInstanceMockProtected
                .Setup<Task>("UpdateCompositionAndAcquireAdditionsAsync", ItExpr.IsAny<INonTerminalDescriptor>(), ItExpr.IsAny<IEnumerable<ITerminalDescriptor>>())
                .Callback(() => compositionUpdateCallNumber = ++callCounter)
                .Returns(Task.FromResult(new VoidTaskResult()));
            
            this.StateEventsAggregatorMock
                .Setup(_ => _.PublishDataChangedEvent(this.NonTerminalDescriptor, this.TestInstance))
                .Callback(() => publishEventCallNumber = ++callCounter)
                .Verifiable();

            await this.TestInstance.RefreshDataAsync(this.NonTerminalDescriptor);

            this.StateEventsAggregatorMock.Verify();
            Assert.IsTrue(publishEventCallNumber >= compositionUpdateCallNumber);
        }


        [TestMethod]
        public async Task UpdateCompositionAndAcquireAdditionsAsync__CompositionUpdatedAndAdditionsRetrieved()
        {
            var additions = new Mock<IEnumerable<ITerminalDescriptor>>().Object;
            var dataCollection = new Mock<IEnumerable<IData>>().Object;

            this.DataPoolMock
                .Setup(_ => _.UpdateCompositionAndProvideAdditions(this.NonTerminalDescriptor, this.TerminalDescriptorsCollection))
                .Returns(additions)
                .Verifiable();

            this.DataRetrieverMock
                .Setup(_ => _.RetrieveAsync(additions))
                .Returns(Task.FromResult(dataCollection))
                .Verifiable();

            await (ReflectionHelper.Invoke(this.TestInstance, "UpdateCompositionAndAcquireAdditionsAsync", this.NonTerminalDescriptor, this.TerminalDescriptorsCollection) as Task);

            this.DataPoolMock.Verify();
            this.DataRetrieverMock.Verify();
        }

        [TestMethod]
        public async Task UpdateCompositionAndAcquireAdditionsAsync__AdditionsSaved()
        {
            var dataCollection = new Mock<IEnumerable<IData>>().Object;

            this.DataPoolMock
                .Setup(_ => _.UpdateCompositionAndProvideAdditions(It.IsAny<INonTerminalDescriptor>(), It.IsAny<IEnumerable<ITerminalDescriptor>>()))
                .Returns(new Mock<IEnumerable<ITerminalDescriptor>>().Object);

            this.DataRetrieverMock
                .Setup(_ => _.RetrieveAsync(It.IsAny<IEnumerable<ITerminalDescriptor>>()))
                .Returns(Task.FromResult(dataCollection));

            this.DataPoolMock
                .Setup(_ => _.Save(dataCollection))
                .Verifiable();

            await (ReflectionHelper.Invoke(this.TestInstance, "UpdateCompositionAndAcquireAdditionsAsync", this.NonTerminalDescriptor, this.TerminalDescriptorsCollection) as Task);

            this.DataPoolMock.Verify();
        }
    }
}
