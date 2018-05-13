using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Moq.Protected;

using DAA.Helpers;
using DAA.StateManagement.Interfaces;

namespace DAA.StateManagement.Tests
{
    [TestClass]
    public class UnitTest_DataPool
    {
        private Mock<IData> DataMock { get; set; }
        private Mock<IDescriptor> DescriptorMock { get; set; }
        private Mock<IEnumerable<IData>> DataCollectionMock { get; set; }
        private Mock<ITerminalDescriptor> TerminalDescriptorMock { get; set; }
        private Mock<INonTerminalDescriptor> NonTerminalDescriptorMock { get; set; }
        private Mock<IEnumerable<ITerminalDescriptor>> TerminalDescriptorsCollectionMock { get; set; }
        private Mock<ITerminalDescriptorsFlyweightFactory> TerminalDescriptorsFlyweightFactoryMock { get; set; }
        private Mock<DataPool<IData>> TestInstanceMock { get; set; }

        private IData Data { get => this.DataMock.Object; }
        private IDescriptor Descriptor { get => this.DescriptorMock.Object; }
        private IEnumerable<IData> DataCollection { get => this.DataCollectionMock.Object; }
        private ITerminalDescriptor TerminalDescriptor { get => this.TerminalDescriptorMock.Object; }
        private INonTerminalDescriptor NonTerminalDescriptor { get => this.NonTerminalDescriptorMock.Object; }
        private IEnumerable<ITerminalDescriptor> TerminalDescriptorsCollection { get => this.TerminalDescriptorsCollectionMock.Object; }
        private ITerminalDescriptorsFlyweightFactory TerminalDescriptorsFlyweightFactory { get => this.TerminalDescriptorsFlyweightFactoryMock.Object; }
        private DataPool<IData> TestInstance { get => this.TestInstanceMock.Object; }
        private IProtectedMock<DataPool<IData>> TestInstanceMockProtected { get => this.TestInstanceMock.Protected(); }


        [TestInitialize]
        public void BeforeEach()
        {
            this.DataMock = new Mock<IData>();
            this.DescriptorMock = new Mock<IDescriptor>();
            this.DataCollectionMock = new Mock<IEnumerable<IData>>();
            this.TerminalDescriptorMock = new Mock<ITerminalDescriptor>();
            this.NonTerminalDescriptorMock = new Mock<INonTerminalDescriptor>();
            this.TerminalDescriptorsCollectionMock = new Mock<IEnumerable<ITerminalDescriptor>>();
            this.TerminalDescriptorsFlyweightFactoryMock = new Mock<ITerminalDescriptorsFlyweightFactory>();

            this.TestInstanceMock = new Mock<DataPool<IData>>(this.TerminalDescriptorsFlyweightFactory);
            this.TestInstanceMock.CallBase = true;
        }


        [TestMethod]
        public void GetTerminalDescriptorsFlyweightFactory__ProvidedValue()
        {
            var testInstance = new DataPool<IData>(this.TerminalDescriptorsFlyweightFactory);

            var result = ReflectionHelper.Invoke(testInstance, "TerminalDescriptorsFlyweightFactory");

            Assert.IsTrue(Object.ReferenceEquals(this.TerminalDescriptorsFlyweightFactory, result));
        }


        [TestMethod]
        public void Store__DataStored()
        {
            this.TestInstanceMockProtected.Setup("StoreDataAndProvideTerminalDescriptors", ItExpr.IsAny<IEnumerable<IData>>());
            this.TestInstanceMockProtected.Setup("StoreNonTerminalDescriptorIfNonTerminal", ItExpr.IsAny<IDescriptor>(), ItExpr.IsAny<IEnumerable<ITerminalDescriptor>>());

            this.TestInstance.Store(this.Descriptor, this.DataCollection);

            this.TestInstanceMockProtected.Verify("StoreDataAndProvideTerminalDescriptors", Times.Once(), this.DataCollection);
        }

        [TestMethod]
        public void Store__NonTerminalDescriptorStoredIfNecessary()
        {
            this.TestInstanceMockProtected
                .Setup<IEnumerable<ITerminalDescriptor>>("StoreDataAndProvideTerminalDescriptors", ItExpr.IsAny<IEnumerable<IData>>())
                .Returns(this.TerminalDescriptorsCollection);
            this.TestInstanceMockProtected.Setup("StoreNonTerminalDescriptorIfNonTerminal", ItExpr.IsAny<IDescriptor>(), ItExpr.IsAny<IEnumerable<ITerminalDescriptor>>());

            this.TestInstance.Store(this.Descriptor, this.DataCollection);

            this.TestInstanceMockProtected.Verify("StoreNonTerminalDescriptorIfNonTerminal", Times.Once(), this.Descriptor, this.TerminalDescriptorsCollection);
        }


