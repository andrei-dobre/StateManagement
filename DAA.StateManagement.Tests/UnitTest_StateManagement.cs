using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAA.Helpers;
using DAA.StateManagement.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DAA.StateManagement
{
    [TestClass]
    public class UnitTest_StateManagement
    {
        private IData Data => MockedData.Object;
        private Mock<IData> MockedData { get; set; }

        private IDataRepository<IData> DataRepository => MockedDataRepository.Object;
        private Mock<IDataRepository<IData>> MockedDataRepository { get; set; }

        private IStateManagementSystemsCatalog StateManagementSystemsCatalog =>
            MockedStateManagementSystemsCatalog.Object;
        private Mock<IStateManagementSystemsCatalog> MockedStateManagementSystemsCatalog { get; set; }

        private IStateManagementSystem<IData> StateManagementSystem => MockedStateManagementSystem.Object;
        private Mock<IStateManagementSystem<IData>> MockedStateManagementSystem { get; set; }

        private StateManagementSystemBuildingDirector SystemBuildingDirector => MockedSystemBuildingDirector.Object;
        private Mock<StateManagementSystemBuildingDirector> MockedSystemBuildingDirector { get; set; }

        private IStateManagementSystemBuilder<IData> StateManagementSystemBuilder =>
            MockedStateManagementSystemBuilder.Object;
        private Mock<IStateManagementSystemBuilder<IData>> MockedStateManagementSystemBuilder { get; set; }

        private ICollection<IData> Collection => MockedCollection.Object;
        private Mock<ICollection<IData>> MockedCollection { get; set; }

        private INonTerminalDescriptor Descriptor => MockedDescriptor.Object;
        private Mock<INonTerminalDescriptor> MockedDescriptor { get; set; }

        private ITerminalDescriptor TerminalDescriptor => MockedTerminalDescriptor.Object;
        private Mock<ITerminalDescriptor> MockedTerminalDescriptor { get; set; }

        private IDataBuilder<IData> DataBuilder => MockedDataBuilder.Object;
        private Mock<IDataBuilder<IData>> MockedDataBuilder { get; set; }

        private StateManagement TestInstance => MockedTestInstance.Object;
        private Mock<StateManagement> MockedTestInstance { get; set; }


        [TestInitialize]
        public void BeforeEach()
        {
            MockedData = new Mock<IData>();
            MockedDescriptor = new Mock<INonTerminalDescriptor>();
            MockedStateManagementSystemBuilder = new Mock<IStateManagementSystemBuilder<IData>>();
            MockedStateManagementSystem = new Mock<IStateManagementSystem<IData>>();
            MockedDataRepository = new Mock<IDataRepository<IData>>();
            MockedSystemBuildingDirector = new Mock<StateManagementSystemBuildingDirector>();
            MockedStateManagementSystemsCatalog = new Mock<IStateManagementSystemsCatalog>();
            MockedCollection = new Mock<ICollection<IData>>();
            MockedDataBuilder = new Mock<IDataBuilder<IData>>();
            MockedTerminalDescriptor = new Mock<ITerminalDescriptor>();

            MockedStateManagementSystemsCatalog.Setup(_ => _.Retrieve<IData>()).Returns(StateManagementSystem);
            MockedStateManagementSystem.Setup(_ => _.Repository).Returns(DataRepository);

            MockedTestInstance = new Mock<StateManagement>();
            MockedTestInstance.CallBase = true;
            MockedTestInstance.Setup(_ => _.StateManagementSystemsCatalog).Returns(StateManagementSystemsCatalog);
            MockedTestInstance.Setup(_ => _.SystemBuildingDirector).Returns(SystemBuildingDirector);
        }


        [TestMethod]
        public void GetRepository__CorrectlyRetrieved()
        {
            Assert.AreSame(DataRepository, TestInstance.GetRepository<IData>());
        }

        [TestMethod]
        public void Register__SystemBuilt()
        {
            TestInstance.Register(StateManagementSystemBuilder);
            
            MockedSystemBuildingDirector.Verify(_ => _.Build(StateManagementSystemBuilder));
        }

        [TestMethod]
        public void Register__SystemRegistered()
        {
            MockedStateManagementSystemBuilder.Setup(_ => _.ExtractResult()).Returns(StateManagementSystem);

            TestInstance.Register(StateManagementSystemBuilder);

            MockedStateManagementSystemsCatalog.Verify(_ => _.Register(StateManagementSystem));
        }

        [TestMethod]
        public void Register__SystemExtractedAfterBuild()
        {
            var callCounter = 0;
            var buildCallNumber = 0;
            var extractCallNumber = 0;

            MockedSystemBuildingDirector.Setup(_ => _.Build(It.IsAny<IStateManagementSystemBuildingOperations>()))
                .Callback(() => buildCallNumber = ++callCounter);
            MockedStateManagementSystemBuilder.Setup(_ => _.ExtractResult())
                .Callback(() => extractCallNumber = ++callCounter);

            TestInstance.Register(StateManagementSystemBuilder);

            Assert.IsTrue(extractCallNumber > buildCallNumber);
        }

        [TestMethod]
        public void DropCollection__DelegatedToRepository()
        {
            MockedTestInstance.Setup(_ => _.GetRepository<IData>()).Returns(DataRepository);

            TestInstance.DropCollection(Collection);

            MockedDataRepository.Verify(_ => _.DropCollection(Collection));
        }

        [TestMethod]
        public void IsCollectionRegistered__DelegatedToRepository()
        {
            var expectedResult = RandomizationHelper.Instance.GetBool();

            MockedTestInstance.Setup(_ => _.GetRepository<IData>()).Returns(DataRepository);
            MockedDataRepository.Setup(_ => _.IsCollectionRegistered(It.IsAny<ICollection<IData>>()))
                .Returns(expectedResult);

            var result = TestInstance.IsCollectionRegistered(Collection);

            Assert.AreEqual(expectedResult, result);
            MockedDataRepository.Verify(_ => _.IsCollectionRegistered(Collection));
        }

        [TestMethod]
        public void IsCollectionRegisteredWithDescriptor__DelegatedToRepository()
        {
            var expectedResult = RandomizationHelper.Instance.GetBool();

            MockedTestInstance.Setup(_ => _.GetRepository<IData>()).Returns(DataRepository);
            MockedDataRepository.Setup(_ =>
                    _.IsCollectionRegisteredWithDescriptor(It.IsAny<ICollection<IData>>(),
                        It.IsAny<INonTerminalDescriptor>()))
                .Returns(expectedResult);

            var result = TestInstance.IsCollectionRegisteredWithDescriptor(Collection, Descriptor);

            Assert.AreEqual(expectedResult, result);
            MockedDataRepository.Verify(_ => _.IsCollectionRegisteredWithDescriptor(Collection, Descriptor));
        }

        [TestMethod]
        public async Task FillCollectionAsync__NoBuilder_NotAlreadyRegisteredWithDescriptor__DelegatedToRepository()
        {
            var awaited = false;

            MockedTestInstance.Setup(_ =>
                _.IsCollectionRegisteredWithDescriptor(It.IsAny<ICollection<IData>>(),
                    It.IsAny<INonTerminalDescriptor>())).Returns(false);
            MockedDataRepository.Setup(_ =>
                    _.FillCollectionAsync(It.IsAny<IFillCollectionArgs<IData>>()))
                .Returns(Task.Delay(10).ContinueWith(_ => awaited = true));

            await TestInstance.FillCollectionAsync(Collection, Descriptor);

            MockedDataRepository.Verify(_ =>
                _.FillCollectionAsync(It.Is<IFillCollectionArgs<IData>>(args =>
                    Equals(args.Collection, Collection) && args.Descriptor == Descriptor)));
            Assert.IsTrue(awaited);
            MockedTestInstance.Verify(_ => _.IsCollectionRegisteredWithDescriptor(Collection, Descriptor));
        }

        [TestMethod]
        public async Task FillCollectionAsync__NoBuilder_AlreadyRegisteredWithDescriptor_CollectionEmpty__DelegatedToRepository()
        {
            MockedCollection.Setup(_ => _.Count).Returns(0); 
            MockedTestInstance.Setup(_ =>
                _.IsCollectionRegisteredWithDescriptor(It.IsAny<ICollection<IData>>(),
                    It.IsAny<INonTerminalDescriptor>())).Returns(true);
            MockedDataRepository.Setup(_ => _.FillCollectionAsync(It.IsAny<IFillCollectionArgs<IData>>()))
                .Returns(Task.FromResult(0));

            await TestInstance.FillCollectionAsync(Collection, Descriptor);

            MockedDataRepository.Verify(_ => _.FillCollectionAsync(It.IsAny<IFillCollectionArgs<IData>>()));
        }

        [TestMethod]
        public async Task FillCollectionAsync__NoBuilder_AlreadyRegisteredWithDescriptor_CollectionNotEmpty__NotDelegatedToRepository()
        {
            MockedCollection.Setup(_ => _.Count).Returns(RandomizationHelper.Instance.GetInt());
            MockedTestInstance.Setup(_ =>
                _.IsCollectionRegisteredWithDescriptor(It.IsAny<ICollection<IData>>(),
                    It.IsAny<INonTerminalDescriptor>())).Returns(true);
            MockedDataRepository.Setup(_ => _.FillCollectionAsync(It.IsAny<IFillCollectionArgs<IData>>()))
                .Returns(Task.FromResult(0));

            await TestInstance.FillCollectionAsync(Collection, Descriptor);

            MockedDataRepository.Verify(_ => _.FillCollectionAsync(It.IsAny<IFillCollectionArgs<IData>>()), Times.Never);
        }

        [TestMethod]
        public async Task FillCollectionAsync__Builder_NotAlreadyRegisteredWithDescriptor__FillDelegatedToRepository()
        {
            var awaited = false;

            MockedTestInstance.Setup(_ =>
                _.IsCollectionRegisteredWithDescriptor(It.IsAny<ICollection<IData>>(),
                    It.IsAny<INonTerminalDescriptor>())).Returns(false);
            MockedDataRepository.Setup(_ =>
                    _.FillCollectionAsync(It.IsAny<IFillCollectionArgs<IData>>()))
                .Returns(Task.Delay(10).ContinueWith(_ => awaited = true));

            await TestInstance.FillCollectionAsync(Collection, Descriptor, DataBuilder);

            MockedDataRepository.Verify(_ =>
                _.FillCollectionAsync(It.Is<IFillCollectionArgs<IData>>(args =>
                    Equals(args.Collection, Collection) && args.Descriptor == Descriptor && args.Builder == DataBuilder)));
            Assert.IsTrue(awaited);
            MockedTestInstance.Verify(_ => _.IsCollectionRegisteredWithDescriptor(Collection, Descriptor));
        }

        [TestMethod]
        public async Task FillCollectionAsync__Builder_AlreadyRegisteredWithDescriptor_CollectionEmpty__FillDelegatedToRepository()
        {
            MockedCollection.Setup(_ => _.Count).Returns(0);
            MockedTestInstance.Setup(_ =>
                _.IsCollectionRegisteredWithDescriptor(It.IsAny<ICollection<IData>>(),
                    It.IsAny<INonTerminalDescriptor>())).Returns(true);
            MockedDataRepository.Setup(_ => _.FillCollectionAsync(It.IsAny<IFillCollectionArgs<IData>>()))
                .Returns(Task.FromResult(0));

            await TestInstance.FillCollectionAsync(Collection, Descriptor, DataBuilder);

            MockedDataRepository.Verify(_ =>
                _.FillCollectionAsync(It.Is<IFillCollectionArgs<IData>>(args =>
                    Equals(args.Collection, Collection) && args.Descriptor == Descriptor && args.Builder == DataBuilder)));
        }

        [TestMethod]
        public async Task FillCollectionAsync__Builder_AlreadyRegisteredWithDescriptor_CollectionNotEmpty__FillNotDelegatedToRepository()
        {
            MockedCollection.Setup(_ => _.Count).Returns(RandomizationHelper.Instance.GetInt());
            MockedTestInstance.Setup(_ =>
                _.IsCollectionRegisteredWithDescriptor(It.IsAny<ICollection<IData>>(),
                    It.IsAny<INonTerminalDescriptor>())).Returns(true);
            MockedDataRepository.Setup(_ => _.FillCollectionAsync(It.IsAny<IFillCollectionArgs<IData>>()))
                .Returns(Task.FromResult(0));

            await TestInstance.FillCollectionAsync(Collection, Descriptor, DataBuilder);

            MockedDataRepository.Verify(_ => _.FillCollectionAsync(It.IsAny<IFillCollectionArgs<IData>>()), Times.Never);
        }

        [TestMethod]
        public async Task FillCollectionAsync__Builder_AlreadyRegisteredWithDescriptor_CollectionNotEmpty__ChangeBuilderDelegatedToRepository()
        {
            MockedCollection.Setup(_ => _.Count).Returns(RandomizationHelper.Instance.GetInt());
            MockedTestInstance.Setup(_ =>
                _.IsCollectionRegisteredWithDescriptor(It.IsAny<ICollection<IData>>(),
                    It.IsAny<INonTerminalDescriptor>())).Returns(true);
            MockedDataRepository.Setup(_ => _.FillCollectionAsync(It.IsAny<IFillCollectionArgs<IData>>()))
                .Returns(Task.FromResult(0));

            await TestInstance.FillCollectionAsync(Collection, Descriptor, DataBuilder);

            MockedDataRepository.Verify(_ => _.ChangeBuilderAsync(Collection, DataBuilder), Times.Once);
        }

        [TestMethod]
        public async Task FillCollectionAsync__Builder_NotAlreadyRegisteredWithDescriptor__ChangeBuilderNotDelegatedToRepository()
        {
            MockedTestInstance.Setup(_ =>
                _.IsCollectionRegisteredWithDescriptor(It.IsAny<ICollection<IData>>(),
                    It.IsAny<INonTerminalDescriptor>())).Returns(false);
            MockedDataRepository.Setup(_ => _.FillCollectionAsync(It.IsAny<IFillCollectionArgs<IData>>()))
                .Returns(Task.FromResult(0));

            await TestInstance.FillCollectionAsync(Collection, Descriptor, DataBuilder);

            MockedDataRepository.Verify(
                _ => _.ChangeBuilderAsync(It.IsAny<ICollection<IData>>(), It.IsAny<IDataBuilder<IData>>()),
                Times.Never);
        }

        [TestMethod]
        public async Task ChangeBuilderAsync__DelegatedToRepository()
        {
            var awaited = false;

            MockedDataRepository.Setup(_ =>
                    _.ChangeBuilderAsync(It.IsAny<ICollection<IData>>(), It.IsAny<IDataBuilder<IData>>()))
                .Returns(Task.Delay(10).ContinueWith(_ => awaited = true));

            await TestInstance.ChangeBuilderAsync(Collection, DataBuilder);

            MockedDataRepository.Verify(_ => _.ChangeBuilderAsync(Collection, DataBuilder));
            Assert.IsTrue(awaited);
        }

        [TestMethod]
        public async Task RetrieveAsync__DelegatedToRepository()
        {
            MockedTestInstance.Setup(_ => _.GetRepository<IData>()).Returns(DataRepository);
            MockedDataRepository.Setup(_ => _.RetrieveAsync(It.IsAny<ITerminalDescriptor>()))
                .Returns(Task.FromResult(Data));

            var result = await TestInstance.RetrieveAsync<IData>(TerminalDescriptor);

            Assert.AreSame(Data, result);
            MockedDataRepository.Verify(_ => _.RetrieveAsync(TerminalDescriptor));
        }

        [TestMethod]
        public async Task RetrieveAsync_BuilderSpecified_DelegatedToRepository()
        {
            MockedTestInstance.Setup(_ => _.GetRepository<IData>()).Returns(DataRepository);
            MockedDataRepository.Setup(_ =>
                    _.RetrieveAsync(It.IsAny<ITerminalDescriptor>(), It.IsAny<IDataBuilder<IData>>()))
                .Returns(Task.FromResult(Data));

            var result = await TestInstance.RetrieveAsync<IData>(TerminalDescriptor, DataBuilder);

            Assert.AreSame(Data, result);
            MockedDataRepository.Verify(_ => _.RetrieveAsync(TerminalDescriptor, DataBuilder));
        }
    }
}
