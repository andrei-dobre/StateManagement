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
        private IDataRetriever<IData> DataRetriever => MockedDataRetriever.Object;
        private Mock<IDataRetriever<IData>> MockedDataRetriever { get; set; }

        private IDataPool<IData> DataPool => MockedDataPool.Object;
        private Mock<IDataPool<IData>> MockedDataPool { get; set; }

        private ICollectionsManager<IData> CollectionsManager => MockedCollectionsManager.Object;
        private Mock<ICollectionsManager<IData>> MockedCollectionsManager { get; set; }

        private ICollection<IData> Collection => MockedCollection.Object;
        private Mock<ICollection<IData>> MockedCollection { get; set; }

        private INonTerminalDescriptor Descriptor => MockedDescriptor.Object;
        private Mock<INonTerminalDescriptor> MockedDescriptor { get; set; }

        private DataRepository<IData> TestInstance => MockedTestInstance.Object;
        private Mock<DataRepository<IData>> MockedTestInstance { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            MockedDataRetriever = new Mock<IDataRetriever<IData>>();
            MockedDataPool = new Mock<IDataPool<IData>>();
            MockedCollectionsManager = new Mock<ICollectionsManager<IData>>();

            MockedCollection = new Mock<ICollection<IData>>();
            MockedDescriptor = new Mock<INonTerminalDescriptor>();

            MockedTestInstance = new Mock<DataRepository<IData>>(DataRetriever, DataPool, CollectionsManager);
            MockedTestInstance.CallBase = true;
        }


        [TestMethod]
        public async Task FillCollectionAsync__MissingDataAcquired()
        {
            var awaited = false;

            MockedTestInstance.Setup(_ => _.AcquireMissingData(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.Delay(200).ContinueWith(_ => awaited = true));
            MockedCollectionsManager.Setup(_ =>
                    _.FillCollectionAsync(It.IsAny<ICollection<IData>>(), It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.Delay(0));

            await TestInstance.FillCollectionAsync(Collection, Descriptor);

            MockedTestInstance.Verify(_ => _.AcquireMissingData(Descriptor));
            Assert.IsTrue(awaited);
        }

        [TestMethod]
        public async Task FillCollectionAsync__CollectionFilled()
        {
            var awaited = false;

            MockedTestInstance.Setup(_ => _.AcquireMissingData(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.Delay(0));
            MockedCollectionsManager.Setup(_ =>
                    _.FillCollectionAsync(It.IsAny<ICollection<IData>>(), It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.Delay(200).ContinueWith(_ => awaited = true));

            await TestInstance.FillCollectionAsync(Collection, Descriptor);

            MockedCollectionsManager.Verify(_ => _.FillCollectionAsync(Collection, Descriptor));
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
            MockedCollectionsManager.Setup(_ =>
                    _.FillCollectionAsync(It.IsAny<ICollection<IData>>(), It.IsAny<INonTerminalDescriptor>()))
                .Callback(() => fillCollectionCallNumber = ++callCounter)
                .Returns(Task.Delay(10));

            await TestInstance.FillCollectionAsync(Collection, Descriptor);
            
            Assert.IsTrue(fillCollectionCallNumber > acquireDataCallNumber);
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
                .Returns(Task.FromResult<IEnumerable<IData>>(Collection));

            await TestInstance.AcquireMissingData(Descriptor);

            MockedDataRetriever.Verify(_ => _.RetrieveAsync(Descriptor));
        }

        [TestMethod]
        public async Task AcquireMissingData_DataNotContained_RetrievedDataSaved()
        {
            MockedDataPool.Setup(_ => _.Contains(It.IsAny<INonTerminalDescriptor>())).Returns(false);
            MockedDataRetriever.Setup(_ => _.RetrieveAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult<IEnumerable<IData>>(Collection));

            await TestInstance.AcquireMissingData(Descriptor);

            MockedDataPool.Verify(_ => _.Save(Descriptor, Collection));
        }

        [TestMethod]
        public async Task AcquireMissingData_DataContained_DataNotRetrieved()
        {
            MockedDataPool.Setup(_ => _.Contains(It.IsAny<INonTerminalDescriptor>())).Returns(true);
            MockedDataRetriever.Setup(_ => _.RetrieveAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult<IEnumerable<IData>>(Collection));

            await TestInstance.AcquireMissingData(Descriptor);

            MockedDataRetriever.Verify(_ => _.RetrieveAsync(It.IsAny<INonTerminalDescriptor>()), Times.Never);
        }

        [TestMethod]
        public async Task AcquireMissingData_DataContained_DataSaveNotAttempted()
        {
            MockedDataPool.Setup(_ => _.Contains(It.IsAny<INonTerminalDescriptor>())).Returns(true);
            MockedDataRetriever.Setup(_ => _.RetrieveAsync(It.IsAny<INonTerminalDescriptor>()))
                .Returns(Task.FromResult<IEnumerable<IData>>(Collection));

            await TestInstance.AcquireMissingData(Descriptor);

            MockedDataPool.Verify(_ => _.Save(It.IsAny<INonTerminalDescriptor>(), It.IsAny<ICollection<IData>>()),
                Times.Never);
        }
    }
}