        [TestMethod]
        public void StoreNonTerminalDescriptorIfNonTerminal_NonTerminalDescriptor_Stored()
        {
            this.TestInstanceMockProtected.Setup("StoreNonTerminalDescriptor", ItExpr.IsAny<INonTerminalDescriptor>(), ItExpr.IsAny<IEnumerable<ITerminalDescriptor>>());

            ReflectionHelper.Invoke(this.TestInstance, "StoreNonTerminalDescriptorIfNonTerminal", this.NonTerminalDescriptor, this.TerminalDescriptorsCollection);

            this.TestInstanceMockProtected.Verify("StoreNonTerminalDescriptor", Times.Once(), this.NonTerminalDescriptor, this.TerminalDescriptorsCollection);
        }

        [TestMethod]
        public void StoreNonTerminalDescriptorIfNonTerminal_TerminalDescriptor_NotStored()
        {
            this.TestInstanceMockProtected.Setup("StoreNonTerminalDescriptor", ItExpr.IsAny<INonTerminalDescriptor>(), ItExpr.IsAny<IEnumerable<ITerminalDescriptor>>());

            ReflectionHelper.Invoke(this.TestInstance, "StoreNonTerminalDescriptorIfNonTerminal", this.TerminalDescriptor, this.TerminalDescriptorsCollection);

            this.TestInstanceMockProtected.Verify("StoreNonTerminalDescriptor", Times.Never(), ItExpr.IsAny<INonTerminalDescriptor>(), ItExpr.IsAny<IEnumerable<ITerminalDescriptor>>());
        }


        [TestMethod]
        public void StoreNonTerminalDescriptor__RegisteredIfUnknown()
        {
            this.TestInstanceMockProtected.Setup("RegisterNonTerminalDescriptorIfUnknown", ItExpr.IsAny<INonTerminalDescriptor>());
            this.TestInstanceMock.Setup(_ => _.UpdateNonTerminalDescriptorComposition(It.IsAny<INonTerminalDescriptor>(), It.IsAny<IEnumerable<ITerminalDescriptor>>()));

            ReflectionHelper.Invoke(this.TestInstance, "StoreNonTerminalDescriptor", this.NonTerminalDescriptor, this.TerminalDescriptorsCollection);

            this.TestInstanceMockProtected.Verify("RegisterNonTerminalDescriptorIfUnknown", Times.Once(), this.NonTerminalDescriptor);
        }

        [TestMethod]
        public void StoreNonTerminalDescriptor__CompositionUpdated()
        {
            this.TestInstanceMockProtected.Setup("RegisterNonTerminalDescriptorIfUnknown", ItExpr.IsAny<INonTerminalDescriptor>());
            this.TestInstanceMock.Setup(_ => _.UpdateNonTerminalDescriptorComposition(It.IsAny<INonTerminalDescriptor>(), It.IsAny<IEnumerable<ITerminalDescriptor>>()));

            ReflectionHelper.Invoke(this.TestInstance, "StoreNonTerminalDescriptor", this.NonTerminalDescriptor, this.TerminalDescriptorsCollection);

            this.TestInstanceMock.Verify(_ => _.UpdateNonTerminalDescriptorComposition(this.NonTerminalDescriptor, this.TerminalDescriptorsCollection), Times.Once());
        }


        [TestMethod]
        public void RegisterNonTerminalDescriptorIfUnknown_NotContained_Registered()
        {
            this.TestInstanceMockProtected.Setup("RegisterNonTerminalDescriptor", ItExpr.IsAny<INonTerminalDescriptor>());
            this.TestInstanceMockProtected.Setup<bool>("ContainsNonTerminalDescriptor", ItExpr.IsAny<INonTerminalDescriptor>()).Returns(false);

            ReflectionHelper.Invoke(this.TestInstance, "RegisterNonTerminalDescriptorIfUnknown", this.NonTerminalDescriptor);

            this.TestInstanceMockProtected.Verify("RegisterNonTerminalDescriptor", Times.Once(), this.NonTerminalDescriptor);
        }

        [TestMethod]
        public void RegisterNonTerminalDescriptorIfUnknown_Contained_NotRegistered()
        {
            this.TestInstanceMockProtected.Setup("RegisterNonTerminalDescriptor", ItExpr.IsAny<INonTerminalDescriptor>());
            this.TestInstanceMockProtected.Setup<bool>("ContainsNonTerminalDescriptor", this.NonTerminalDescriptor).Returns(true);

            ReflectionHelper.Invoke(this.TestInstance, "RegisterNonTerminalDescriptorIfUnknown", this.NonTerminalDescriptor);

            this.TestInstanceMockProtected.Verify("RegisterNonTerminalDescriptor", Times.Never(), this.NonTerminalDescriptor);
        }


