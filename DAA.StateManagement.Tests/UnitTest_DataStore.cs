using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;
using Moq.Protected;

using DAA.Helpers;
using DAA.StateManagement.DataManagement;
using DAA.StateManagement.Interfaces;
using System;

namespace DAA.StateManagement.Tests
{
    [TestClass]
    public class UnitTest_DataStore
    {
        private IDataManipulator<IData> DataManipulator { get => this.DataManipulatorMock.Object; }
        private Mock<IDataManipulator<IData>> DataManipulatorMock { get; set; }

        private ITerminalDescriptor Descriptor { get => this.DescriptorMock.Object; }
        private Mock<ITerminalDescriptor> DescriptorMock { get; set; }

        private DataStore<IData> TestInstance { get => this.TestInstanceMock.Object; }
        private Mock<DataStore<IData>> TestInstanceMock { get; set; }
        private IProtectedMock<DataStore<IData>> TestInstanceMockProtected { get => this.TestInstanceMock.Protected(); }


        [TestInitialize]
        public void BeforeEach()
        {
            this.DataManipulatorMock = new Mock<IDataManipulator<IData>>();
            this.DescriptorMock = new Mock<ITerminalDescriptor>();

            this.TestInstanceMock = new Mock<DataStore<IData>>(this.DataManipulator);
            this.TestInstanceMock.CallBase = true;
        }


        [TestMethod]
        public void GetDataManipulator__ProvidedValue()
        {
            var testInstance = new DataStore<IData>(this.DataManipulator);

            var result = ReflectionHelper.Invoke(testInstance, "DataManipulator");

            Assert.AreSame(this.DataManipulator, result);
        }


        [TestMethod]
        public void Update__InitialDataUpdatedFromSpecifiedInstance()
        {
            var initialData = new Mock<IData>().Object;
            var newData = new Mock<IData>().Object;

            this.TestInstanceMock
                .Setup(_ => _.Retrieve(this.Descriptor))
                .Returns(initialData);

            this.TestInstance.Update(this.Descriptor, newData);

            this.DataManipulatorMock.Verify(_ => _.Update(initialData, newData));
        }
    }
}
