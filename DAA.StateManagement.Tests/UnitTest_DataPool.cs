using System.Collections.Generic;
using System.Linq;
using DAA.Helpers;
using DAA.StateManagement.Interfaces;
using DAA.StateManagement.Stores;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;

namespace DAA.StateManagement
{
    [TestClass]
    public class UnitTest_DataPool
    {
        private ITerminalDescriptorsFactory<IData> TerminalDescriptorsFactory => TerminalDescriptorsFactoryMock.Object;
        private Mock<ITerminalDescriptorsFactory<IData>> TerminalDescriptorsFactoryMock { get; set; }

        private IDataManipulator<IData> DataManipulator => DataManipulatorMock.Object;
        private Mock<IDataManipulator<IData>> DataManipulatorMock { get; set; }

        private DataStore<IData> DataStore => DataStoreMock.Object;
        private Mock<DataStore<IData>> DataStoreMock { get; set; }

        private CompositionsStore Compositions => CompositionsMock.Object;
        private Mock<CompositionsStore> CompositionsMock { get; set; } 

        private IData Data => DataMock.Object;
        private Mock<IData> DataMock { get; set; }

        private IEnumerable<IData> DataCollection => DataCollectionMock.Object;
        private Mock<IEnumerable<IData>> DataCollectionMock { get; set; }

        private ITerminalDescriptor TerminalDescriptor => TerminalDescriptorMock.Object;
        private Mock<ITerminalDescriptor> TerminalDescriptorMock { get; set; }

        private INonTerminalDescriptor NonTerminalDescriptor => NonTerminalDescriptorMock.Object;
        private Mock<INonTerminalDescriptor> NonTerminalDescriptorMock { get; set; }

        private IEnumerable<ITerminalDescriptor> TerminalDescriptorsCollection => TerminalDescriptorsCollectionMock.Object;
        private Mock<IEnumerable<ITerminalDescriptor>> TerminalDescriptorsCollectionMock { get; set; }

        private DataPool<IData> TestInstance => TestInstanceMock.Object;
        private Mock<DataPool<IData>> TestInstanceMock { get; set; }
        private IProtectedMock<DataPool<IData>> TestInstanceMockProtected => TestInstanceMock.Protected();


        [TestInitialize]
        public void BeforeEach()
        {
            TerminalDescriptorsFactoryMock = new Mock<ITerminalDescriptorsFactory<IData>>();
            DataManipulatorMock = new Mock<IDataManipulator<IData>>();
            DataStoreMock = new Mock<DataStore<IData>>(DataManipulator);
            CompositionsMock = new Mock<CompositionsStore>();
            DataMock = new Mock<IData>();
            DataCollectionMock = new Mock<IEnumerable<IData>>();
            TerminalDescriptorMock = new Mock<ITerminalDescriptor>();
            NonTerminalDescriptorMock = new Mock<INonTerminalDescriptor>();
            TerminalDescriptorsCollectionMock = new Mock<IEnumerable<ITerminalDescriptor>>();

            TestInstanceMock = new Mock<DataPool<IData>>(TerminalDescriptorsFactory, DataManipulator);
            TestInstanceMock.CallBase = true;

            TestInstanceMockProtected
                .SetupGet<DataStore<IData>>("Data")
                .Returns(DataStore);
            TestInstanceMockProtected
                .SetupGet<CompositionsStore>("Compositions")
                .Returns(Compositions);
        }


        [TestMethod]
        public void GetTerminalDescriptorsFactory__ProvidedValue()
        {
            var testInstance = new DataPool<IData>(TerminalDescriptorsFactory, DataManipulator);

            var result = ReflectionHelper.Invoke(testInstance, "TerminalDescriptorsFactory");

            Assert.AreSame(TerminalDescriptorsFactory, result);
        }

        [TestMethod]
        public void GetData__NotNull_Constant()
        {
            var testInstance = new DataPool<IData>(TerminalDescriptorsFactory, DataManipulator);

            var resultOne = ReflectionHelper.Invoke(testInstance, "Data");
            var resultTwo = ReflectionHelper.Invoke(testInstance, "Data");

            Assert.IsNotNull(resultOne);
            Assert.IsNotNull(resultTwo);
            Assert.AreSame(resultOne, resultTwo);
        }

        [TestMethod]
        public void GetNonTerminalDescriptorCompositions__NotNull_Constant()
        {
            var testInstance = new DataPool<IData>(TerminalDescriptorsFactory, DataManipulator);

            var resultOne = ReflectionHelper.Invoke(testInstance, "Compositions");
            var resultTwo = ReflectionHelper.Invoke(testInstance, "Compositions");

            Assert.IsNotNull(resultOne);
            Assert.IsNotNull(resultTwo);
            Assert.AreSame(resultOne, resultTwo);
        }


