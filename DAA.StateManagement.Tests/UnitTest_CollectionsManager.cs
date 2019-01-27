using System.Collections.Generic;
using System.Threading.Tasks;
using DAA.StateManagement.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DAA.StateManagement
{
    [TestClass]
    public class UnitTest_CollectionsManager
    {
        private IStateManagementEventsAggregator<IData> EventsAggregator => MockedEventsAggregator.Object;
        private Mock<IStateManagementEventsAggregator<IData>> MockedEventsAggregator { get; set; }

        private IDataPool<IData> DataPool => MockedDataPool.Object;
        private Mock<IDataPool<IData>> MockedDataPool { get; set; }

        private INonTerminalDescriptor Descriptor => MockedDescriptor.Object;
        private Mock<INonTerminalDescriptor> MockedDescriptor { get; set; }

        private ICollection<IData> Collection => MockedCollection.Object;
        private Mock<ICollection<IData>> MockedCollection { get; set; }

        private CollectionsManager<IData> TestInstance => MockedTestInstance.Object;
        private Mock<CollectionsManager<IData>> MockedTestInstance { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            MockedEventsAggregator = new Mock<IStateManagementEventsAggregator<IData>>();
            MockedDataPool = new Mock<IDataPool<IData>>();

            MockedDescriptor = new Mock<INonTerminalDescriptor>();
            MockedCollection = new Mock<ICollection<IData>>();

            MockedTestInstance = new Mock<CollectionsManager<IData>>(DataPool, EventsAggregator);
            MockedTestInstance.CallBase = true;
        }


        [TestMethod]
        public async Task FillCollectionAsync__CollectionRegistered()
        {
            MockedTestInstance.Setup(_ => _.RegisterCollection(It.IsAny<ICollection<IData>>(),
                It.IsAny<INonTerminalDescriptor>()));
            MockedTestInstance.Setup(_ => _.FillCollectionWithData(It.IsAny<ICollection<IData>>(),
                It.IsAny<INonTerminalDescriptor>()));

            await TestInstance.FillCollectionAsync(Collection, Descriptor);

            MockedTestInstance.Verify(_ => _.RegisterCollection(Collection, Descriptor));
        }

        [TestMethod]
        public async Task FillCollectionAsync__CollectionFilledWithData()
        {
            MockedTestInstance.Setup(_ => _.RegisterCollection(It.IsAny<ICollection<IData>>(),
                It.IsAny<INonTerminalDescriptor>()));
            MockedTestInstance.Setup(_ => _.FillCollectionWithData(It.IsAny<ICollection<IData>>(),
                It.IsAny<INonTerminalDescriptor>()));

            await TestInstance.FillCollectionAsync(Collection, Descriptor);

            MockedTestInstance.Verify(_ => _.FillCollectionWithData(Collection, Descriptor));
        }

        [TestMethod]
        public async Task FillCollectionAsync__CollectionFilledWithDataAfterRegistration()
        {
            var callCounter = 0;
            var registrationCallNumber = 0;
            var fillCollectionCallNumber = 0;

            MockedTestInstance.Setup(_ => _.RegisterCollection(It.IsAny<ICollection<IData>>(),
                It.IsAny<INonTerminalDescriptor>())).Callback(() => registrationCallNumber = ++callCounter);
            MockedTestInstance.Setup(_ => _.FillCollectionWithData(It.IsAny<ICollection<IData>>(),
                It.IsAny<INonTerminalDescriptor>())).Callback(() => fillCollectionCallNumber = ++callCounter);

            await TestInstance.FillCollectionAsync(Collection, Descriptor);

            Assert.IsTrue(fillCollectionCallNumber > registrationCallNumber);
        }

        [TestMethod]
        public void DropCollection_CollectionRegistered_CollectionCleared()
        {
            MockedTestInstance.Setup(_ => _.IsCollectionRegistered(It.IsAny<ICollection<IData>>())).Returns(true);
            MockedTestInstance.Setup(_ => _.ClearCollection(It.IsAny<ICollection<IData>>()));
            MockedTestInstance.Setup(_ =>
                _.DropCollection(It.IsAny<ICollection<IData>>(), It.IsAny<INonTerminalDescriptor>()));
            MockedTestInstance.Setup(_ => _.GetDescriptor(It.IsAny<ICollection<IData>>()));

            TestInstance.DropCollection(Collection);

            MockedTestInstance.Verify(_ => _.ClearCollection(Collection));
        }

        [TestMethod]
        public void DropCollection_CollectionRegistered_CollectionDroppedCorrectly()
        {
            MockedTestInstance.Setup(_ => _.IsCollectionRegistered(It.IsAny<ICollection<IData>>())).Returns(true);
            MockedTestInstance.Setup(_ => _.ClearCollection(It.IsAny<ICollection<IData>>()));
            MockedTestInstance.Setup(_ =>
                _.DropCollection(It.IsAny<ICollection<IData>>(), It.IsAny<INonTerminalDescriptor>()));
            MockedTestInstance.Setup(_ => _.GetDescriptor(It.IsAny<ICollection<IData>>())).Returns(Descriptor);

            TestInstance.DropCollection(Collection);

            MockedTestInstance.Verify(_ => _.GetDescriptor(Collection));
            MockedTestInstance.Verify(_ => _.DropCollection(Collection, Descriptor));
        }

        [TestMethod]
        public void DropCollection_CollectionNotRegistered_CollectionNotCleared()
        {
            MockedTestInstance.Setup(_ => _.IsCollectionRegistered(It.IsAny<ICollection<IData>>())).Returns(false);
            MockedTestInstance.Setup(_ => _.ClearCollection(It.IsAny<ICollection<IData>>()));
            MockedTestInstance.Setup(_ =>
                _.DropCollection(It.IsAny<ICollection<IData>>(), It.IsAny<INonTerminalDescriptor>()));
            MockedTestInstance.Setup(_ => _.GetDescriptor(It.IsAny<ICollection<IData>>()));

            TestInstance.DropCollection(Collection);

            MockedTestInstance.Verify(_ => _.ClearCollection(It.IsAny<ICollection<IData>>()), Times.Never);
        }

        [TestMethod]
        public void DropCollection_CollectionNotRegistered_DoesNotAttemptToDropCollection()
        {
            MockedTestInstance.Setup(_ => _.IsCollectionRegistered(It.IsAny<ICollection<IData>>())).Returns(false);
            MockedTestInstance.Setup(_ => _.ClearCollection(It.IsAny<ICollection<IData>>()));
            MockedTestInstance.Setup(_ =>
                _.DropCollection(It.IsAny<ICollection<IData>>(), It.IsAny<INonTerminalDescriptor>()));
            MockedTestInstance.Setup(_ => _.GetDescriptor(It.IsAny<ICollection<IData>>()));

            TestInstance.DropCollection(Collection);

            MockedTestInstance.Verify(_ => _.GetDescriptor(It.IsAny<ICollection<IData>>()), Times.Never);
            MockedTestInstance.Verify(
                _ => _.DropCollection(It.IsAny<ICollection<IData>>(), It.IsAny<INonTerminalDescriptor>()), Times.Never);
        }

        [TestMethod]
        public void WhenCompositionChanged__AllAffectedCollectionsUpdated()
        {
            var collectionOne = new Mock<ICollection<IData>>().Object;
            var collectionTwo = new Mock<ICollection<IData>>().Object;
            var collections = new[] {collectionOne, collectionTwo};

            MockedTestInstance.Setup(_ => _.FindAffectedCollections(It.IsAny<INonTerminalDescriptor>()))
                .Returns(collections);
            MockedTestInstance.Setup(_ => _.UpdateCollection(It.IsAny<ICollection<IData>>()));

            TestInstance.WhenCompositionChanged(new object(), Descriptor);

            MockedTestInstance.Verify(_ => _.FindAffectedCollections(Descriptor));
            
            foreach (var collection in collections)
            {
                MockedTestInstance.Verify(_ => _.UpdateCollection(collection));
            }
        }

        [TestMethod]
        public void IsCollectionRegistered_RegisteredCollection_True()
        {
            TestInstance.RegisterCollection(Collection, Descriptor);

            Assert.IsTrue(TestInstance.IsCollectionRegistered(Collection));
        }

        [TestMethod]
        public void IsCollectionRegistered_CollectionRegisteredAndDropped_False()
        {
            TestInstance.RegisterCollection(Collection, Descriptor);
            TestInstance.DropCollection(Collection);

            Assert.IsFalse(TestInstance.IsCollectionRegistered(Collection));
        }

        [TestMethod]
        public void IsCollectionRegistered_CollectionNeverRegistered_False()
        {
            Assert.IsFalse(TestInstance.IsCollectionRegistered(Collection));
        }

        [TestMethod]
        public void IsCollectionRegisteredWithDescriptor_CollectionRegisteredWithDescriptor_True()
        {
            TestInstance.RegisterCollection(Collection, Descriptor);

            Assert.IsTrue(TestInstance.IsCollectionRegisteredWithDescriptor(Collection, Descriptor));
        }

        [TestMethod]
        public void IsCollectionRegisteredWithDescriptor_CollectionRegisteredWithOther_False()
        {
            var otherDescriptor = new Mock<INonTerminalDescriptor>().Object;

            TestInstance.RegisterCollection(Collection, Descriptor);

            Assert.IsFalse(TestInstance.IsCollectionRegisteredWithDescriptor(Collection, otherDescriptor));
        }

        [TestMethod]
        public void IsCollectionRegisteredWithDescriptor_CollectionNotRegistered_False()
        {
            Assert.IsFalse(TestInstance.IsCollectionRegisteredWithDescriptor(Collection, Descriptor));
        }

        [TestMethod]
        public void IsCollectionRegisteredWithDescriptor_CollectionReRegisteredWithOtherDescriptor_TrueForOtherDescriptor()
        {
            var otherDescriptor = new Mock<INonTerminalDescriptor>().Object;

            TestInstance.RegisterCollection(Collection, Descriptor);
            TestInstance.RegisterCollection(Collection, otherDescriptor);

            Assert.IsFalse(TestInstance.IsCollectionRegisteredWithDescriptor(Collection, Descriptor));
            Assert.IsTrue(TestInstance.IsCollectionRegisteredWithDescriptor(Collection, otherDescriptor));
        }

        [TestMethod]
        public void GetDescriptor_RegisteredCollection_CorrectDescriptor()
        {
            TestInstance.RegisterCollection(Collection, Descriptor);

            Assert.AreSame(Descriptor, TestInstance.GetDescriptor(Collection));
        }

        [TestMethod]
        public void GetDescriptor_CollectionRegisteredTwice_CorrectDescriptor()
        {
            var descriptorOne = new Mock<INonTerminalDescriptor>().Object;
            var descriptorTwo = new Mock<INonTerminalDescriptor>().Object;

            TestInstance.RegisterCollection(Collection, descriptorOne);
            TestInstance.RegisterCollection(Collection, descriptorTwo);

            Assert.AreSame(descriptorTwo, TestInstance.GetDescriptor(Collection));
        }
    }
}
