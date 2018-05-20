﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Moq.Protected;

using DAA.Helpers;
using DAA.StateManagement.DataManagement;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement.Tests
{
    [TestClass]
    public class UnitTest_DataPool
    {
        private ITerminalDescriptorsFlyweightFactory TerminalDescriptorsFlyweightFactory { get => this.TerminalDescriptorsFlyweightFactoryMock.Object; }
        private Mock<ITerminalDescriptorsFlyweightFactory> TerminalDescriptorsFlyweightFactoryMock { get; set; }

        private DataStore<IData> DataStore { get => this.DataStoreMock.Object; }
        private Mock<DataStore<IData>> DataStoreMock { get; set; }

        private NonTerminalDescriptorCompositionsStore NonTerminalDescriptorCompositions { get => this.NonTerminalDescriptorCompositionsMock.Object; }
        private Mock<NonTerminalDescriptorCompositionsStore> NonTerminalDescriptorCompositionsMock { get; set; }

        private IData Data { get => this.DataMock.Object; }
        private Mock<IData> DataMock { get; set; }

        private IEnumerable<IData> DataCollection { get => this.DataCollectionMock.Object; }
        private Mock<IEnumerable<IData>> DataCollectionMock { get; set; }

        private ITerminalDescriptor TerminalDescriptor { get => this.TerminalDescriptorMock.Object; }
        private Mock<ITerminalDescriptor> TerminalDescriptorMock { get; set; }

        private INonTerminalDescriptor NonTerminalDescriptor { get => this.NonTerminalDescriptorMock.Object; }
        private Mock<INonTerminalDescriptor> NonTerminalDescriptorMock { get; set; }

        private IEnumerable<ITerminalDescriptor> TerminalDescriptorsCollection { get => this.TerminalDescriptorsCollectionMock.Object; }
        private Mock<IEnumerable<ITerminalDescriptor>> TerminalDescriptorsCollectionMock { get; set; }

        private DataPool<IData> TestInstance { get => this.TestInstanceMock.Object; }
        private Mock<DataPool<IData>> TestInstanceMock { get; set; }
        private IProtectedMock<DataPool<IData>> TestInstanceMockProtected { get => this.TestInstanceMock.Protected(); }


        [TestInitialize]
        public void BeforeEach()
        {
            this.TerminalDescriptorsFlyweightFactoryMock = new Mock<ITerminalDescriptorsFlyweightFactory>();
            this.DataStoreMock = new Mock<DataStore<IData>>();
            this.NonTerminalDescriptorCompositionsMock = new Mock<NonTerminalDescriptorCompositionsStore>();
            this.DataMock = new Mock<IData>();
            this.DataCollectionMock = new Mock<IEnumerable<IData>>();
            this.TerminalDescriptorMock = new Mock<ITerminalDescriptor>();
            this.NonTerminalDescriptorMock = new Mock<INonTerminalDescriptor>();
            this.TerminalDescriptorsCollectionMock = new Mock<IEnumerable<ITerminalDescriptor>>();

            this.TestInstanceMock = new Mock<DataPool<IData>>(this.TerminalDescriptorsFlyweightFactory);
            this.TestInstanceMock.CallBase = true;

            this.TestInstanceMockProtected
                .SetupGet<DataStore<IData>>("Data")
                .Returns(this.DataStore);
            this.TestInstanceMockProtected
                .SetupGet<NonTerminalDescriptorCompositionsStore>("NonTerminalDescriptorCompositions")
                .Returns(this.NonTerminalDescriptorCompositions);
        }


        [TestMethod]
        public void GetTerminalDescriptorsFlyweightFactory__ProvidedValue()
        {
            var testInstance = new DataPool<IData>(this.TerminalDescriptorsFlyweightFactory);

            var result = ReflectionHelper.Invoke(testInstance, "TerminalDescriptorsFlyweightFactory");

            Assert.AreSame(this.TerminalDescriptorsFlyweightFactory, result);
        }

        [TestMethod]
        public void GetData__NotNull_Constant()
        {
            var testInstance = new DataPool<IData>(this.TerminalDescriptorsFlyweightFactory);

            var resultOne = ReflectionHelper.Invoke(testInstance, "Data");
            var resultTwo = ReflectionHelper.Invoke(testInstance, "Data");

            Assert.IsNotNull(resultOne);
            Assert.IsNotNull(resultTwo);
            Assert.AreSame(resultOne, resultTwo);
        }

        [TestMethod]
        public void GetNonTerminalDescriptorCompositions__NotNull_Constant()
        {
            var testInstance = new DataPool<IData>(this.TerminalDescriptorsFlyweightFactory);

            var resultOne = ReflectionHelper.Invoke(testInstance, "NonTerminalDescriptorCompositions");
            var resultTwo = ReflectionHelper.Invoke(testInstance, "NonTerminalDescriptorCompositions");

            Assert.IsNotNull(resultOne);
            Assert.IsNotNull(resultTwo);
            Assert.AreSame(resultOne, resultTwo);
        }


        [TestMethod]
        public void Save_TerminalDescriptor_DataSaved()
        {
            this.TestInstance.Save(this.TerminalDescriptor, this.Data);

            this.DataStoreMock.Verify(_ => _.Save(this.TerminalDescriptor, this.Data));
        }


        [TestMethod]
        public void Save_NonTerminalDescriptor_DataSaved()
        {
            this.TestInstanceMockProtected.Setup("DescribeAndSaveData", ItExpr.IsAny<IEnumerable<IData>>());

            this.TestInstance.Save(this.NonTerminalDescriptor, this.DataCollection);

            this.TestInstanceMockProtected.Verify("DescribeAndSaveData", Times.Once(), this.DataCollection);
        }

        [TestMethod]
        public void Save_NonTerminalDescriptor_CompositionUpdated()
        {
            this.TestInstanceMockProtected
                .Setup<IEnumerable<ITerminalDescriptor>>("DescribeAndSaveData", ItExpr.IsAny<IEnumerable<IData>>())
                .Returns(this.TerminalDescriptorsCollection);

            this.TestInstance.Save(this.NonTerminalDescriptor, this.DataCollection);

            this.NonTerminalDescriptorCompositionsMock.Verify(_ => _.Save(this.NonTerminalDescriptor, this.TerminalDescriptorsCollection), Times.Once());
        }


        [TestMethod]
        public void Contains_NonTerminalContained_True()
        {
            this.NonTerminalDescriptorCompositionsMock
                .Setup(_ => _.Contains(this.NonTerminalDescriptor))
                .Returns(true);

            var result = this.TestInstance.Contains(this.NonTerminalDescriptor);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Contains_NonTerminalNotContained_False()
        {
            this.NonTerminalDescriptorCompositionsMock
                .Setup(_ => _.Contains(this.NonTerminalDescriptor))
                .Returns(false);

            var result = this.TestInstance.Contains(this.NonTerminalDescriptor);

            Assert.IsFalse(result);
        }


        [TestMethod]
        public void Contains_TerminalContained_True()
        {
            this.DataStoreMock
                .Setup(_ => _.Contains(this.TerminalDescriptor))
                .Returns(true);

            var result = this.TestInstance.Contains(this.TerminalDescriptor);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Contains_TerminalNotContained_False()
        {
            this.DataStoreMock
                .Setup(_ => _.Contains(this.TerminalDescriptor))
                .Returns(false);

            var result = this.TestInstance.Contains(this.TerminalDescriptor);

            Assert.IsFalse(result);
        }


        [TestMethod]
        public void Retrieve_TerminalDescriptor_ProxyDataStore()
        {
            this.DataStoreMock
                .Setup(_ => _.Retrieve(this.TerminalDescriptor))
                .Returns(this.Data)
                .Verifiable();

            var result = this.TestInstance.Retrieve(this.TerminalDescriptor);

            this.DataStoreMock.Verify();
            Assert.AreSame(this.Data, result);
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

            for (int i = 0; i < terminalDescriptors.Length; ++i)
            {
                var terminalDescriptor = terminalDescriptors[i];
                var dataItem = new Mock<IData>().Object;
                
                this.TestInstanceMock
                    .Setup<IData>(_ => _.Retrieve(terminalDescriptor))
                    .Returns(dataItem);

                data[i] = dataItem;
            }

            this.NonTerminalDescriptorCompositionsMock
                .Setup(_ => _.Retrieve(this.NonTerminalDescriptor))
                .Returns(() => terminalDescriptors);

            var result = this.TestInstance.Retrieve(this.NonTerminalDescriptor);

            Assert.IsTrue(data.Equivalent(result));
        }


        [TestMethod]
        public void DescribeAndSaveData__CreatesTerminalDescriptorsAndSavesData_TerminalDescriptorsReturned()
        {
            var dataMocks = ArraysHelper.CreateWithContent(new Mock<IData>(), new Mock<IData>(), new Mock<IData>());
            var data = dataMocks.Select(_ => _.Object);
            var terminalDescriptors = new ITerminalDescriptor[dataMocks.Length];

            for (int i = 0; i < dataMocks.Length; ++i)
            {
                var dataIdentifier = new object();
                var dataMock = dataMocks[i];
                var terminalDescriptor = new Mock<ITerminalDescriptor>().Object;

                terminalDescriptors[i] = terminalDescriptor;

                dataMock
                    .Setup<object>(_ => _.DataIdentifier)
                    .Returns(dataIdentifier);

                this.TerminalDescriptorsFlyweightFactoryMock
                    .Setup<ITerminalDescriptor>(_ => _.Create(dataIdentifier))
                    .Returns(terminalDescriptor)
                    .Verifiable();

                this.DataStoreMock
                    .Setup(_ => _.Save(terminalDescriptor, dataMock.Object))
                    .Verifiable();
            }

            var result = ReflectionHelper.Invoke(this.TestInstance, "DescribeAndSaveData", data)
                            as IEnumerable<ITerminalDescriptor>;

            this.TerminalDescriptorsFlyweightFactoryMock.Verify();
            this.DataStoreMock.Verify();
            Assert.IsTrue(terminalDescriptors.Equivalent(result));
        }
    }
}
