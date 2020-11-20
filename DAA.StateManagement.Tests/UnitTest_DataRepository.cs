using System.Collections.Generic;
using System.Threading.Tasks;
using DAA.Helpers;
using DAA.StateManagement.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DAA.StateManagement
{
    [TestClass]
    public class UnitTest_DataRepository
    {
        private IData Data => MockedData.Object;
        private Mock<IData> MockedData { get; set; }

        private IInstanceRetrievalContext<IData> InstanceRetrievalContext => MockedInstanceRetrievalContext.Object;
        private Mock<IInstanceRetrievalContext<IData>> MockedInstanceRetrievalContext { get; set; }

        private IDataRetriever<IData> DataRetriever => MockedDataRetriever.Object;
        private Mock<IDataRetriever<IData>> MockedDataRetriever { get; set; }

        private IDataPool<IData> DataPool => MockedDataPool.Object;
        private Mock<IDataPool<IData>> MockedDataPool { get; set; }

        private ICollectionsManager<IData> CollectionsManager => MockedCollectionsManager.Object;
        private Mock<ICollectionsManager<IData>> MockedCollectionsManager { get; set; }

        private ICollection<IData> Collection => MockedCollection.Object;
        private Mock<ICollection<IData>> MockedCollection { get; set; }

        private ICollectionRetrievalContext<IData> CollectionRetrievalContext => MockedCollectionRetrievalContext.Object;
        private Mock<ICollectionRetrievalContext<IData>> MockedCollectionRetrievalContext { get; set; }

        private INonTerminalDescriptor Descriptor => MockedDescriptor.Object;
        private Mock<INonTerminalDescriptor> MockedDescriptor { get; set; }

        private ITerminalDescriptor TerminalDescriptor => MockedTerminalDescriptor.Object;
        private Mock<ITerminalDescriptor> MockedTerminalDescriptor { get; set; }

        private IFillCollectionArgs<IData> FillCollectionArgs => MockedFillCollectionArgs.Object;
        private Mock<IFillCollectionArgs<IData>> MockedFillCollectionArgs { get; set; }

        private IDataBuilder<IData> DataBuilder => MockedDataBuilder.Object;
        private Mock<IDataBuilder<IData>> MockedDataBuilder { get; set; }

        private IInstancesBuilder<IData> InstancesBuilder => MockedInstancesBuilder.Object;
        private Mock<IInstancesBuilder<IData>> MockedInstancesBuilder { get; set; }

        private DataRepository<IData> TestInstance => MockedTestInstance.Object;
        private Mock<DataRepository<IData>> MockedTestInstance { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            MockedDataRetriever = new Mock<IDataRetriever<IData>>();
            MockedDataPool = new Mock<IDataPool<IData>>();
            MockedCollectionsManager = new Mock<ICollectionsManager<IData>>();
            MockedInstancesBuilder = new Mock<IInstancesBuilder<IData>>();

            MockedData = new Mock<IData>();
            MockedCollection = new Mock<ICollection<IData>>();
            MockedDescriptor = new Mock<INonTerminalDescriptor>();
            MockedDataBuilder = new Mock<IDataBuilder<IData>>();
            MockedTerminalDescriptor = new Mock<ITerminalDescriptor>();

            MockedFillCollectionArgs = new Mock<IFillCollectionArgs<IData>>();

            MockedFillCollectionArgs.Setup(_ => _.Descriptor).Returns(Descriptor);
            MockedFillCollectionArgs.Setup(_ => _.Collection).Returns(Collection);

            MockedInstanceRetrievalContext = new Mock<IInstanceRetrievalContext<IData>>();
            MockedInstanceRetrievalContext
                .Setup(x => x.Data)
                .Returns(Data);

            MockedCollectionRetrievalContext = new Mock<ICollectionRetrievalContext<IData>>();
            MockedCollectionRetrievalContext
                .Setup(x => x.Data)
                .Returns(Collection);

            MockedTestInstance = new Mock<DataRepository<IData>>(DataRetriever, DataPool, CollectionsManager, InstancesBuilder);
            MockedTestInstance.CallBase = true;
        }

        [TestMethod]
        public async Task RetrieveAsync_TerminalDescriptor_MissingDataAcquired()
        {
            var awaited = false;

            MockedTestInstance.Setup(_ => _.AcquireMissingData(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.Delay(200).ContinueWith(_ => awaited = true));

            await TestInstance.RetrieveAsync(TerminalDescriptor);

            MockedTestInstance.Verify(_ => _.AcquireMissingData(TerminalDescriptor));
            Assert.IsTrue(awaited);
        }

        [TestMethod]
        public async Task RetrieveAsync_TerminalDescriptor_DataRetrievedFromPool()
        {
            MockedTestInstance.Setup(_ => _.AcquireMissingData(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.FromResult(0));
            MockedDataPool.Setup(_ => _.Retrieve(It.IsAny<ITerminalDescriptor>()))
                .Returns(Data);

            var result = await TestInstance.RetrieveAsync(TerminalDescriptor);

            Assert.AreSame(result, Data);
            MockedDataPool.Verify(_ => _.Retrieve(TerminalDescriptor));
        }

        [TestMethod]
        public async Task RetrieveAsync_TerminalDescriptor_DataRetrievedAfterBeingAcquired()
        {
            var callCounter = 0;
            var acquireDataCall = 0;
            var retrieveDataCall = 0;

            MockedTestInstance.Setup(_ => _.AcquireMissingData(It.IsAny<ITerminalDescriptor>()))
                .Callback(() => acquireDataCall = ++callCounter)
                .Returns(Task.FromResult(0));
            MockedDataPool.Setup(_ => _.Retrieve(It.IsAny<ITerminalDescriptor>()))
                .Callback(() => retrieveDataCall = ++callCounter);

            await TestInstance.RetrieveAsync(TerminalDescriptor);

            Assert.IsTrue(retrieveDataCall > acquireDataCall);
        }

        [TestMethod]
        public async Task RetrieveAsync_TerminalDescriptor_BuilderSpecified_BuilderEnqueued()
        {
            MockedTestInstance.Setup(_ => _.RetrieveAsync(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.FromResult(Data));

            await TestInstance.RetrieveAsync(TerminalDescriptor, DataBuilder);

            MockedInstancesBuilder.Verify(_ => _.EnqueueBuilderForInstance(TerminalDescriptor, DataBuilder));
        }

        [TestMethod]
        public async Task RetrieveAsync_TerminalDescriptor_BuilderSpecified_InstanceBuilt()
        {
            var awaited = false;

            MockedTestInstance.Setup(_ => _.RetrieveAsync(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.FromResult(Data));
            MockedDataBuilder.Setup(_ => _.DoWorkAsync(It.IsAny<IData>()))
                .Returns(Task.Delay(300).ContinueWith(_ => awaited = true));

            await TestInstance.RetrieveAsync(TerminalDescriptor, DataBuilder);

            MockedDataBuilder.Verify(_ => _.DoWorkAsync(Data));
            Assert.IsTrue(awaited);
        }

        [TestMethod]
        public async Task RetrieveAsync_TerminalDescriptor_BuilderSpecified_DataRetrievedAndProvided()
        {
            MockedTestInstance.Setup(_ => _.RetrieveAsync(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.FromResult(Data));

            var result = await TestInstance.RetrieveAsync(TerminalDescriptor, DataBuilder);

            Assert.AreSame(result, Data);
            MockedTestInstance.Verify(_ => _.RetrieveAsync(TerminalDescriptor));
        }
        
        [TestMethod]
        public async Task RetrieveAsync_NonTerminalDescriptor_MissingDataAcquired()
        {
            var awaited = false;

            MockedTestInstance.Setup(_ => _.AcquireMissingData(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.Delay(200).ContinueWith(_ => awaited = true));

            await TestInstance.RetrieveAsync(Descriptor);

            MockedTestInstance.Verify(_ => _.AcquireMissingData(Descriptor));
            Assert.IsTrue(awaited);
        }

        [TestMethod]
        public async Task RetrieveAsync_NonTerminalDescriptor_DataRetrievedFromPool()
        {
            var data = new IData[0];
            
            MockedTestInstance.Setup(_ => _.AcquireMissingData(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult(0));
            MockedDataPool.Setup(_ => _.Retrieve(It.IsAny<INonTerminalDescriptor>()))
                .Returns(data);

            var result = await TestInstance.RetrieveAsync(Descriptor);

            Assert.AreSame(result, data);
            MockedDataPool.Verify(_ => _.Retrieve(Descriptor));
        }

        [TestMethod]
        public async Task RetrieveAsync_NonTerminalDescriptor_DataRetrievedAfterBeingAcquired()
        {
            var callCounter = 0;
            var acquireDataCall = 0;
            var retrieveDataCall = 0;

            MockedTestInstance.Setup(_ => _.AcquireMissingData(It.IsAny<INonTerminalDescriptor>()))
                .Callback(() => acquireDataCall = ++callCounter)
                .Returns(Task.CompletedTask);
            MockedDataPool.Setup(_ => _.Retrieve(It.IsAny<INonTerminalDescriptor>()))
                .Callback(() => retrieveDataCall = ++callCounter);

            await TestInstance.RetrieveAsync(Descriptor);

            Assert.IsTrue(retrieveDataCall > acquireDataCall);
        }

        [TestMethod]
        public async Task RetrieveAsync_NonTerminalDescriptor_BuilderSpecified_InstancesBuilt()
        {
            var data = new[]{Data};
            var awaited = false;

            MockedTestInstance.Setup(_ => _.RetrieveAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult((IEnumerable<IData>)data));
            MockedDataBuilder.Setup(_ => _.DoWorkAsync(It.IsAny<IData>()))
                .Returns(Task.Delay(300).ContinueWith(_ => awaited = true));

            await TestInstance.RetrieveAsync(Descriptor, DataBuilder);

            MockedDataBuilder.Verify(_ => _.DoWorkAsync(Data));
            Assert.IsTrue(awaited);
        }

        [TestMethod]
        public async Task RetrieveAsync_NonTerminalDescriptor_BuilderSpecified_DataRetrievedAndProvided()
        {
            var data = new[]{Data};

            MockedTestInstance.Setup(_ => _.RetrieveAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult((IEnumerable<IData>) data));

            var result = await TestInstance.RetrieveAsync(Descriptor, DataBuilder);

            Assert.AreSame(result, data);
            MockedTestInstance.Verify(_ => _.RetrieveAsync(Descriptor));
        }

        [TestMethod]
        public async Task FillCollectionAsync__MissingDataAcquired()
        {
            var awaited = false;

            MockedTestInstance.Setup(_ => _.AcquireMissingData(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.Delay(200).ContinueWith(_ => awaited = true));
            MockedCollectionsManager.Setup(_ => _.FillCollectionAsync(It.IsAny<IFillCollectionArgs<IData>>()))
                .Returns(Task.Delay(0));

            await TestInstance.FillCollectionAsync(FillCollectionArgs);

            MockedTestInstance.Verify(_ => _.AcquireMissingData(Descriptor));
            Assert.IsTrue(awaited);
        }

        [TestMethod]
        public async Task FillCollectionAsync__CollectionFilled()
        {
            var awaited = false;

            MockedTestInstance.Setup(_ => _.AcquireMissingData(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.Delay(0));
            MockedCollectionsManager.Setup(_ => _.FillCollectionAsync(It.IsAny<IFillCollectionArgs<IData>>()))
                .Returns(Task.Delay(200).ContinueWith(_ => awaited = true));

            await TestInstance.FillCollectionAsync(FillCollectionArgs);

            MockedCollectionsManager.Verify(_ => _.FillCollectionAsync(FillCollectionArgs));
            Assert.IsTrue(awaited);
        }

        [TestMethod]
        public async Task FillCollectionAsync__CollectionFilledAfterDataAcquired()
        {
            var callCounter = 0;
            var acquireDataCallNumber = 0;
            var fillCollectionCallNumber = 0;

            MockedTestInstance.Setup(_ => _.AcquireMissingData(It.IsAny<INonTerminalDescriptor>()))
                .Callback(() => acquireDataCallNumber = ++callCounter)
                .Returns(Task.Delay(10));
            MockedCollectionsManager.Setup(_ => _.FillCollectionAsync(It.IsAny<IFillCollectionArgs<IData>>()))
                .Callback(() => fillCollectionCallNumber = ++callCounter)
                .Returns(Task.Delay(10));

            await TestInstance.FillCollectionAsync(FillCollectionArgs);
            
            Assert.IsTrue(fillCollectionCallNumber > acquireDataCallNumber);
        }

        [TestMethod]
        public async Task ChangeBuilderAsync__DelegatesToCollectionManager()
        {
            var awaited = false;

            MockedCollectionsManager.Setup(_ => _.ChangeBuilderAsync(It.IsAny<ICollection<IData>>(), It.IsAny<IDataBuilder<IData>>()))
                .Returns(Task.Delay(200).ContinueWith(_ => awaited = true));

            await TestInstance.ChangeBuilderAsync(Collection, DataBuilder);

            MockedCollectionsManager.Verify(_ => _.ChangeBuilderAsync(Collection, DataBuilder));
            Assert.IsTrue(awaited);
        }

        [TestMethod]
        public void IsCollectionRegistered__DelegatesToCollectionsManager()
        {
            var expectedResult = RandomizationHelper.Instance.GetBool();

            MockedCollectionsManager.Setup(_ => _.IsCollectionRegistered(It.IsAny<ICollection<IData>>()))
                .Returns(expectedResult);

            var result = TestInstance.IsCollectionRegistered(Collection);

            MockedCollectionsManager.Verify(_ => _.IsCollectionRegistered(Collection));
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void IsCollectionRegisteredWithDescriptor__DelegatedToCollectionsManager()
        {
            var expectedResult = RandomizationHelper.Instance.GetBool();

            MockedCollectionsManager.Setup(_ =>
                    _.IsCollectionRegisteredWithDescriptor(It.IsAny<ICollection<IData>>(),
                        It.IsAny<INonTerminalDescriptor>()))
                .Returns(expectedResult);

            var result = TestInstance.IsCollectionRegisteredWithDescriptor(Collection, Descriptor);

            Assert.AreEqual(expectedResult, result);
            MockedCollectionsManager.Verify(_ => _.IsCollectionRegisteredWithDescriptor(Collection, Descriptor));
        }

        [TestMethod]
        public void DropCollection__DelegatesToCollectionsManager()
        {
            TestInstance.DropCollection(Collection);

            MockedCollectionsManager.Verify(_ => _.DropCollection(Collection));
        }

        [TestMethod]
        public async Task AcquireMissingData_DataNotContained_DataCorrectlyRetrieved()
        {
            MockedDataPool.Setup(_ => _.Contains(It.IsAny<INonTerminalDescriptor>())).Returns(false);
            MockedDataRetriever.Setup(_ => _.RetrieveAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult(CollectionRetrievalContext));

            await TestInstance.AcquireMissingData(Descriptor);

            MockedDataRetriever.Verify(_ => _.RetrieveAsync(Descriptor));
        }

        [TestMethod]
        public async Task AcquireMissingData_DataNotContained_RetrievedDataSaved()
        {
            MockedDataPool.Setup(_ => _.Contains(It.IsAny<INonTerminalDescriptor>())).Returns(false);
            MockedDataRetriever.Setup(_ => _.RetrieveAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult(CollectionRetrievalContext));

            await TestInstance.AcquireMissingData(Descriptor);

            MockedDataPool.Verify(_ => _.SaveAsync(Descriptor, CollectionRetrievalContext));
        }

        [TestMethod]
        public async Task AcquireMissingData_DataContained_DataNotRetrieved()
        {
            MockedDataPool.Setup(_ => _.Contains(It.IsAny<INonTerminalDescriptor>())).Returns(true);
            MockedDataRetriever.Setup(_ => _.RetrieveAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult(CollectionRetrievalContext));

            await TestInstance.AcquireMissingData(Descriptor);

            MockedDataRetriever.Verify(_ => _.RetrieveAsync(It.IsAny<INonTerminalDescriptor>()), Times.Never);
        }

        [TestMethod]
        public async Task AcquireMissingData_DataContained_DataSaveNotAttempted()
        {
            MockedDataPool.Setup(_ => _.Contains(It.IsAny<INonTerminalDescriptor>())).Returns(true);
            MockedDataRetriever.Setup(_ => _.RetrieveAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult(CollectionRetrievalContext));

            await TestInstance.AcquireMissingData(Descriptor);

            MockedDataPool.Verify(_ => _.SaveAsync(It.IsAny<INonTerminalDescriptor>(), It.IsAny<ICollectionRetrievalContext<IData>>()),
                Times.Never);
        }





        [TestMethod]
        public async Task AcquireMissingData__TerminalDescriptor_DataNotContained__DataCorrectlyRetrieved()
        {
            MockedDataPool.Setup(_ => _.Contains(It.IsAny<ITerminalDescriptor>())).Returns(false);
            MockedDataRetriever.Setup(_ => _.RetrieveAsync(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.FromResult(InstanceRetrievalContext));

            await TestInstance.AcquireMissingData(TerminalDescriptor);

            MockedDataRetriever.Verify(_ => _.RetrieveAsync(TerminalDescriptor));
        }

        [TestMethod]
        public async Task AcquireMissingData__TerminalDescriptor_DataNotContained__RetrievedDataSaved()
        {
            MockedDataPool.Setup(_ => _.Contains(It.IsAny<ITerminalDescriptor>())).Returns(false);
            MockedDataRetriever.Setup(_ => _.RetrieveAsync(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.FromResult(InstanceRetrievalContext));

            await TestInstance.AcquireMissingData(TerminalDescriptor);

            MockedDataPool.Verify(_ => _.SaveAsync(TerminalDescriptor, InstanceRetrievalContext));
        }

        [TestMethod]
        public async Task AcquireMissingData__TerminalDescriptor_DataContained__DataNotRetrieved()
        {
            MockedDataPool.Setup(_ => _.Contains(It.IsAny<ITerminalDescriptor>())).Returns(true);
            MockedDataRetriever.Setup(_ => _.RetrieveAsync(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.FromResult(InstanceRetrievalContext));

            await TestInstance.AcquireMissingData(TerminalDescriptor);

            MockedDataRetriever.Verify(_ => _.RetrieveAsync(It.IsAny<ITerminalDescriptor>()), Times.Never);
        }

        [TestMethod]
        public async Task AcquireMissingData__TerminalDescriptor_DataContained__DataSaveNotAttempted()
        {
            MockedDataPool.Setup(_ => _.Contains(It.IsAny<ITerminalDescriptor>())).Returns(true);
            MockedDataRetriever.Setup(_ => _.RetrieveAsync(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.FromResult(InstanceRetrievalContext));

            await TestInstance.AcquireMissingData(TerminalDescriptor);

            MockedDataPool.Verify(_ => _.SaveAsync(It.IsAny<ITerminalDescriptor>(), It.IsAny<IInstanceRetrievalContext<IData>>()),
                Times.Never);
        }
    }
}