        [TestMethod]
        public void Ctor__DataStoreCreatedWithProvidedDataManipulator()
        {
            var testInstance = new DataPool<IData>(TerminalDescriptorsFactory, DataManipulator);
            var dataStore = ReflectionHelper.Invoke(testInstance, "Data") as DataStore<IData>;

            var result = ReflectionHelper.Invoke(dataStore, "DataManipulator");

            Assert.AreSame(DataManipulator, result);
        }


        [TestMethod]
        public void Save_TerminalDescriptor_DataSaved()
        {
            TestInstance.Save(TerminalDescriptor, Data);

            DataStoreMock.Verify(_ => _.Save(TerminalDescriptor, Data));
        }


        [TestMethod]
        public void Save_NonTerminalDescriptor_DataSaved()
        {
            TestInstanceMockProtected.Setup("DescribeAndSave", ItExpr.IsAny<IEnumerable<IData>>());

            TestInstance.Save(NonTerminalDescriptor, DataCollection);

            TestInstanceMockProtected.Verify("DescribeAndSave", Times.Once(), DataCollection);
        }

        [TestMethod]
        public void Save_NonTerminalDescriptor_CompositionUpdated()
        {
            TestInstanceMockProtected
                .Setup<IEnumerable<ITerminalDescriptor>>("DescribeAndSave", ItExpr.IsAny<IEnumerable<IData>>())
                .Returns(TerminalDescriptorsCollection);

            TestInstance.Save(NonTerminalDescriptor, DataCollection);

            CompositionsMock.Verify(_ => _.Save(NonTerminalDescriptor, TerminalDescriptorsCollection), Times.Once());
        }


        [TestMethod]
        public void Save_DataCollection_DataDescribedSaved()
        {
            TestInstanceMockProtected.Setup("DescribeAndSave", ItExpr.IsAny<IEnumerable<IData>>());

            TestInstance.Save(DataCollection);

            TestInstanceMockProtected.Verify("DescribeAndSave", Times.Once(), DataCollection);
        }


