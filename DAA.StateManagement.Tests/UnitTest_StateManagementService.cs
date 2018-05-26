using System.Collections.Generic;
using System.Linq;
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

            this.TestInstanceMock = new Mock<StateManagementService<IData>>(this.DataRetriever, this.DataPool);
            this.TestInstanceMock.CallBase = true;
        }


        [TestMethod]
        public void GetDataPool__ProvidedValue()
        {
            var testInstance = new StateManagementService<IData>(this.DataRetriever, this.DataPool);

            var result = ReflectionHelper.Invoke(testInstance, "DataPool");

            Assert.AreSame(result, this.DataPool);
        }

        [TestMethod]
        public void GetDataRetriever__ProvidedValue()
        {
            var testInstance = new StateManagementService<IData>(this.DataRetriever, this.DataPool);

            var result = ReflectionHelper.Invoke(testInstance, "DataRetriever");

            Assert.AreSame(result, this.DataRetriever);
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
    }
}