        [TestMethod]
        public void Contains_NonTerminalContained_True()
        {
            this.TestInstanceMockProtected.Setup<bool>("ContainsNonTerminalDescriptor", this.NonTerminalDescriptor).Returns(true);

            var result = this.TestInstance.Contains(this.NonTerminalDescriptor);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Contains_NonTerminalNotContained_False()
        {
            this.TestInstanceMockProtected.Setup<bool>("ContainsNonTerminalDescriptor", ItExpr.IsAny<INonTerminalDescriptor>()).Returns(false);

            var result = this.TestInstance.Contains(this.NonTerminalDescriptor);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Contains_TerminalContained_True()
        {
            this.TestInstanceMockProtected.Setup<bool>("ContainsTerminalDescriptor", this.TerminalDescriptor).Returns(true);

            var result = this.TestInstance.Contains(this.TerminalDescriptor);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Contains_TerminalNotContained_False()
        {
            this.TestInstanceMockProtected.Setup<bool>("ContainsTerminalDescriptor", ItExpr.IsAny<ITerminalDescriptor>()).Returns(false);

            var result = this.TestInstance.Contains(this.TerminalDescriptor);

            Assert.IsFalse(result);
        }


        [TestMethod]
        public void StoreDataAndProvideTerminalDescriptors__CreatesAndStoresTerminalDescriptorsForAllData_TerminalDescriptorsReturned()
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

                this.TestInstanceMockProtected
                    .Setup("StoreTerminalDescriptor", terminalDescriptor, dataMock.Object)
                    .Verifiable();
            }

            var result = ReflectionHelper.Invoke(this.TestInstance, "StoreDataAndProvideTerminalDescriptors", data)
                            as IEnumerable<ITerminalDescriptor>;

            this.TerminalDescriptorsFlyweightFactoryMock.VerifyAll();
            this.TestInstanceMock.VerifyAll();
            Assert.IsTrue(terminalDescriptors.Equivalent(result));
        }


        [TestMethod]
        public void StoreTerminalDescriptor_DescriptorNotContained_Registered()
        {
            this.TestInstanceMockProtected.Setup("RegisterTerminalDescriptor", ItExpr.IsAny<ITerminalDescriptor>(), ItExpr.IsAny<IData>());
            this.TestInstanceMockProtected.Setup("UpdateData", ItExpr.IsAny<ITerminalDescriptor>(), ItExpr.IsAny<IData>());
            this.TestInstanceMockProtected.Setup<bool>("ContainsTerminalDescriptor", ItExpr.IsAny<ITerminalDescriptor>()).Returns(false);

            ReflectionHelper.Invoke(this.TestInstance, "StoreTerminalDescriptor", this.TerminalDescriptor, this.Data);

            this.TestInstanceMockProtected.Verify("RegisterTerminalDescriptor", Times.Once(), this.TerminalDescriptor, this.Data);
        }

        [TestMethod]
        public void StoreTerminalDescriptor_DescriptorNotContained_DataNotUpdated()
        {
            this.TestInstanceMockProtected.Setup("RegisterTerminalDescriptor", ItExpr.IsAny<ITerminalDescriptor>(), ItExpr.IsAny<IData>());
            this.TestInstanceMockProtected.Setup("UpdateData", ItExpr.IsAny<ITerminalDescriptor>(), ItExpr.IsAny<IData>());
            this.TestInstanceMockProtected.Setup<bool>("ContainsTerminalDescriptor", ItExpr.IsAny<ITerminalDescriptor>()).Returns(false);

            ReflectionHelper.Invoke(this.TestInstance, "StoreTerminalDescriptor", this.TerminalDescriptor, this.Data);

            this.TestInstanceMockProtected.Verify("UpdateData", Times.Never(), ItExpr.IsAny<ITerminalDescriptor>(), ItExpr.IsAny<IData>());
        }

        [TestMethod]
        public void StoreTerminalDescriptor_DescriptorContained_DataUpdated()
        {
            this.TestInstanceMockProtected.Setup("RegisterTerminalDescriptor", ItExpr.IsAny<ITerminalDescriptor>(), ItExpr.IsAny<IData>());
            this.TestInstanceMockProtected.Setup("UpdateData", ItExpr.IsAny<ITerminalDescriptor>(), ItExpr.IsAny<IData>());
            this.TestInstanceMockProtected.Setup<bool>("ContainsTerminalDescriptor", this.TerminalDescriptor).Returns(true);

            ReflectionHelper.Invoke(this.TestInstance, "StoreTerminalDescriptor", this.TerminalDescriptor, this.Data);

            this.TestInstanceMockProtected.Verify("UpdateData", Times.Once(), this.TerminalDescriptor, this.Data);
        }

        [TestMethod]
        public void StoreTerminalDescriptor_DescriptorContained_NotReRegistered()
        {
            this.TestInstanceMockProtected.Setup("RegisterTerminalDescriptor", ItExpr.IsAny<ITerminalDescriptor>(), ItExpr.IsAny<IData>());
            this.TestInstanceMockProtected.Setup("UpdateData", ItExpr.IsAny<ITerminalDescriptor>(), ItExpr.IsAny<IData>());
            this.TestInstanceMockProtected.Setup<bool>("ContainsTerminalDescriptor", this.TerminalDescriptor).Returns(true);

            ReflectionHelper.Invoke(this.TestInstance, "StoreTerminalDescriptor", this.TerminalDescriptor, this.Data);

            this.TestInstanceMockProtected.Verify("RegisterTerminalDescriptor", Times.Never(), this.TerminalDescriptor, this.Data);
        }
    }
}