        [TestMethod]
        public void Contains_NonTerminalContained_True()
        {
            CompositionsMock
                .Setup(_ => _.Contains(NonTerminalDescriptor))
                .Returns(true);

            var result = TestInstance.Contains(NonTerminalDescriptor);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Contains_NonTerminalNotContained_False()
        {
            CompositionsMock
                .Setup(_ => _.Contains(NonTerminalDescriptor))
                .Returns(false);

            var result = TestInstance.Contains(NonTerminalDescriptor);

            Assert.IsFalse(result);
        }


        [TestMethod]
        public void Contains_TerminalContained_True()
        {
            DataStoreMock
                .Setup(_ => _.Contains(TerminalDescriptor))
                .Returns(true);

            var result = TestInstance.Contains(TerminalDescriptor);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Contains_TerminalNotContained_False()
        {
            DataStoreMock
                .Setup(_ => _.Contains(TerminalDescriptor))
                .Returns(false);

            var result = TestInstance.Contains(TerminalDescriptor);

            Assert.IsFalse(result);
        }


        [TestMethod]
        public void Retrieve_TerminalDescriptor_ProxyDataStore()
        {
            DataStoreMock
                .Setup(_ => _.Retrieve(TerminalDescriptor))
                .Returns(Data)
                .Verifiable();

            var result = TestInstance.Retrieve(TerminalDescriptor);

            DataStoreMock.Verify();
            Assert.AreSame(Data, result);
        }


        [TestMethod]
        public void Retrieve_NonTerminalDescriptor_DataOfTheTerminalDescriptorsThatComposeIt()
        {
            var terminalDescriptors = 
                ArraysHelper.CreateWithContent(
                    new Mock<ITerminalDescriptor>().Object,
                    new Mock<ITerminalDescriptor>().Object,
                    new Mock<ITerminalDescriptor>().Object
                );
            var data = new IData[terminalDescriptors.Length];

            for (var i = 0; i < terminalDescriptors.Length; ++i)
            {
                var terminalDescriptor = terminalDescriptors[i];
                var dataItem = new Mock<IData>().Object;
                
                TestInstanceMock
                    .Setup<IData>(_ => _.Retrieve(terminalDescriptor))
                    .Returns(dataItem);

                data[i] = dataItem;
            }

            CompositionsMock
                .Setup(_ => _.Retrieve(NonTerminalDescriptor))
                .Returns(() => terminalDescriptors);

            var result = TestInstance.Retrieve(NonTerminalDescriptor);

            Assert.IsTrue(data.Equivalent(result));
        }


        [TestMethod]
        public void DescribeAndSave__CreatesTerminalDescriptorsAndSavesData_TerminalDescriptorsReturned()
        {
            var dataMocks = ArraysHelper.CreateWithContent(new Mock<IData>(), new Mock<IData>(), new Mock<IData>());
            var data = dataMocks.Select(_ => _.Object);
            var terminalDescriptors = new ITerminalDescriptor[dataMocks.Length];

            for (var i = 0; i < dataMocks.Length; ++i)
            {
                var dataMock = dataMocks[i];
                var dataItem = dataMock.Object;
                var terminalDescriptor = new Mock<ITerminalDescriptor>().Object;

                terminalDescriptors[i] = terminalDescriptor;

                TerminalDescriptorsFactoryMock
                    .Setup<ITerminalDescriptor>(_ => _.Create(dataItem))
                    .Returns(terminalDescriptor)
                    .Verifiable();

                DataStoreMock
                    .Setup(_ => _.Save(terminalDescriptor, dataMock.Object))
                    .Verifiable();
            }

            var result = ReflectionHelper.Invoke(TestInstance, "DescribeAndSave", data)
                            as IEnumerable<ITerminalDescriptor>;

            TerminalDescriptorsFactoryMock.Verify();
            DataStoreMock.Verify();
            Assert.IsTrue(terminalDescriptors.Equivalent(result));
        }


        [TestMethod]
        public void UpdateCompositionAndProvideAdditions__ProxiesNonTerminalDescriptorCompositionsStore()
        {
            var additions = ArraysHelper.CreateWithContent(new Mock<ITerminalDescriptor>().Object, new Mock<ITerminalDescriptor>().Object);

            CompositionsMock
                .Setup(_ => _.UpdateAndProvideAdditions(NonTerminalDescriptor, TerminalDescriptorsCollection))
                .Returns(additions)
                .Verifiable();

            var result = TestInstance.UpdateCompositionAndProvideAdditions(NonTerminalDescriptor, TerminalDescriptorsCollection);

            CompositionsMock.Verify();
            Assert.AreSame(additions, result);
        }


        [TestMethod]
        public void FindIntersectingDescriptors__AllIntersectingDescriptors()
        {
            var providedDescriptor = new Mock<IDescriptor>().Object;
            var disjunctDescriptorMocks = ArraysHelper.CreateWithContent(new Mock<IDescriptor>(), new Mock<IDescriptor>());
            var intersectingDescriptorMocks = ArraysHelper.CreateWithContent(new Mock<IDescriptor>(), new Mock<IDescriptor>());
            var allDescriptorMocks = intersectingDescriptorMocks.ShuffledMerge(disjunctDescriptorMocks);

            var intersectingDescriptors = intersectingDescriptorMocks.Select(_ => _.Object);
            var allDescriptors = allDescriptorMocks.Select(_ => _.Object);

            disjunctDescriptorMocks.ForEach(_ => _.Setup(__ => __.Intersects(providedDescriptor)).Returns(false));
            intersectingDescriptorMocks.ForEach(_ => _.Setup(__ => __.Intersects(providedDescriptor)).Returns(true));

            TestInstanceMockProtected
                .Setup<IEnumerable<IDescriptor>>("RetrieveAllDescriptors")
                .Returns(allDescriptors)
                .Verifiable();

            var result = TestInstance.FindIntersectingDescriptors(providedDescriptor);

            TestInstanceMock.Verify();
            allDescriptorMocks.ForEach(_ => _.VerifyAll());
            Assert.IsTrue(intersectingDescriptors.Equivalent(result));
        }


        [TestMethod]
        public void RetrieveAllDescriptors__TerminalDescriptorsRetrieved()
        {
            var terminalDescriptorMocks = ArraysHelper.CreateWithContent(new Mock<ITerminalDescriptor>(), new Mock<ITerminalDescriptor>());
            var terminalDescriptors = terminalDescriptorMocks.Select(_ => _.Object);

            CompositionsMock.Setup(_ => _.RetrieveDescriptors()).Returns(new INonTerminalDescriptor[0]);
            DataStoreMock.Setup(_ => _.RetrieveDescriptors()).Returns(terminalDescriptors).Verifiable();

            var result = (IEnumerable<IDescriptor>)ReflectionHelper.Invoke(TestInstance, "RetrieveAllDescriptors");

            DataStoreMock.Verify();
            Assert.IsTrue(terminalDescriptors.Equivalent(result));
        }

        [TestMethod]
        public void RetrieveAllDescriptors__NonTerminalDescriptorsRetrieved()
        {
            var nonTerminalDescriptorMocks = ArraysHelper.CreateWithContent(new Mock<INonTerminalDescriptor>(), new Mock<INonTerminalDescriptor>());
            var nonTerminalDescriptors = nonTerminalDescriptorMocks.Select(_ => _.Object);

            DataStoreMock.Setup(_ => _.RetrieveDescriptors()).Returns(new ITerminalDescriptor[0]);
            CompositionsMock.Setup(_ => _.RetrieveDescriptors()).Returns(nonTerminalDescriptors).Verifiable();

            var result = (IEnumerable<IDescriptor>)ReflectionHelper.Invoke(TestInstance, "RetrieveAllDescriptors");

            CompositionsMock.Verify();
            Assert.IsTrue(nonTerminalDescriptors.Equivalent(result));
        }
    }
}
